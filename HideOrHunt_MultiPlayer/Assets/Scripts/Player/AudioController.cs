using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioController : NetworkBehaviour
{
    [Header("Players sounds")]
    public AudioClip receiveDamageClip;
    public AudioClip dieClip;
    public AudioClip shootClip;
    
    [Header("Collectables Clips")]
    public AudioClip coinAudioClip;
    public AudioClip butterflyAudioClip;
    public AudioClip diamondAudioClip;

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayAudioRpc(int index)
    {
        if (GetComponent<AudioSource>() == null) return;
        print("PlayAudioRpc");
        switch (index)
        {
            case 0:
                GetComponent<AudioSource>().PlayOneShot(receiveDamageClip);
                break;
            case 1:
                GetComponent<AudioSource>().PlayOneShot(dieClip);
                break;
            case 2:
                GetComponent<AudioSource>().PlayOneShot(shootClip);
                break;
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    public void CollectAudioRpc(string collectableTag)
    {
        if (!IsOwner) return;
        switch (collectableTag)
        {
            case "Butterfly":
                GetComponent<AudioSource>().PlayOneShot(butterflyAudioClip);
                break;
            case "Coin":
                GetComponent<AudioSource>().PlayOneShot(coinAudioClip);
                break;
            case "Diamond":
                GetComponent<AudioSource>().PlayOneShot(diamondAudioClip);
                break;
        }
    }
}
