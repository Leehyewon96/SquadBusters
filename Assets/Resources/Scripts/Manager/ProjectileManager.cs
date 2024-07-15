using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private List<FireBullet> bullets = new List<FireBullet>();

    private void Awake()
    {
        bullets = GetComponentsInChildren<FireBullet>().ToList();
        bullets.ForEach(b => b.gameObject.SetActive(false));
    }

    public FireBullet GetBullet(Vector3 pos)
    {
        FireBullet bullet = bullets.Find(b => !b.gameObject.activeSelf);
        if (bullet == null)
        {
            string path = $"Prefabs/Projectile/Firebullet";
            GameObject newBullet = PhotonNetwork.Instantiate(path, pos, Quaternion.identity);
            bullet = newBullet.GetComponent<FireBullet>();
            bullet.transform.SetParent(transform);
            bullets.Add(bullet);
        }

        bullet.transform.position = pos;
        bullet.gameObject.SetActive(true);

        return bullet;
    }
}
