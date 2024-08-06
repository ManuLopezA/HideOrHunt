using Unity.Netcode;
using UnityEngine;

public class MissionCompleterPosition : NetworkBehaviour, MissionCompleterInterface<SOMissionPosition>
{
    public SOMissionPosition mission { get; private set; }
    private Vector3 target;
    public GameObject cameraMission;
    private GameObject finalCamera;

    public void Init(SOMissionPosition mission)
    {
        target = mission.position;
        this.mission = mission;
        SpawnCameraRpc(target, mission.radius);
    }

    public void OnAfterMove(Vector3 currenPosition)
    {
        if (mission == null) return;
        if (Vector3.Distance(currenPosition, target) > mission.radius) return;
        GetComponent<PlayerMissionController>().MissionCompleted();
    }

    public void FinishMission()
    {
        //
        DeactivateCameraRpc();
        mission = default;
        target = default;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DeactivateCameraRpc()
    {
        if (!IsOwner) return;
        GUIMissionManager.Instance.missionCameraImage.SetActive(false);
    }


    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnCameraRpc(Vector3 target, float radius)
    {
        //print("Spawn camera");
        //finalCamera = Instantiate(cameraMission);
        //finalCamera.transform.position = new Vector3(target.x, 8, target.z - mission.radius * 2);
        //finalCamera.SetActive(true);
        if (!IsOwner) return;
        GUIMissionManager.Instance.missionCameraImage.SetActive(true);
        GUIMissionManager.Instance.missionCamera.transform.position = new Vector3(target.x, 8, target.z - radius * 2);
    }

}
