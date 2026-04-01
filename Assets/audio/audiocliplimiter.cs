using System.Collections.Generic;
using UnityEngine;

public class SoundLimiter : MonoBehaviour
{
    public static SoundLimiter Instance;

    public int maxInstances = 4;

    private class PlayingSound
    {
        public AudioSource source;
        public float startTime;
    }

    private Dictionary<AudioClip, List<PlayingSound>> activeSounds = new();

    void Awake()
    {
        Instance = this;
    }

    public void PlaySound(AudioSource source, AudioClip clip, float pitch = 1f)
    {
        if (!activeSounds.ContainsKey(clip))
            activeSounds[clip] = new List<PlayingSound>();

        var list = activeSounds[clip];

        // If we're at limit ? kill oldest
        if (list.Count >= maxInstances)
        {
            PlayingSound oldest = list[0];

            foreach (var s in list)
            {
                if (s.startTime < oldest.startTime)
                    oldest = s;
            }

            if (oldest.source != null)
                oldest.source.Stop();

            list.Remove(oldest);
        }

        // Play new sound
        source.pitch = pitch;
        source.clip = clip;
        source.Play();

        list.Add(new PlayingSound
        {
            source = source,
            startTime = Time.time
        });
    }
}