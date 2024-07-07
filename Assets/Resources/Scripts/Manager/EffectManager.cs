using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum EffectType
{ 
    SnowHit = 0,
    Explosion,
    StoneHit,
    StarAura,

    End,
}


public class EffectManager : MonoBehaviour
{
    [SerializeField] private GameObject snowHit = null;
    [SerializeField] private GameObject explosion = null;
    [SerializeField] private GameObject stoneHit = null;
    [SerializeField] private GameObject starAura = null;

    public void Play(EffectType type, Vector3 pos)
    {
        GameObject effect = null;
        switch (type)
        {
            case EffectType.SnowHit:
                effect = snowHit;
                break;
            case EffectType.Explosion:
                effect = explosion;
                break;
            case EffectType.StoneHit:
                effect = stoneHit;
                break;
            case EffectType.StarAura:
                effect = starAura;
                break;
            default:
                break;
        }

        pos.y += 0.5f;
        effect.transform.position = pos;

        effect.gameObject.SetActive(true);
        effect.GetComponent<ParticleSystem>().Play();
    }

    public void AttachEffect(GameObject target, EffectType type)
    {
        GameObject effect = null;
        switch (type)
        {
            case EffectType.SnowHit:
                effect = snowHit;
                break;
            case EffectType.Explosion:
                effect = explosion;
                break;
            case EffectType.StoneHit:
                effect = stoneHit;
                break;
            case EffectType.StarAura:
                effect = starAura;
                break;
            default:
                break;
        }

        effect.transform.position = target.transform.position + Vector3.up * 1.2f;
        effect.transform.SetParent(target.transform);

        //pos.y = 1.2f;
        effect.gameObject.SetActive(true);
        effect.GetComponent<ParticleSystem>().Play();

        StartCoroutine(CoBackPos(effect.GetComponent<ParticleSystem>().main.duration + 0.2f));
    }

    private IEnumerator CoBackPos(float time)
    {
        yield return new WaitForSeconds(time);

        starAura.transform.SetParent(transform);
    }
}
