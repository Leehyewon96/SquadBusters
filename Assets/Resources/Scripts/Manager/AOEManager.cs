using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class AOEManager : MonoBehaviour
{
    private List<AOE> aoes = new List<AOE>();

    public AOE GetAOE(Vector3 pos)
    {
        AOE aoe = null;
        if (aoes.Count > 0)
        {
            aoe = aoes.Find(a => !a.gameObject.activeSelf);
        }

        if(aoe == null)
        {
            string path = $"Prefabs/AOE/AOE";
            GameObject newAoe = PhotonNetwork.Instantiate(path, pos, Quaternion.identity);
            aoe = newAoe.GetComponent<AOE>();
            aoe.gameObject.transform.SetParent(transform);
            aoes.Add(aoe);
        }

        aoe.gameObject.transform.position = pos;
        aoe.SetActive(true);

        return aoe;

    }

}
