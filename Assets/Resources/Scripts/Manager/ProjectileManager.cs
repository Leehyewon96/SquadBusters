using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private List<FireBullet> bullets = new List<FireBullet>();

    public FireBullet GetBullet(Vector3 pos)
    {
        FireBullet bullet = null;
        if(bullets.Count > 0)
        {
            bullet = bullets.Find(b => !b.gameObject.activeSelf);
        }
        
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
