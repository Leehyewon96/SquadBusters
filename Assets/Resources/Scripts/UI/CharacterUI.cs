using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    protected Image hpBar = null;

    protected virtual void Awake()
    {
        hpBar = GetComponent<Image>();
    }
}
