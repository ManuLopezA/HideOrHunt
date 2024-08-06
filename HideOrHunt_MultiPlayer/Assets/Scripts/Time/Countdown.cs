using System.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Countdown : NetworkBehaviour
{
    [SerializeField] private string title;

    public bool IsRunning => coroutine != null;

    public NetworkVariable<int> secLeft = new();

    private Coroutine coroutine;

    [SerializeField] private int secStart;
    [SerializeField] private int secWarn;
    [SerializeField] private int secDanger;

    [SerializeField] private GameEvent OnCountdownEnd;
    [SerializeField] private TextMeshProUGUI timerDisplay;

    [SerializeField] private Color neutral;
    [SerializeField] private Color success;
    [SerializeField] private Color danger;
    [SerializeField] private Color warning;

    public override void OnNetworkSpawn()
    {
        secLeft.OnValueChanged = UpdateGUI;
    }

    public override void OnDestroy()
    {
        if (coroutine == null) return;
        StopCoroutine(coroutine);
    }

    public void StartCountdown(bool skip)
    {
        if (!IsServer) return;
        if (IsRunning) return;
        if (skip)
        {
            OnCountdownEnd.Raise();
            return;
        }

        secLeft.Value = secStart;
        coroutine = StartCoroutine(Count());
    }

    public void Abort()
    {
        if (!IsServer) return;
        if (!IsRunning) return;
        if (coroutine == null) return;
        StopCoroutine(coroutine);
        coroutine = null;
        secLeft.Value = secStart;
    }

    private void UpdateGUI(int prev, int actual)
    {
        int min = actual / 60;
        int sec = actual % 60;
        // if (sec < secDanger)
        // {
        //     timerDisplay.color = danger;
        // }
        // else if (sec < secWarn)
        // {
        //     timerDisplay.color = warning;
        // }
        timerDisplay.text = $"{min:00}:{sec:00}";
    }

    private IEnumerator Count()
    {
        while (secLeft.Value > 0)
        {
            yield return new WaitForSeconds(1);
            secLeft.Value--;
        }

        if (IsServer) OnCountdownEnd.Raise();
    }
}
