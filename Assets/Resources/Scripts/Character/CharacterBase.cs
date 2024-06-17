using UnityEditor.Animations;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected Animator animator = null;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }
}
