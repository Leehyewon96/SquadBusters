using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType type = ItemType.Coin;

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

    protected void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(LayerLocalize.player))
        {
            other.gameObject.GetComponent<ICharacterItemInterface>().TakeItem(type);
            SetActive(false);
        }
    }
}
