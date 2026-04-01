using System.Collections.Generic;
using UnityEngine;

public class SoundLimiter : MonoBehaviour
{
    public static SoundLimiter Instance;

    public int maxInstances = 3;

    private class PlayingSound
    {
        public AudioSource source;
        public float startTime;
    }

    private Dictionary<string, List<PlayingSound>> activeGroups = new();

    void Awake()
    {
        Instance = this;
    }

    public void PlaySound(string group, AudioSource source, AudioClip clip, float pitch = 1f)
    {
        if (!activeGroups.ContainsKey(group))
            activeGroups[group] = new List<PlayingSound>();

        var list = activeGroups[group];

        // Clean dead entries (IMPORTANT)
        list.RemoveAll(s => s.source == null || !s.source.isPlaying);

        // If at limit → kill oldest
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

        // Play new
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