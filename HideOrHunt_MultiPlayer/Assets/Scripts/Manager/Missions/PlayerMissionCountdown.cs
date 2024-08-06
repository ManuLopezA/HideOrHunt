using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerMissionCountdown : NetworkBehaviour
{
    public NetworkVariable<int> secLeft = new();
    public NetworkVariable<int> maxProgress = new();
    public NetworkVariable<int> currProgress = new();
    public Action OnEndMissionCounter;

    private Coroutine counter;

    private bool IsRunning => secLeft.Value > 0;

    [Rpc(SendTo.ClientsAndHost)]
    public void SetMissionDataRpc(int index)
    {
        if (!IsOwner) return;
        SOMission mission = MissionManager.Instance.GetMission(index);
        secLeft.OnValueChanged = GUIMissionManager.Instance.UpdateMissionTimer;
        GUIMissionManager.Instance.NewMissionData(mission.title, mission.description, mission.image, mission.progress);
    }

    [Rpc(SendTo.Everyone)]
    public void StartCountdownRpc(int sec)
    {
        if (GetComponent<PlayerController>().isDummy) return;
        if (IsRunning) return;
        if (IsOwner) secLeft.OnValueChanged = GUIMissionManager.Instance.UpdateMissionTimer;
        if (!IsServer) return;
        EndMissionCounterRpc();
        // the server starts the coroutine to which the client is subscribed
        secLeft.Value = sec;
        counter = StartCoroutine(Count());
    }

    [Rpc(SendTo.Everyone)]
    public void StartProgressRpc(int prog)
    {
        if (GetComponent<PlayerController>().isDummy) return;
        if (IsServer) maxProgress.Value = prog;
        if (IsOwner) currProgress.OnValueChanged += GUIMissionManager.Instance.UpdateMissionProgress;
    }

    [Rpc(SendTo.Everyone)]
    public void EndMissionCounterRpc()
    {
        if (GetComponent<PlayerController>().isDummy) return;
        if (IsServer)
        {
            if (counter != null)
            {
                StopCoroutine(counter);
            }

            secLeft.Value = 0;
        }

        if (IsOwner) secLeft.OnValueChanged -= GUIMissionManager.Instance.UpdateMissionTimer;
    }

    private IEnumerator Count()
    {
        while (secLeft.Value > 0)
        {
            yield return new WaitForSeconds(1);
            secLeft.Value--;
        }

        if (IsServer) OnEndMissionCounter();
    }
}
