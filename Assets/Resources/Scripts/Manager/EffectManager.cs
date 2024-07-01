using System.Collections;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private GameObject snowHit = null;
    [SerializeField] private GameObject explosion = null;
    [SerializeField] private GameObject stoneHit = null;
    [SerializeField] private GameObject starAura = null;

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

    public void StarAura(Vector3 pos)
    {
        starAura.transform.position = pos;
        pos.y = 1.2f;
        starAura.gameObject.SetActive(true);
        starAura.GetComponent<ParticleSystem>().Play();
    }

    public void AttachStarAura(GameObject target)
    {
        starAura.transform.position = target.transform.position + Vector3.up * 1.2f;
        starAura.transform.SetParent(target.transform);
        
        //pos.y = 1.2f;
        starAura.gameObject.SetActive(true);
        starAura.GetComponent<ParticleSystem>().Play();

        StartCoroutine(CoBackPos(starAura.GetComponent<ParticleSystem>().main.duration + 0.2f));
    }

    private IEnumerator CoBackPos(float time)
    {
        yield return new WaitForSeconds(time);

        starAura.transform.SetParent(transform);
    }
}
