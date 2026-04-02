using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    // Audio players components.
    public AudioSource EffectsSource;

    // Random pitch adjustment range.
    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    // Play a single clip through the sound effects source.
    public void Play(AudioClip clip)
    {
        if (EffectsSource == null)
        {
            Debug.LogWarning("AudioPlayer: EffectsSource is not assigned.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("AudioPlayer: Tried to play a null clip.");
            return;
        }

        EffectsSource.clip = clip;
        EffectsSource.Play();
    }

    // Play a random clip from an array, and randomize the pitch slightly.
    public void RandomSoundEffect(params AudioClip[] clips)
    {
        if (EffectsSource == null)
        {
            Debug.LogWarning("AudioPlayer: EffectsSource is not assigned.");
            return;
        }

        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("AudioPlayer: No clips provided.");
            return;
        }

        int randomIndex = Random.Range(0, clips.Length);
        AudioClip chosenClip = clips[randomIndex];

        if (chosenClip == null)
        {
            Debug.LogWarning("AudioPlayer: Selected clip is null.");
            return;
        }

        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        EffectsSource.pitch = randomPitch;
        EffectsSource.clip = chosenClip;
        EffectsSource.Play();
    }
}