using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType type = ItemType.None;

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public ItemType GetItemType()
    {
        return type;
    }

    public void UpdateItemType(ItemType newType)
    {
        type = newType;
    }
}
