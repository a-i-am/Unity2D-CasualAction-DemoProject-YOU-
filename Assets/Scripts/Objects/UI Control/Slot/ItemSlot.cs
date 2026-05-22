using Assets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [Header("아이템 정보")]
    public int itemSlotnum;
    public Image itemIcon;
    public Item.ItemData itemData;

    private Player player;

    [Header("외부 참조")]
    private Pointable pointable;
    private InventoryUI inventoryUI;
    private PlayerHPValue health;

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
        health = Player.Instance.health;
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

        bool isUse = itemData.UseItem(health);
        

        if (isUse && inventoryUI != null)
        {
            inventoryUI.RemoveItem(itemData);
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
        if (itemData != null)
        {
            itemData = null;
            itemIcon.gameObject.SetActive(false);
        }
    }


}

