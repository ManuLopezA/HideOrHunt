using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    public void StartPlay(AudioClip audio)
    {
        var source = GetComponent<AudioSource>();
        source.PlayOneShot(audio);
        KMS(audio.length);
    }

    private IEnumerator KMS(float sec)
    {
        yield return new WaitForSeconds(sec);
        Destroy(gameObject);
    }
}
