using UnityEngine;
using Unity.Netcode;

public class LogManager : NetworkBehaviour

{
    public static LogManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    [SerializeField] private GameObject MessageList;
    [SerializeField] private GameObject MessagePrefab;

    [Rpc(SendTo.ClientsAndHost)]
    public void LogSingleRpc(string msg, LogType type, ulong uid)
    {
        if (GameManager.Instance.owner.OwnerClientId != uid) return;
        SetMessage(msg, type);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void LogAllRpc(string msg, LogType type)
    {
        SetMessage(msg, type);
    }

    private void SetMessage(string msg, LogType type)
    {
        GameObject prefab = Instantiate(MessagePrefab, MessageList.transform);
        prefab.GetComponent<MessageController>().SetMessage(msg, type);
    }

}
