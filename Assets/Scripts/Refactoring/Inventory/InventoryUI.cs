// InventoryUI.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private InventoryDatabase invenDB;
    private Inventory inven;

    private bool activeInventory = false;

    [Header("패널 및 서브 요소")]
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject characterInvenSlotUI;
    [SerializeField] private GameObject itemInvenSlotUI;
    [SerializeField] private GameObject characterSubTab;
    [SerializeField] private GameObject itemSubTab;

    [Header("텍스트")]
    [SerializeField] private TextMeshProUGUI itemSlotNumText;
    [SerializeField] private TextMeshProUGUI characterSlotNumText;

    [Header("슬롯 설정")]
    public ItemSlot[] itemSlots;
    public CharacterSlot[] characterSlots;
    [SerializeField] private Transform itemSlotHolder;
    [SerializeField] private Transform characterSlotHolder;

    [Header("탭 데이터 분류")]
    public string itemCurSubType = "Absorption";
    public string characterCurSubType = "A";
    [SerializeField] private string curMainTabType = "Character";

    [Header("탭 이미지 및 스프라이트")]
    [SerializeField] private Image[] mainTabImage;
    [SerializeField] private Image[] characterSubTabImage;
    [SerializeField] private Image[] itemSubTabImage;
    [SerializeField] private Sprite mainTabIdleSprite, mainTabSelectSprite;
    [SerializeField] private Sprite subTabIdleSprite, subTabSelectSprite;

    private void Awake()
    {
        if (itemSlotHolder != null) itemSlots = itemSlotHolder.GetComponentsInChildren<ItemSlot>();
        if (characterSlotHolder != null) characterSlots = characterSlotHolder.GetComponentsInChildren<CharacterSlot>();
    }

    private void Start()
    {
        inven = Inventory.Instance;
        invenDB = InventoryDatabase.Instance;

        inven.Items.OnSlotCountChange += ItemSlotChange;
        inven.Characters.OnSlotCountChange += CharacterSlotChange;

        inven.Items.OnChange += RedrawItemSlotUI;
        inven.Characters.OnChange += RedrawAllCharacterSlotsUI;

        if (inventoryPanel != null) inventoryPanel.SetActive(activeInventory);

        if (invenDB.IsInitialized)
        {
            MainTabClick(curMainTabType);
        }
        else
        {
            invenDB.OnDatabaseInitialized += () => MainTabClick(curMainTabType);
        }

        UpdateSlotCountTexts();
    }

    private void SlotCountChange<TSlot>(TSlot[] slots, int slotCount, Action<TSlot, int> setIndex) where TSlot : MonoBehaviour
    {
        if (slots == null) return;
        for (int i = 0; i < slots.Length; i++)
        {
            setIndex(slots[i], i);
            Button btn = slots[i].GetComponent<Button>();
            if (btn != null) btn.interactable = i < slotCount;
        }
    }

    private void RedrawSlots<TSlot, TData>(TSlot[] slots, List<TData> filtered, Action<TSlot> clear, Action<TSlot, TData> assign, Action<TSlot> refresh)
    {
        if (slots == null) return;
        for (int i = 0; i < slots.Length; i++) clear(slots[i]);
        for (int i = 0; i < filtered.Count && i < slots.Length; i++)
        {
            assign(slots[i], filtered[i]);
            refresh(slots[i]);
        }
    }

    private void UpdateSlotCountTexts()
    {
        if (itemSlotNumText != null) itemSlotNumText.text = string.Format("{0} / {1}", inven.Items.Acquired, inven.Items.SlotCount);
        if (characterSlotNumText != null) characterSlotNumText.text = string.Format("{0} / {1}", inven.Characters.Acquired, inven.Characters.SlotCount);
    }

    public void MainTabClick(string tabName)
    {
        curMainTabType = tabName;
        int tabNum = tabName == "Character" ? 0 : (tabName == "Item" ? 1 : 2);

        for (int i = 0; i < mainTabImage.Length; i++)
            if (mainTabImage[i] != null) mainTabImage[i].sprite = i == tabNum ? mainTabSelectSprite : mainTabIdleSprite;

        switch (tabNum)
        {
            case 0:
                CharacterSubTabClick(characterCurSubType);
                if (itemInvenSlotUI != null) itemInvenSlotUI.SetActive(false);
                if (itemSlotNumText != null) itemSlotNumText.gameObject.SetActive(false);
                if (characterInvenSlotUI != null) characterInvenSlotUI.SetActive(true);
                if (characterSlotNumText != null) characterSlotNumText.gameObject.SetActive(true);
                if (characterSubTab != null) characterSubTab.SetActive(true);
                if (itemSubTab != null) itemSubTab.SetActive(false);
                break;
            case 1:
                ItemSubTabClick(itemCurSubType);
                if (characterInvenSlotUI != null) characterInvenSlotUI.SetActive(false);
                if (characterSlotNumText != null) characterSlotNumText.gameObject.SetActive(false);
                if (itemInvenSlotUI != null) itemInvenSlotUI.SetActive(true);
                if (itemSlotNumText != null) itemSlotNumText.gameObject.SetActive(true);
                if (itemSubTab != null) itemSubTab.SetActive(true);
                if (characterSubTab != null) characterSubTab.SetActive(false);
                break;
        }
    }

    public void CharacterSubTabClick(string tabName)
    {
        characterCurSubType = tabName;
        RedrawAllCharacterSlotsUI();

        int tabNum = tabName == "A" ? 0 : (tabName == "B" ? 1 : (tabName == "C" ? 2 : (tabName == "D" ? 3 : 4)));
        for (int i = 0; i < characterSubTabImage.Length; i++)
            if (characterSubTabImage[i] != null) characterSubTabImage[i].sprite = i == tabNum ? subTabSelectSprite : subTabIdleSprite;
    }

    public void ItemSubTabClick(string tabName)
    {
        itemCurSubType = tabName;
        RedrawItemSlotUI();

        int tabNum = tabName == "Absorption" ? 0 : (tabName == "Equipment" ? 1 : (tabName == "Etc" ? 2 : (tabName == "Mission" ? 3 : 4)));
        for (int i = 0; i < itemSubTabImage.Length; i++)
            if (itemSubTabImage[i] != null) itemSubTabImage[i].sprite = i == tabNum ? subTabSelectSprite : subTabIdleSprite;
    }

    public void RemoveItem(ItemInstance itemInstance)
    {
        if (itemInstance != null) inven.Items.Remove(itemInstance);
    }

    public void RedrawItemSlotUI()
    {
        var filtered = inven.Items.List.FindAll(instance => instance.masterData != null && instance.masterData.type == itemCurSubType);
        RedrawSlots(itemSlots, filtered, s => s.Bind(null), (s, d) => s.Bind(d), s => s.UpdateUI());
        UpdateSlotCountTexts();
    }

    private void ItemSlotChange(int _) => SlotCountChange(itemSlots, inven.Items.SlotCount, (s, i) => s.slotIndex = i);

    public void RemoveCharacter(CharacterInstance characterInstance)
    {
        if (characterInstance != null) inven.Characters.Remove(characterInstance);
    }

    public void RedrawAllCharacterSlotsUI()
    {
        var filtered = inven.Characters.List.FindAll(c => c.masterData != null && c.masterData.type == characterCurSubType);
        RedrawSlots(characterSlots, filtered, s => s.Bind(null), (s, d) => s.Bind(d), s => s.UpdateUI());
        UpdateSlotCountTexts();
    }

    private void CharacterSlotChange(int _) => SlotCountChange(characterSlots, inven.Characters.SlotCount, (s, i) => s.slotIndex = i);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            activeInventory = !activeInventory;
            if (playerUI != null) playerUI.SetActive(activeInventory);
            if (inventoryPanel != null) inventoryPanel.SetActive(activeInventory);
        }
    }

    public void AddCharacterSlot()
    {
        if (characterSlots != null && inven.Characters.SlotCount < characterSlots.Length) inven.Characters.SlotCount++;
    }

    public void AddItemSlot()
    {
        if (itemSlots != null && inven.Items.SlotCount < itemSlots.Length) inven.Items.SlotCount++;
    }
}