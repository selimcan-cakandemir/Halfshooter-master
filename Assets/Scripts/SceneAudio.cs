using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class SceneAudio : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] audioClips;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
        }
    }
}
