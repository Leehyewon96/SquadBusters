using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    List<Coin> coins = new List<Coin>();

    private void Awake()
    {
        coins = GetComponentsInChildren<Coin>().ToList();
        Init();
    }

    public void Init()
    {
        foreach (Coin coin in coins)
        {
            coin.SetActive(false);
        }
    }

    public Coin GetCoin()
    {
        Coin coin = coins.Find(c => !c.gameObject.activeSelf);
        if(coin == null)
        {
            coin = Instantiate(coins.FirstOrDefault(), transform);
            coins.Add(coin);
        }

        return coin;
    }

    public void ShowCoin(int num, Vector3 pos)
    {
        Vector3 offsetPos = new Vector3(0.5f, 0, 0.5f);
        pos.y = 1.2f;
        for (int i = 1; i <= num; ++i)
        {
            offsetPos *= i;
            Coin coin = GetCoin();
            coin.SetActive(true);
            coin.transform.position = pos + offsetPos;
        }
    }
}
