using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    
    List<Effect> effects = new List<Effect>();

    private void Awake()
    {
        effects = GetComponentsInChildren<Effect>(true).ToList();
    }

    public void Play(EffectType type, Vector3 pos)
    {
        Effect effect = effects.Find(e => e.effectType.Equals(type) && !e.GetIsPlaying());
        if(effect == null)
        {
            Effect origin = effects.Find(e => e.effectType.Equals(type));
            effect = Instantiate(origin, transform);
            effects.Add(effect);
        }

        //pos.y += 0.5f;
        effect.gameObject.transform.position = pos;

        effect.gameObject.SetActive(true);
        effect.Play();
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
