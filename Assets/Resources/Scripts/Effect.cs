using UnityEngine;

public class Effect : MonoBehaviour
{
    public EffectType effectType;
    private ParticleSystem particle = null;

    private void Awake()
    {
        Debug.Log("파티클 초기화");
        particle = GetComponent<ParticleSystem>();
        
    }

    public bool GetIsPlaying()
    {
        if(particle == null)
        {
            Debug.Log("파티클 없음");
        }
        return particle.isPlaying;
    }

    public void Play()
    {
        particle.Play();
    }
}
