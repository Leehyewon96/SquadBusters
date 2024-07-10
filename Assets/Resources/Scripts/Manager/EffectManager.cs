using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EffectType
{ 
    SnowHit = 0,
    Explosion,
    StoneHit,
    StarAura,
    StoneSlash,
    MagicCircle2,
    PortalRed,


    End,
}


public class EffectManager : MonoBehaviour
{
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

}
