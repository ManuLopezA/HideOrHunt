using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerRewardController))]
[RequireComponent(typeof(PlayerMissionCountdown))]
[RequireComponent(typeof(MissionCompleterTransform))]
[RequireComponent(typeof(MissionCompleterPosition))]
[RequireComponent(typeof(MissionCompleterCollect))]
public class PlayerMissionController : NetworkBehaviour
{
    private PlayerController playerController;
    private PlayerRewardController playerRewardController;
    private PlayerMissionCountdown playerMissionCountdown;
    private MissionCompleterTransform missionCompleterTransform;
    private MissionCompleterPosition missionCompleterPosition;
    private MissionCompleterCollect missionCompleterCollect;

    [SerializeField] private SOMission currentMission;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerRewardController = GetComponent<PlayerRewardController>();
        playerMissionCountdown = GetComponent<PlayerMissionCountdown>();
        missionCompleterTransform = GetComponent<MissionCompleterTransform>();
        missionCompleterPosition = GetComponent<MissionCompleterPosition>();
        missionCompleterCollect = GetComponent<MissionCompleterCollect>();
    }

    public void InitMissions(int timeSec)
    {
        if (playerController.isDummy) return;
        playerMissionCountdown.OnEndMissionCounter = AskForNewMission;
        playerMissionCountdown.StartCountdownRpc(timeSec);
        playerMissionCountdown.SetMissionDataRpc(0);
    }

    public void EndMissions()
    {
        if (playerController.isDummy) return;
        playerMissionCountdown.EndMissionCounterRpc();
        missionCompleterCollect.FinishMission();
        missionCompleterPosition.FinishMission();
        missionCompleterTransform.FinishMission();
        currentMission = default;
    }

    private void AskForNewMission()
    {
        if (GetComponent<HealthController>().isDead) return;
        var (newMission, index) = MissionManager.Instance.GetNewMission();
        AssignMission(newMission, index);
    }

    private void AssignMission(SOMission mission, int index)
    {
        currentMission = mission;
        switch (mission)
        {
            case SOMissionTransform soMissionTransform:
                missionCompleterTransform.Init(soMissionTransform);
                break;
            case SOMissionPosition soMissionPosition:
                missionCompleterPosition.Init(soMissionPosition);
                break;
            case SOMissionCollect soMissionCollect:
                missionCompleterCollect.Init(soMissionCollect);
                break;
        }

        playerMissionCountdown.OnEndMissionCounter = MissionFailed;
        playerMissionCountdown.StartCountdownRpc(currentMission.availableTimeSec);
        playerMissionCountdown.SetMissionDataRpc(index);
    }

    private void MissionFailed()
    {
        if (playerController.isDummy) return;
        playerRewardController.AskForCooldown();
        playerMissionCountdown.EndMissionCounterRpc();
        int waitTimeSec = currentMission.waitTimeSec;
        EndMissions();
        InitMissions(waitTimeSec);
    }

    public void MissionCompleted()
    {
        if (playerController.isDummy) return;
        PointsManager.Instance.CompleteMission(OwnerClientId, playerController._name.Value.ToString(), currentMission.difficulty);
        playerRewardController.AskForPowerup();
        playerMissionCountdown.EndMissionCounterRpc();
        int waitTimeSec = currentMission.waitTimeSec;
        EndMissions();
        InitMissions(waitTimeSec);
    }
}
