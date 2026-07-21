using UnityEngine;

public interface IInventoryPickup
{
    Item.ItemData GetItem();
    void DestroyItem();
}

public class FieldItems : MonoBehaviour, IInventoryPickup
{
    public Item.ItemData field_item;
    public SpriteRenderer image;

    public void SetItem(Item.ItemData _item)
    {
        field_item = _item;
        image.sprite = _item.itemImage;
    }

    public Item.ItemData GetItem()
    {
        return field_item;
    }

    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}
