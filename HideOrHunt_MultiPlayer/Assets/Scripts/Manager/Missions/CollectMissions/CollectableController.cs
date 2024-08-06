using System;
using Unity.Netcode;
using UnityEngine;

public class CollectableController : NetworkBehaviour
{
    public MissionCompleterCollect player;
    [SerializeField] private GameObject sprite;
    [SerializeField] private AudioClip audioClip;

    private bool done;

    public void SetPlayer(MissionCompleterCollect player)
    {
        this.player = player;
        if (player.GetComponent<NetworkObject>().OwnerClientId == GameManager.Instance.owner.GetComponent<NetworkObject>().OwnerClientId)
        {
            sprite.SetActive(true);
        }
        else
        {
            sprite.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (done) return;
        if (!other.TryGetComponent(out MissionCompleterCollect otherPlayer)) return;
        if (otherPlayer != player) return;
        done = true;
        player.UpdateProgress();
        player.GetComponent<AudioController>().CollectAudioRpc(GetComponent<Transform>().tag);
        try
        {
            DisableSpriteRpc();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DisableSpriteRpc()
    {
        sprite.SetActive(false);
    }

    public void KMS()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

}
