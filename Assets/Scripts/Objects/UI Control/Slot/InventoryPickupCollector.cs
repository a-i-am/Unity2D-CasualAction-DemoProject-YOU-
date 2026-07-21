using UnityEngine;

public static class InventoryPickupCollector
{
    public static void TryCollect(Inventory inventory, Collider2D collision)
    {
        if (inventory == null || collision == null || !collision.CompareTag("FieldItem")) return;

        IInventoryPickup pickup = collision.GetComponent<IInventoryPickup>();
        if (pickup != null && inventory.AddItem(pickup.GetItem()))
        {
            pickup.DestroyItem();
        }
    }
}
