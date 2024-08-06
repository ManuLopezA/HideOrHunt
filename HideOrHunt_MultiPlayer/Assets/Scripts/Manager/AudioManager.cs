using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct AudioThing
{
    public string name;
    public AudioClip audio;
}

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource audioSource;
    [SerializeField] private GameObject AudioPlayer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
        audioSource = GetComponent<AudioSource>();
    }

    [SerializeField] private AudioThing[] audios;

    [Rpc(SendTo.ClientsAndHost)]
    public void PlaySoundRpc(string name, Vector3 position)
    {
        AudioClip audio = null;
        foreach (var a in audios)
        {
            if (a.name != name) continue;
            audio = a.audio;
            break;
        }
        if (audio == null) return;

        GameObject ap = Instantiate(AudioPlayer);
        ap.GetComponent<AudioPlayer>().StartPlay(audio);
    }

}
