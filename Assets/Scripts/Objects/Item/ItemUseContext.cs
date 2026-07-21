using UnityEngine;

public class ItemUseContext
{
    public PlayerHPValue Health { get; }
    public Inventory Inventory { get; }
    public Transform User { get; }

    public ItemUseContext(PlayerHPValue health, Inventory inventory = null, Transform user = null)
    {
        Health = health;
        Inventory = inventory;
        User = user;
    }
}
