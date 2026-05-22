using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Inventory;
using System.Reflection;
using UnityEngine.TextCore.Text;

public class InventoryUI : MonoBehaviour
{
    private InventoryDatabase invenDB;
    private Inventory inven;

    private bool activeInventory = false;

    private List<Character.CharacterData> filteredCharacters = new List<Character.CharacterData>();
    private List<Item.ItemData> filteredItems = new List<Item.ItemData>();

    [Header("패널")]
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject inventoryPanel;

    [Header("텍스트")]
    [SerializeField] private TextMeshProUGUI itemSlotNumText;
    [SerializeField] private TextMeshProUGUI characterSlotNumText;

    [Header("슬롯")]
    public ItemSlot[] itemSlots;
    public CharacterSlot[] characterSlots;
    [SerializeField] private Transform itemSlotHolder;
    [SerializeField] private Transform characterSlotHolder;

    private void Awake()
    {
        itemSlots = itemSlotHolder.GetComponentsInChildren<ItemSlot>();
        characterSlots = characterSlotHolder.GetComponentsInChildren<CharacterSlot>();
    }
    private void Start()
    {
        inven = Inventory.Instance;
        invenDB = InventoryDatabase.Instance;

        inven.onItemSlotCountChange += ItemSlotChange;
        inven.onCharacterSlotCountChange += CharacterSlotChange;

        inven.onChangeItem += RedrawItemSlotUI;
        invenDB.onItemSubTab += RedrawItemSlotUI;

        inven.onChangeCharacter += () => RedrawAllCharacterSlotsUI();
        invenDB.onCharacterSubTab += () => RedrawAllCharacterSlotsUI();

        inventoryPanel.SetActive(activeInventory);

        //itemSlotNumText.text = string.Format("{0} / {1}", inven.acquiredItems, itemSlots.Length);
        itemSlotNumText.text = string.Format("{0} / {1}", inven.acquiredItems, inven.ItemSlotCnt);

        //characterSlotNumText.text = string.Format("{0} / {1}", inven.acquiredCharacters, characterSlots.Length);
        characterSlotNumText.text = string.Format("{0} / {1}", inven.acquiredCharacters, inven.CharacterSlotCnt);
    }

    void FixedUpdate()
    {
        //itemSlotNumText.text = string.Format("{0} / {1}", inven.acquiredItems, itemSlots.Length);
        //characterSlotNumText.text = string.Format("{0} / {1}", inven.acquiredCharacters, characterSlots.Length);
        characterSlotNumText.text = string.Format("{0} / {1}", inven.acquiredCharacters, inven.CharacterSlotCnt);
        itemSlotNumText.text = string.Format("{0} / {1}", inven.acquiredItems, inven.ItemSlotCnt);
    }

    #region 아이템 인벤토리 UI
    
    public void RemoveItem(Item.ItemData item)
    {
        if (item != null)
        {
            inven.RemoveItem(item);
            // RedrawItemSlotUI는 inven.onChangeItem 이벤트를 통해 자동 호출될 수도 있으나
            // 현재 코드 구조상 명시적으로 호출해주는 것이 안전함
            RedrawItemSlotUI();
        }
    }

    public void RedrawItemSlotUI()
    {
        // 이전 슬롯 필터링 데이터 초기화
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].RemoveItemSlot();
        }

        // 슬롯 데이터 필터링
        filteredItems = inven.items.FindAll(item => item.type == invenDB.itemCurSubType);
        for (int i = 0; i < filteredItems.Count && i < itemSlots.Length; i++)
        {
            itemSlots[i].itemData = filteredItems[i];
            itemSlots[i].UpdateItemSlotUI();
        }
    }

    private void ItemSlotChange(int val)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].itemSlotnum = i;

            if (i < inven.ItemSlotCnt)
                itemSlots[i].GetComponent<Button>().interactable = true;
            else
                itemSlots[i].GetComponent<Button>().interactable = false;
        }
    }

    #endregion

    #region 캐릭터 인벤토리 UI
    private void CharacterSlotChange(int val)
    {
        for (int i = 0; i < characterSlots.Length; i++)
        {
            characterSlots[i].characterSlotnum = i;

            if (i < inven.CharacterSlotCnt)
                characterSlots[i].GetComponent<Button>().interactable = true;
            else
                characterSlots[i].GetComponent<Button>().interactable = false;
        }
    }

    public void RedrawAllCharacterSlotsUI()
    {
        for (int i = 0; i < characterSlots.Length; i++)
        {
            characterSlots[i].RemoveCharacterSlot();
        }

        filteredCharacters = inven.characters.FindAll(character => character.type == invenDB.characterCurSubType);

        for (int i = 0; i < filteredCharacters.Count && i < characterSlots.Length; i++)
        {
            characterSlots[i].characterData = filteredCharacters[i];
            characterSlots[i].UpdateCharacterSlotUI();
        }
    }

    // 특정 슬롯만 비움
    public void RemoveCharacter(Character.CharacterData character)
    {
        if (character != null)
        {
            inven.RemoveCharacter(character);
            RedrawAllCharacterSlotsUI();
        }
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