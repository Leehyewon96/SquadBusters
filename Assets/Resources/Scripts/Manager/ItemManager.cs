using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemType
{ 
    Coin,
    Gem,
    TreasureBox,

    End,
}

public class ItemManager : MonoBehaviour
{
    List<Item> items = new List<Item>();

    private void Awake()
    {
        items = GetComponentsInChildren<Item>().ToList();
        Init();
    }

    public void Init()
    {
        foreach (Item item in items)
        {
            item.SetActive(false);
        }
    }

    public Item GetItem(ItemType itemType)
    {
        Item item = items.Find(it => !it.gameObject.activeSelf && it.GetItemType() == itemType);
        if(item == null)
        {
            Item originItem = items.Find(it => it.GetItemType() == itemType);
            item = Instantiate(originItem, transform);
            item.UpdateItemType(itemType);
            items.Add(item);
        }

        return item;
    }

    public void ShowItem(int num, Vector3 pos, ItemType itemType)
    {
        Vector3 randomPos = pos;
        randomPos.y = 1.2f;
        for (int i = 1; i <= num; ++i)
        {
            randomPos.x = Random.Range(pos.x - 1, pos.x + 1);
            randomPos.z = Random.Range(pos.z - 1, pos.z + 1);
            Item item = GetItem(itemType);
            item.SetActive(true);
            item.transform.position = randomPos;
        }
    }
}
