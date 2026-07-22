using System;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    // 외부 참조
    private InventoryDatabase invenDB;
    private Inventory inven;

    [Header("패널")]
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject inventoryPanel;
    private PlayerHPValue itemTargetHealth;
    [Header("텍스트")]
    [SerializeField] private TextMeshProUGUI itemSlotNumText;
    [SerializeField] private TextMeshProUGUI characterSlotNumText;
    [Header("슬롯")]
    public ItemSlot[] itemSlots;
    public CharacterSlot[] characterSlots;
    [SerializeField] private Transform itemSlotHolder;
    [SerializeField] private Transform characterSlotHolder;

    // 리스트    
    private List<Character.CharacterData> filteredCharacterList = new List<Character.CharacterData>();
    private List<Item.ItemData> filteredItemList = new List<Item.ItemData>();

    // 인벤토리 ON/OFF
    private bool activeInventory = false;
    private ItemUseContext itemUseContext;

    public PlayerHPValue ItemTargetHealth => itemTargetHealth;
    public ItemUseContext ItemUseContext => itemUseContext;

    private void Awake()
    {
        itemSlots = itemSlotHolder.GetComponentsInChildren<ItemSlot>(true);
        characterSlots = characterSlotHolder.GetComponentsInChildren<CharacterSlot>(true);
    }
    private void Start()
    {
        inven = Inventory.Instance;
        invenDB = InventoryDatabase.Instance;
        PlayerScr player = FindObjectOfType<PlayerScr>(true);
        if (itemTargetHealth == null && player != null) itemTargetHealth = player.health;
        SetItemTarget(itemTargetHealth);

        inven.onItemSlotCountChange += ItemSlotChange;
        inven.onCharacterSlotCountChange += CharacterSlotChange;

        inven.onChangeItem += RedrawItemSlotUI;
        invenDB.onItemSubTab += RedrawItemSlotUI;

        inven.onChangeCharacter += () => RedrawAllCharacterSlotsUI();
        invenDB.onCharacterSubTab += () => RedrawAllCharacterSlotsUI();

        inventoryPanel.SetActive(activeInventory);

        ItemSlotChange(inven.ItemSlotCnt);
        CharacterSlotChange(inven.CharacterSlotCnt);
        RedrawItemSlotUI();
        RedrawAllCharacterSlotsUI();
        UpdateSlotCountTexts();
    }

    void FixedUpdate()
    {
        UpdateSlotCountTexts();
    }

    private void UpdateSlotCountTexts()
    {
        characterSlotNumText.text = string.Format("{0} / {1}", inven.acquiredCharacters, inven.CharacterSlotCnt);
        itemSlotNumText.text = string.Format("{0} / {1}", inven.GetItemCount(invenDB.itemCurSubType), inven.ItemSlotCnt);
    }

    private void SetSlotButtons<TSlot>(TSlot[] slots, int slotCount, Action<TSlot, int> setIndex) where TSlot : MonoBehaviour
    {
        for (int i = 0; i < slots.Length; i++)
        {
            setIndex(slots[i], i);
            Button button = slots[i].GetComponent<Button>();
            if (button != null) button.interactable = i < slotCount;
        }
    }

    public void SetItemTarget(PlayerHPValue target)
    {
        itemTargetHealth = target;
        itemUseContext = new ItemUseContext(itemTargetHealth, inven, transform);
        if (itemSlots == null) return;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].SetUseContext(itemUseContext);
        }
    }

    #region 아이템 인벤토리 UI

    public void RemoveItemSlotAt(int index)
    {
        inven.RemoveItem(invenDB.itemCurSubType, index);
    }

    public void SwapItemSlot(int fromIndex, int toIndex)
    {
        inven.SwapItem(invenDB.itemCurSubType, fromIndex, toIndex);
    }

    public void RedrawItemSlotUI()
    {
        // 이전 슬롯 필터링 데이터 초기화
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].gameObject.SetActive(i < inven.ItemSlotCnt);
            itemSlots[i].RemoveItemSlot();
        }

        // 슬롯 데이터 필터링
        filteredItemList = inven.GetItems(invenDB.itemCurSubType);
        for (int i = 0; i < filteredItemList.Count; i++)
        {
            int slotIndex = filteredItemList[i].slotIndex;
            if (slotIndex < 0 || slotIndex >= itemSlots.Length) continue;
            itemSlots[slotIndex].gameObject.SetActive(true);
            itemSlots[slotIndex].itemData = filteredItemList[i];
            itemSlots[slotIndex].SetUseContext(itemUseContext);
            itemSlots[slotIndex].UpdateItemSlotUI();
        }
    }

    private void ItemSlotChange(int val)
    {
        SetSlotButtons(itemSlots, inven.ItemSlotCnt, (slot, i) => slot.itemSlotnum = i);
    }

    #endregion

    #region 캐릭터 인벤토리 UI
    private void CharacterSlotChange(int val)
    {
        SetSlotButtons(characterSlots, inven.CharacterSlotCnt, (slot, i) => slot.characterSlotnum = i);
    }

    public void RedrawAllCharacterSlotsUI()
    {
        for (int i = 0; i < characterSlots.Length; i++)
        {
            characterSlots[i].gameObject.SetActive(i < inven.CharacterSlotCnt);
            characterSlots[i].RemoveCharacterSlot();
        }

        filteredCharacterList = inven.characters.FindAll(character => character.type == invenDB.characterCurSubType);

        for (int i = 0; i < filteredCharacterList.Count; i++)
        {
            int slotIndex = filteredCharacterList[i].slotIndex;
            if (slotIndex < 0 || slotIndex >= characterSlots.Length) continue;
            characterSlots[slotIndex].gameObject.SetActive(true);
            characterSlots[slotIndex].characterData = filteredCharacterList[i];
            characterSlots[slotIndex].UpdateCharacterSlotUI();
        }
    }

    // 특정 슬롯만 비움
    public void RemoveCharacterSlotAt(int index)
    {
        int characterIndex = inven.characters.FindIndex(character =>
            character.type == invenDB.characterCurSubType && character.slotIndex == index);
        if (characterIndex >= 0) inven.RemoveCharacter(characterIndex);
    }

    #endregion

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            playerUI.SetActive(activeInventory); // 기본 false
            activeInventory = !activeInventory; // 기본 false -> true
            inventoryPanel.SetActive(activeInventory);
        }
    }

    // [+] 슬롯 추가 버튼 이벤트
    public void AddCharacterSlot()
    {
        if (inven.CharacterSlotCnt < characterSlots.Length)
            inven.CharacterSlotCnt++;
    }

    public void AddItemSlot()
    {
        if (inven.ItemSlotCnt < itemSlots.Length)
            inven.ItemSlotCnt++;
    }
}
