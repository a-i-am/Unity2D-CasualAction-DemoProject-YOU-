using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
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
            pointable.OnPointerUpAction = OnPointerUp;
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

    public void OnPointerUp()
    {
        // 드래그 & 드롭
        if (itemData == null) return;
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
