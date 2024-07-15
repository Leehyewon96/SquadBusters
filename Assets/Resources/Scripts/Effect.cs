using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Effect : MonoBehaviour
{
    public EffectType effectType;
    private ParticleSystem particle = null;
    //private PhotonView photonView = null;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        //photonView = GetComponent<PhotonView>();    
    }

    public bool GetIsPlaying()
    {
        return particle.isPlaying;
    }

    public void Play()
    {
        particle.Play();
        //photonView.RPC("RPCPlay", RpcTarget.AllBuffered);
    }

    //[PunRPC]
    //public void RPCPlay()
    //{
    //    //gameObject.transform.position = pos;
    //    //gameObject.transform.rotation = Quaternion.LookRotation(rot);
    //    gameObject.SetActive(true);
    //    particle.Play();
    //}
}
