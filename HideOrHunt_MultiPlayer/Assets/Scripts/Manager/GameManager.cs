using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public NetworkVariable<Terrain> selectedTerrain;
    public Difficulty difficulty;
    public List<PlayerController> All = new();
    public PlayerController owner;

    public Sprite[] characterSprites;
    public string password;
    public string nickname;
    public string db_password;
    public int gm_selectedCharacter;

    public bool GameOngoing;
    public static GameManager Instance { get; private set; }

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

    public void ChangeNickName(string value)
    {
        nickname = value;
    }

    public void ChangePassword(string value)
    {
        password = value;
    }

    public void ChangeDbPassword(string value)
    {
        db_password = value;
    }

    public PlayerController GetDisconnectedPlayer(ulong uid)
    {
        PlayerController player = All.Find(p => p.GetComponent<NetworkObject>().OwnerClientId == uid);
        if (player == null) return null;
        All.Remove(player);
        return player;
    }
}
