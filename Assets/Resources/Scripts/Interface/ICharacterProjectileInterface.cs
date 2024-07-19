using UnityEngine;

public interface ICharacterProjectileInterface
{
    public void Stun(float duration, string animName);
    public void GetAOE(float inDamage, Vector3 fromPos, float distance);
}
