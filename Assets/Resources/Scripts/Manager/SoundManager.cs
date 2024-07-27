using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AudioSourceType
{
    BG,
    Shot,
    Walk,

    End,
}


public class SoundManager : MonoBehaviour
{
    private List<AudioSource> audioSources = new List<AudioSource>();

    private void Awake()
    {
        audioSources = GetComponentsInChildren<AudioSource>(true).ToList();
    }


}
