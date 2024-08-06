using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUIMissionManager))]
public class GUIManager : NetworkBehaviour
{
    [SerializeField] private Image myPicture;
    [SerializeField] private Image myLifeSlice;
    [SerializeField] private Countdown hidingCountdown;
    [SerializeField] private Countdown matchCountdown;
    [SerializeField] private Countdown restultsCountdown;
    [SerializeField] public GameObject missionGui;
    [SerializeField] private GameObject guiMatch;
    [SerializeField] private GameObject guiHiding;
    [SerializeField] private GameObject guiResult;
    [SerializeField] private Image hidingBG;
    [SerializeField] private Image resultsBG;
    


    public GameObject hidersLifeGrid;
    public GameObject huntersLifeGrid;
    public InfoBoxDetailController ownInfo;
    public GameObject rewardList;
    public static GUIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        matchCountdown.gameObject.SetActive(false);
    }

    public void CreatePlayersGUI()
    {
        foreach (PlayerController player in MatchManager.Instance.all)
        {
            player.GetComponent<PlayerGUIController>().CreateClientInfoBoxRpc();
        }
    }

    public void DestroyPlayersGUI()
    {
        foreach (PlayerController player in MatchManager.Instance.all)
        {
            player.GetComponent<PlayerGUIController>().DestroyClientInfoBoxRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ActivateMissionGUIRpc()
    {
        if (GameManager.Instance.owner.GetComponent<RoleController>().role.Value == Role.HIDER)
        {
            GUIMissionManager.Instance.imageGrande.SetActive(true);
            GUIMissionManager.Instance.missionGui.SetActive(true);
            GUIMissionManager.Instance.missionImage.SetActive(true);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void StartMatchCountdownRpc()
    {
        matchCountdown.gameObject.SetActive(true);
        matchCountdown.StartCountdown(MatchManager.Instance.endMatchNow || DevTools.Instance.SkipGame);
    }

    [Rpc(SendTo.Everyone)]
    public void DisableMatchCountdownRpc()
    {
        matchCountdown.gameObject.SetActive(false);
    }

    [Rpc(SendTo.Everyone)]
    public void StartHidingCountdownRpc()
    {
        if (GameManager.Instance.owner.GetComponent<RoleController>().role.Value == Role.HUNTER)
        {
            hidingBG.enabled = true;
        }
        resultsBG.enabled = false;
        restultsCountdown.gameObject.SetActive(false);
        hidingCountdown.gameObject.SetActive(true);
        hidingCountdown.StartCountdown(MatchManager.Instance.endMatchNow || DevTools.Instance.SkipHide);
    }

    [Rpc(SendTo.Everyone)]
    public void StartResultCountdownRpc()
    {
        MatchManager.Instance.StopGameMusic();
        guiMatch.SetActive(false);
        resultsBG.enabled = true;
        restultsCountdown.gameObject.SetActive(true);
        restultsCountdown.StartCountdown(DevTools.Instance.SkipResults);
        if (!IsServer) return;
        guiResult.GetComponent<GUIResultManager>().SetWinningTeamRpc(MatchManager.Instance.winnerTeam);
    }

    /* private IEnumerator SendPoints() */
    /* { */
    /*     yield return new WaitForEndOfFrame(); */
    /*     guiResult.GetComponent<GUIResultManager>().SetWinningTeamRpc(MatchManager.Instance.winnerTeam); */
    /*     foreach (PlayerController player in GameManager.Instance.All) */
    /*     { */
    /*         var points = PointsManager.Instance.GetPoints(player._name.Value.ToString()); */
    /*         guiResult.GetComponent<GUIResultManager>().SetScoreRpc(player._name.Value.ToString(), points); */
    /*     } */
    /* } */

    [Rpc(SendTo.Everyone)]
    public void DisableHidingCountdownRpc()
    {
        print("Disable hidig countdown rpc");
        hidingCountdown.gameObject.SetActive(false);
        hidingBG.enabled = false;
    }
}
