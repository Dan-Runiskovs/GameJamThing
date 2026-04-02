using UnityEngine;

public class playCrateSound : MonoBehaviour
{
    private AudioPlayer audioPlayer;

    [SerializeField] private AudioClip[] CrateSounds;

    void Start()
    {
        audioPlayer = GetComponent<AudioPlayer>();

        if (audioPlayer == null)
        {
            Debug.LogError("AudioPlayer component missing on " + gameObject.name);
            return;
        }

        if (CrateSounds == null || CrateSounds.Length == 0)
        {
            Debug.LogWarning("No crate sounds assigned on " + gameObject.name);
            return;
        }

        PlayAudio();
    }

    void PlayAudio()
    {
        if (audioPlayer == null)
        {
            Debug.LogError("AudioPlayer is null, cannot play sound.");
            return;
        }

        if (CrateSounds == null || CrateSounds.Length == 0)
        {
            Debug.LogWarning("CrateSounds array is empty.");
            return;
        }

        audioPlayer.RandomSoundEffect(CrateSounds);
    }
}