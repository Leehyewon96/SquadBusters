using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public enum AOEType
{
    Red,
    Yellow,
    Black,

    End,
}

public class AOEManager : MonoBehaviour
{
    private List<AOE> aoes = new List<AOE>();

    public AOE GetAOE(Vector3 pos, AOEType aoeType, float radius)
    {
        AOE aoe = aoes.Find(a => !a.gameObject.activeSelf && a.GetAoeType() == aoeType);

        if (aoe == null)
        {
            string path = $"Prefabs/AOE/{aoeType}";
            GameObject newAoe;

            if (GameManager.IsTrainingMode)
            {
                GameObject prefab = Resources.Load<GameObject>(path);
                if (prefab == null)
                {
                    Debug.LogError($"[Training] AOE Prefab ¾øÀ½: {path}");
                    return null;
                }
                newAoe = Instantiate(prefab, pos, Quaternion.identity);
            }
            else
            {
                newAoe = PhotonNetwork.Instantiate(path, pos, Quaternion.identity);
            }

            aoe = newAoe.GetComponent<AOE>();
            aoe.gameObject.transform.SetParent(transform);
            aoes.Add(aoe);
        }

        aoe.gameObject.transform.position = pos;
        aoe.transform.localScale = Vector3.one * radius;
        aoe.SetActive(true);

        return aoe;
    }
}
