using UnityEngine;
public class Inventory : Singleton<Inventory>
{
    public InventoryList<ItemInstance> Items { get; } = new InventoryList<ItemInstance>();
    public InventoryList<CharacterInstance> Characters { get; } = new InventoryList<CharacterInstance>();

    private void Start()
    {
        Items.SlotCount = 20;
        Characters.SlotCount = 50;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("FieldItem"))
        {
            Item fieldItem = collision.GetComponent<Item>();
            int dropItemID = fieldItem.GetItemID();

            ItemMasterData master = InventoryDatabase.Instance.GetMasterData(dropItemID);

            if (master != null)
            {
                ItemInstance newItem = new ItemInstance(master, 1);

                if (Items.Add(newItem))
                {
                    fieldItem.DestroyItem();
                }
            }
        }
    }
}
