using System.Collections.Generic;
using Menu;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetManager : MonoBehaviour
{
    public NetType netType;
    public MenuManager menuManager;
    public ushort port = 7780;

    [Header("Server Settings")] public int maxPlayers;
    [SerializeField] private GameEventULong onClientConnected;
    [SerializeField] private GameEventULong onClientDisconnected;

    [Header("Client Settings")] public string ip = "localhost";
    public int nm_characterSelected;

    public static NetManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
    }

    private bool CanConnect()
    {
        return true;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        if (GameManager.Instance.GameOngoing)
        {
            response.Approved = false;
            response.CreatePlayerObject = false;
            response.Reason = "The Game is ongoing";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClients.Count > maxPlayers)
        {
            response.Approved = false;
            response.CreatePlayerObject = false;
            response.Reason = "The Game is full";
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = true;
    }

    public void StartClient()
    {
        if (menuManager.ipHost)
        {
            string newIp = menuManager.ipAddress;
            Debug.Log($"Connectiong to {newIp}");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                newIp,
                port
            );
        }

        nm_characterSelected = menuManager.characterSelected;
        NetworkManager.Singleton.StartClient();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        StartHostAndServer();
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        StartHostAndServer();
    }

    private void StartHostAndServer()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        maxPlayers = menuManager.maxPlayers;
        nm_characterSelected = menuManager.characterSelected;
        NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }

    private void OnClientConnected(ulong uid)
    {
        Debug.Log("SERVER: New client connected id: " + uid);
        LogManager.Instance.LogAllRpc($"client {uid} has connected", LogType.INFO);
        onClientConnected.Raise(uid);
    }

    private void OnClientDisconnected(ulong uid)
    {
        Debug.Log("SERVER: Client disconnected id: " + uid);
        LogManager.Instance.LogAllRpc($"client {uid} has disconnected", LogType.WARNING);
        onClientDisconnected.Raise(uid);
    }

    public List<PlayerController> GetPlayers()
    {
        List<PlayerController> players = new();
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            players.Add(player.PlayerObject.GetComponent<PlayerController>());
        }

        return players;
    }
}