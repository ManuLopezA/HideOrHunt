using Unity.Netcode;
using UnityEngine;

public class DevTools : NetworkBehaviour
{
    [SerializeField] private GameObject dummyPrefab;
    [SerializeField] private Role role;
    [SerializeField] private int dummyCount;
    [SerializeField] private bool spawnDummies;
    [SerializeField] private bool singlePlayer;
    [SerializeField] private bool skipLobby;
    [SerializeField] private bool skipVote;
    [SerializeField] private bool skipHide;
    [SerializeField] private bool skipGame;
    [SerializeField] private bool skipResults;

    public Role Role => role;
    public bool SinglePlayer => singlePlayer;
    public bool SkipLobby => skipLobby;
    public bool SkipVote => skipVote;
    public bool SkipHide => skipHide;
    public bool SkipGame => skipGame;
    public bool SkipResults => skipResults;

    public static DevTools Instance { get; private set; }

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

    private bool done;

    public void SpawnDummies()
    {
        if (!IsServer) return;
        if (!spawnDummies) return;
        if (done) return;
        done = true;
        for (int i = 0; i < dummyCount; i++)
        {
            GameObject player = Instantiate(dummyPrefab);
            player.GetComponent<PlayerController>().isDummy = true;
            player.GetComponent<NetworkObject>().Spawn();
            player.name = $"Dummy {i + 1}";
            player.GetComponent<PlayerController>().SetNameRpc(player.name);
            player.transform.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        }
    }
}
