using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MatchSpawner))]
public class MatchManager : NetworkBehaviour
{
    [SerializeField] private GameObject loadingGui;
    [Header("Countdowns")]
    [SerializeField]
    private Countdown MatchCountdown;

    [Header("Lists")]
    [SerializeField] private GameEvent OnSceneChange;
    public List<PlayerController> all = new();
    public List<PlayerController> hiders = new();
    public List<PlayerController> hunters = new();
    [SerializeField] private int connectedPlayersCount;
    [SerializeField] private GameEvent OnAllPlayersReady;
    [SerializeField] private GameEvent OnMatchEnd;
    [SerializeField] private PropController[] props;

    [Header("Audio")]
    [SerializeField] private AudioSource GameMusic;
    [SerializeField] private AudioSource HidingMusic;


    public bool endMatchNow;
    public Role winnerTeam = Role.HIDER;

    public PropController GetProp(string targetName)
    {
        return props.FirstOrDefault(prop => prop.name == targetName);
    }


    public static MatchManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void DeactivateLoadingGuiRpc()
    {
        loadingGui.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            foreach (PlayerController player in GameManager.Instance.All)
            {
                all.Add(player);

                if (player.GetComponent<RoleController>().role.Value == Role.HUNTER)
                {
                    hunters.Add(player);
                    GetComponent<MatchSpawner>().SpawnPlayer(Role.HUNTER, player);
                }
                else
                {
                    hiders.Add(player);
                    GetComponent<MatchSpawner>().SpawnPlayer(Role.HIDER, player);
                }

                if (player.GetComponent<PlayerController>().isDummy) PlusPlusRpc();
            }
        }

        if (IsClient)
        {
            PlusPlusRpc();
        }
    }

    public void Start()
    {
        OnSceneChange.Raise();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void StartGameMusicRpc()
    {
        HidingMusic.Stop();
        GameMusic.Play();
    }


    public void StopGameMusic()
    {
        GameMusic.Stop();
    }


    [Rpc(SendTo.Server)]
    private void PlusPlusRpc()
    {
        connectedPlayersCount++;
        if (connectedPlayersCount < all.Count) return;
        StartCoroutine(DelayPlayersReady());
    }

    private IEnumerator DelayPlayersReady()
    {
        yield return new WaitForSeconds(1);
        OnAllPlayersReady.Raise();
    }

    public void StartGame()
    {

        if (!IsServer) return;

        foreach (PlayerController player in all)
        {
            if (player.GetComponent<RoleController>().role.Value == Role.HUNTER)
                player.GetComponent<PlayerController>().speed = 0;
            player.GetComponent<PlayerMatchController>().StartGameServer();
            player.GetComponent<PlayerMatchController>().StartGameClientRpc();
        }

        endMatchNow = CheckMatchEnd();
    }

    //la vida no se ve en los demas players
    public void Kill(PlayerController player)
    {
        if (!IsServer) return;
        if (hiders.Contains(player))
        {
            hiders.Remove(player);
        }
        else if (hunters.Contains(player))
        {
            hunters.Remove(player);
        }

        player.GetComponent<HealthController>().DieRpc();
        if (CheckMatchEnd())
            OnMatchEnd.Raise();
    }

    private bool CheckMatchEnd()
    {
        if (!IsServer) return false;
        if (hiders.Count == 0)
        {
            Debug.Log("Hunters win");
            LogManager.Instance.LogAllRpc("Hunters win!", LogType.DANGER);
            winnerTeam = Role.HUNTER;
            return true;
        }

        if (hunters.Count == 0)
        {
            Debug.Log("Hiders win");
            LogManager.Instance.LogAllRpc("Hiders win!", LogType.SUCCESS);
            winnerTeam = Role.HIDER;
            return true;
        }

        return false;
    }

    public void HandleClientDisconnect(ulong clientId)
    {
        if (!IsServer) return;
        PlayerController player = GameManager.Instance.GetDisconnectedPlayer(clientId);
        if (player == null) return;
        all.Remove(player);
        switch (player.GetComponent<RoleController>().role.Value)
        {
            case Role.HIDER:
                hiders.Remove(player);
                break;
            case Role.HUNTER:
                hunters.Remove(player);
                break;
        }

        EndPlayer(player);
        player.GetComponent<NetworkObject>().Despawn();
        Destroy(player.gameObject);

        if (CheckMatchEnd())
            OnMatchEnd.Raise();
    }

    private void EndPlayer(PlayerController player)
    {
        player.GetComponent<ShootController>().m_Action = null;
        player.GetComponent<RewardExecutorReveal>().HideMinimapDotRpc();
        player.GetComponent<PlayerGUIController>().DestroyClientInfoBoxRpc();
        player.GetComponent<PlayerMissionController>().EndMissions();
        player.GetComponent<PlayerRewardController>().EndRewards();
        player.GetComponent<TransformController>().DestransformRpc();
        player.GetComponent<PlayerMatchController>().EndMatchServer();
        player.GetComponent<PlayerMatchController>().EndMatchClientRpc();
        player.GetComponent<HealthController>().isDead = false;
        player.GetComponent<HealthController>().RebornRpc();
    }

    public void EndMatch()
    {
        GUIManager.Instance.DestroyPlayersGUI();
        foreach (PlayerController player in all)
        {
            EndPlayer(player);
        }

        ActivateLoadingGuiRpc();
        NetworkManager.Singleton.SceneManager.LoadScene(Constants.Instance.LobbyScene, LoadSceneMode.Single);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ActivateLoadingGuiRpc()
    {
        loadingGui.SetActive(true);
    }

}
