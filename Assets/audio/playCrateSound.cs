using UnityEngine;

public class playCrateSound : MonoBehaviour
{
    AudioPlayer audioPlayer;
    [SerializeField] AudioClip[] CrateSounds;
    void Start()
    {
        audioPlayer = GetComponent<AudioPlayer>();
        playAudio();
    }

    void playAudio()
        {
        audioPlayer.RandomSoundEffect(CrateSounds);
    }
}
