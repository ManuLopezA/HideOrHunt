using Unity.Netcode;
using UnityEngine;

public class CounterArea : NetworkBehaviour
{
    [SerializeField] private GameEvent enterEvent;
    [SerializeField] private GameEvent exitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        enterEvent.Raise();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        exitEvent.Raise();
    }
}
