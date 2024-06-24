using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private GameObject snowHit = null;
    [SerializeField] private GameObject explosion = null;
    [SerializeField] private GameObject stoneHit = null;

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

    public void Explosion(Vector3 pos)
    {
        explosion.transform.position = pos;
        pos.y = 1.2f;
        explosion.gameObject.SetActive(true);
        explosion.GetComponent<ParticleSystem>().Play();
    }

    public void StoneHit(Vector3 pos)
    {
        stoneHit.transform.position = pos;
        pos.y = 1.2f;
        stoneHit.gameObject.SetActive(true);
        stoneHit.GetComponent<ParticleSystem>().Play();
    }
}
