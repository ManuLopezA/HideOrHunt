using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MissionCompleterCollect : MonoBehaviour, MissionCompleterInterface<SOMissionCollect>
{
    private PropType target;
    public List<CollectableController> coins = new();
    public SOMissionCollect mission { get; private set; }

    public void Init(SOMissionCollect newMission)
    {
        mission = newMission;
        GetComponent<PlayerMissionCountdown>().currProgress.Value = 0;
        GetComponent<PlayerMissionCountdown>().StartProgressRpc(mission.progress);
        ulong clientId = GetComponent<NetworkObject>().OwnerClientId;
        foreach (var position in mission.positions)
        {
            GameObject coin = Instantiate((mission as SOMissionCollect).prefabToCollect);
            coin.transform.position = new Vector3(position.x, 0.5f, position.y);
            coin.GetComponent<CollectableController>().SetPlayer(this);
            coin.GetComponent<NetworkObject>().Spawn();
            coin.GetComponent<NetworkObject>().NetworkShow(clientId);
            coins.Add(coin.GetComponent<CollectableController>());
        }
    }

    public void UpdateProgress()
    {
        if (mission == null) return;
        GetComponent<PlayerMissionCountdown>().currProgress.Value++;
        Debug.Log($"MissionCOmpleterCollect: progress ->{GetComponent<PlayerMissionCountdown>().currProgress.Value}/{mission.progress}");
        if (GetComponent<PlayerMissionCountdown>().currProgress.Value < mission.progress) return;
        // GUIMissionManager.Instance.UpdateMissionProgress(progress);
        GetComponent<PlayerMissionController>().MissionCompleted();
        FinishMission();
    }

    public void FinishMission()
    {
        mission = default;
        target = default;
        GetComponent<PlayerMissionCountdown>().currProgress.Value = default;
        foreach (var coin in coins)
        {
            coin.GetComponent<CollectableController>().KMS();
        }

        coins.Clear();
    }
}
