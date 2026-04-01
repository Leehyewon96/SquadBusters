using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    private AudioSource audioSource = null;
    [SerializeField] private SoundEffectType soundEffectType;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public SoundEffectType GetSoundEffectType() => soundEffectType;

    public void Play() => audioSource.Play();

    public void Stop() => audioSource.Stop();

    public void UpdateVolume(float newVolume) => audioSource.volume = newVolume;

    public bool IsPlaying() => audioSource.isPlaying;

    public bool IsLoop() => audioSource.loop;
}
