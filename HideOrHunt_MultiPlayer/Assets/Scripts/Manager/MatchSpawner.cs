using UnityEngine;

public class MatchSpawner : MonoBehaviour
{
    [SerializeField] private GameObject hiderSpawnParent;
    private Transform[] hiderSpawns;
    private int hiderIndex;
    [SerializeField] private GameObject hunterSpawnParent;
    private Transform[] hunterSpawns;
    private int hunterIndex;

    private void Awake()
    {
        hiderSpawns = hiderSpawnParent.GetComponentsInChildren<Transform>()[1..];
        hunterSpawns = hunterSpawnParent.GetComponentsInChildren<Transform>()[1..];
    }

    public void SpawnPlayer(Role role, PlayerController player)
    {
        player.transform.position = role switch
        {
            Role.HIDER => GetNextHiderSpawn(),
            Role.HUNTER => GetNextHunterSpawn(),
            _ => GetNextHiderSpawn()
        };
    }

    private Vector3 GetNextHiderSpawn()
    {
        hiderIndex = Random.Range(0,hiderSpawns.Length);
        Transform spawn = hiderSpawns[hiderIndex];
        return spawn.position;
    }
    private Vector3 GetNextHunterSpawn()
    {
        hunterIndex = Random.Range(0,hunterSpawns.Length);
        Transform spawn = hunterSpawns[hunterIndex];
        // hunterSpawns =
        return spawn.position;
    }
    // private Vector3 GetNextHiderSpawn()
    // {
    //     Transform spawn = hiderSpawns[hiderIndex];
    //     hiderIndex = (hiderIndex + 1) % hiderSpawns.Length;
    //     return spawn.position;
    // }


    // private Vector3 GetNextHunterSpawn()
    // {
    //     Transform spawn = hunterSpawns[hunterIndex];
    //     hunterIndex = (hunterIndex + 1) % hunterSpawns.Length;
    //     return spawn.position;
    // }
}