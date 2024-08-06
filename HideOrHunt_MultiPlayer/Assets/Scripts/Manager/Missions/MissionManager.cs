using Unity.Netcode;
using UnityEngine;

public class MissionManager : NetworkBehaviour
{
    public GameObject missionCamera;
    public static MissionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField] private SOMission[] missions;
    [SerializeField] private SOReward[] cooldowns;
    [SerializeField] private SOReward[] powerups;

    public (SOMission, int) GetNewMission()
    {
        int index = Random.Range(1, missions.Length);
        return (missions[index], index);
    }

    public (SOReward, int) GetNewCooldown()
    {
        int index = Random.Range(0, cooldowns.Length);
        return (cooldowns[index], index);
    }

    public (SOReward, int) GetNewPowerup()
    {
        int index = Random.Range(0, powerups.Length);
        return (powerups[index], index);
    }

    public void StartMissions()
    {
        foreach (PlayerController hider in MatchManager.Instance.hiders)
        {
            hider.GetComponent<PlayerMissionController>().InitMissions(Random.Range(2, 10));
        }
    }

    public SOMission GetMission(int index)
    {
        return missions[index];
    }

    public SOReward GetPowerup(int index)
    {
        return powerups[index];
    }

    public SOReward GetCooldown(int index)
    {
        return cooldowns[index];
    }
}