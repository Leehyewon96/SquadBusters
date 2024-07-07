using UnityEngine;

public class Effect : MonoBehaviour
{
    public EffectType effectType;
    private ParticleSystem particle = null;

    private void Awake()
    {
        Debug.Log("��ƼŬ �ʱ�ȭ");
        particle = GetComponent<ParticleSystem>();
        
    }

    public bool GetIsPlaying()
    {
        if(particle == null)
        {
            Debug.Log("��ƼŬ ����");
        }
        return particle.isPlaying;
    }

    public void Play()
    {
        particle.Play();
    }
}
