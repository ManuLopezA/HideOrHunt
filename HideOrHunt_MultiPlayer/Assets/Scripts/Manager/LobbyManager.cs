using TMPro;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LobbyManager : NetworkBehaviour
{
    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new();

    int spawnIndex = 0;

    [SerializeField] private int minPlayers;

    public NetworkVariable<int> playerCount = new();
    public NetworkVariable<int> huntCount = new();
    public NetworkVariable<int> hideCount = new();

    private List<ulong> connectedClients = new();

    [SerializeField] private TextMeshProUGUI huntLabel;
    [SerializeField] private TextMeshProUGUI hideLabel;

    [SerializeField] private Countdown countdownJoining;
    [SerializeField] private Countdown countdownVoting;

    [SerializeField] private GameEvent OnSceneChange;

    [SerializeField] private GameObject lobbyGui;
    [SerializeField] public GameObject voteGui;
    [SerializeField] private GameObject loadingGui;

    public static LobbyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        playerCount.OnValueChanged += UpdateRoleLabels;
        huntCount.OnValueChanged += UpdateHunterLabel;
        hideCount.OnValueChanged += UpdateHiderLabel;
        if (IsServer)
        {
            foreach (PlayerController p in GameManager.Instance.All)
            {
                p.speed = Constants.Instance.MovementSpeed;
            }
            GameManager.Instance.GameOngoing = false;
            playerCount.OnValueChanged += CheckIfGameCanStart;
            DevTools.Instance.SpawnDummies();
        }
    }

    private void Start()
    {
        StartCoroutine(DeactivateLoadingGui());
        voteGui.SetActive(false);
        OnSceneChange.Raise();
        if (IsClient)
        {
            HandleClientJoinLobbyRpc(GameManager.Instance.owner.OwnerClientId);
            PointsManager.Instance.AddPlayerRpc(GameManager.Instance.owner._name.Value.ToString());
        }
        if (IsServer)
        {
            MovePlayers();
            PointsManager.Instance.SubmitPoints();
            foreach (PlayerController player in GameManager.Instance.All)
            {
                int awardedPoints = PointsManager.Instance.GetPoints(player._name.Value.ToString());
                if (awardedPoints == 0) return;
                LogManager.Instance.LogSingleRpc($"You have gained {awardedPoints} points!", LogType.SUCCESS, player.OwnerClientId);
            }
            PointsManager.Instance.ClearPoints();
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void ActivateLoadingGuiRpc()
    {
            loadingGui.SetActive(true);
    }
    
   
    private IEnumerator DeactivateLoadingGui()
    {
        yield return new WaitForSeconds(1.5f);
        loadingGui.SetActive(false);
        lobbyGui.SetActive(true);
    }

    public void StartGame()
    {
        ActivateLoadingGuiRpc();
        if (!IsServer) return;
        GetComponent<LobbyRoleManager>().AssignRolesToPlayers();
        GameManager.Instance.selectedTerrain.Value = GetComponent<LobbyVoteManager>().GetSelectedTerrain();
        StartCoroutine(ChangeGameScene());
    }

    private IEnumerator ChangeGameScene()
    {
        yield return new WaitForSeconds(0.5f);
        NetworkManager.Singleton.SceneManager.LoadScene(Constants.Instance.GameScene, LoadSceneMode.Single);
    }

    private void MovePlayers()
    {
        foreach (PlayerController p in GameManager.Instance.All)
        {
            p.transform.position = GetNextSpawnPoint();
        }
    }

    private Vector3 GetNextSpawnPoint()
    {
        Vector3 vector = spawnPoints[spawnIndex].position;
        spawnIndex = (spawnIndex + 1) % spawnPoints.Count;
        return vector;
    }

    [Rpc(SendTo.Server)]
    public void HandleClientJoinLobbyRpc(ulong uid)
    {
        connectedClients.Add(uid);
        playerCount.Value++;
        PlayerController playerController = GameManager.Instance.All.Find(x => x.OwnerClientId == uid);
        if (playerController == null) return;
        playerController.transform.position = GetNextSpawnPoint();
    }

    public void HandleClientDisconnect(ulong clientId)
    {
        if (!connectedClients.Contains(clientId)) return;
        connectedClients.Remove(clientId);
        playerCount.Value--;
        PlayerController player = GameManager.Instance.GetDisconnectedPlayer(clientId);
        if (player == null) return;
        switch (player.GetComponent<RoleController>().role.Value)
        {
            case Role.WANTS_HIDER:
                hideCount.Value--;
                break;
            case Role.WANTS_HUNTER:
                huntCount.Value--;
                break;
            case Role.WANTS_RANDOM:
            case Role.HIDER:
            case Role.HUNTER:
            case Role.SPECTATOR:
            default:
                break;
        }

        player.GetComponent<NetworkObject>().Despawn();
        Destroy(player.gameObject);
        if (!countdownJoining.IsRunning) return;
        if (CheckIfGameCanStart(playerCount.Value)) return;
        countdownJoining.Abort();
    }

    private void UpdateHiderLabel(int _, int newValue)
    {
        hideLabel.text = newValue + " / " + playerCount.Value;
    }

    private void UpdateHunterLabel(int _, int newValue)
    {
        huntLabel.text = newValue + " / " + playerCount.Value;
    }

    private void UpdateRoleLabels(int _, int __)
    {
        UpdateHiderLabel(0, hideCount.Value);
        UpdateHunterLabel(0, huntCount.Value);
    }

    private void CheckIfGameCanStart(int _, int newPlayerCount)
    {
        if (DevTools.Instance.SinglePlayer)
        {
            countdownJoining.StartCountdown(DevTools.Instance.SkipLobby);
            return;
        }

        if (!CheckIfGameCanStart(newPlayerCount)) return;
        countdownJoining.StartCountdown(DevTools.Instance.SkipLobby);
    }

    private bool CheckIfGameCanStart(int count)
    {
        if (count < minPlayers) return false;
        if (!IsServer) return false;
        return true;
    }

    [Rpc(SendTo.Everyone)]
    public void StartVoteCountdownRpc()
    {
        GameManager.Instance.GameOngoing = true;
        lobbyGui.SetActive(false);
        voteGui.SetActive(true);
        if (countdownVoting.IsRunning) return;
        countdownVoting.StartCountdown(DevTools.Instance.SkipVote);
    }
}
