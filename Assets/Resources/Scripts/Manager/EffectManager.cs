using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private GameObject snowHit = null;

    private void Awake()
    {
        
    }

    public void SnowHit(Vector3 pos)
    {
        snowHit.transform.position = pos;
        pos.y = 1.2f;
        snowHit.gameObject.SetActive(true);
        snowHit.GetComponent<ParticleSystem>().Play();
    }
}
