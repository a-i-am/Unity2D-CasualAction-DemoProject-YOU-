using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    [Header("아이템 정보")]
    public int itemSlotnum;
    public Image itemIcon;
    public Item.ItemData itemData;

    [Header("외부 참조")]
    private Pointable pointable;
    private InventoryUI inventoryUI;
    private ItemUseContext useContext;

    private void Awake()
    {
        inventoryUI = GetComponentInParent<InventoryUI>();
        pointable = GetComponent<Pointable>();
        if (pointable != null)
        {
            pointable.OnClick = OnClick;
        }
    }
    private void Start()
    {
        if (inventoryUI != null) SetUseContext(inventoryUI.ItemUseContext);
    }

    public void SetUseContext(ItemUseContext context)
    {
        useContext = context;
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableUI draggable = eventData.pointerDrag == null ? null : eventData.pointerDrag.GetComponent<DraggableUI>();
        ItemSlot sourceSlot = draggable == null ? null : draggable.SourceSlot;
        if (sourceSlot == null && eventData.pointerDrag != null)
            sourceSlot = eventData.pointerDrag.GetComponentInParent<ItemSlot>();
        if (sourceSlot == null || sourceSlot == this || inventoryUI == null) return;
        if (draggable != null) draggable.RestoreVisual();
        inventoryUI.SwapItemSlot(sourceSlot.itemSlotnum, itemSlotnum);
        if (draggable != null) draggable.ClearSourceSlot();
    }

    public void OnClick()
    {
        // 슬롯 클릭 트리거
        if (itemData == null) return;

        if (useContext == null) return;

        bool isUse = itemData.UseItem(useContext);
        

        if (isUse && inventoryUI != null)
        {
            inventoryUI.RemoveItemSlotAt(itemSlotnum);
        }
    }


    public void UpdateItemSlotUI()
    {
        if (itemData != null)
        {
            itemIcon.sprite = itemData.itemImage;
            itemIcon.gameObject.SetActive(true);
        }
    }

    public void RemoveItemSlot()
    {
        itemData = null;
        itemIcon.gameObject.SetActive(false);
    }
}
