using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;
using static Inventory;
public class Inventory : Singleton<Inventory>
{
    [Header("외부 참조")]
    [HideInInspector] public InventoryUI invenUI;
    private Player player;
    private Enemy enemy;

        // 캐릭터
    public delegate void OnChangeCharacter();
    public delegate void OnCharacterSlotCountChange(int val);
    public OnChangeCharacter onChangeCharacter;
    public OnCharacterSlotCountChange onCharacterSlotCountChange;

    // 아이템
    public delegate void OnChangeItem();
    public delegate void OnItemSlotCountChange(int val);
    public OnChangeItem onChangeItem;
    public OnItemSlotCountChange onItemSlotCountChange;

    // 리스트
    public List<Character.CharacterData> characters = new List<Character.CharacterData>();
    public List<Item.ItemData> items = new List<Item.ItemData>();
    public List<FollowerController> activeFollowers;

    [Header("수량 데이터")]
    // 인벤토리 캐릭터(몹), 아이템 보유(획득)수량 표시
    [HideInInspector] public int acquiredCharacters = 0;
    [HideInInspector] public int acquiredItems = 0;
    [HideInInspector] public int pickupMobCount = 0;
    [SerializeField] private int characterSlotCnt;
    [SerializeField] private int itemSlotCnt;    private int faintedLayerMask;

    public int CharacterSlotCnt
    {
        get => characterSlotCnt;
        set
        {
            characterSlotCnt = value;
            onCharacterSlotCountChange?.Invoke(characterSlotCnt);
        }
    }
    public int ItemSlotCnt
    {
        get => itemSlotCnt;
        set
        {
            itemSlotCnt = value;
            onItemSlotCountChange?.Invoke(itemSlotCnt);
        }
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        faintedLayerMask = LayerMask.GetMask("Fainted");
    }
    void Start()
    {
        CharacterSlotCnt = characterSlotCnt;
        ItemSlotCnt = itemSlotCnt;
        //ItemSlotCnt = invenUI.itemSlots.Length;
        //CharacterSlotCnt = invenUI.characterSlots.Length;
    }
    private void Update()
    {
        DetectMob();
    }

    public bool AddCharacter(Character.CharacterData character)
    {
        if (characters.Count < CharacterSlotCnt)
        {
            characters.Add(character);
            acquiredCharacters++;
            onChangeCharacter?.Invoke();
            return true;
        }
        else return false;
    }
    public bool AddItem(Item.ItemData _item)
    {
        if (items.Count < ItemSlotCnt)
        {
            items.Add(_item);
            acquiredItems++;
            
            onChangeItem?.Invoke();
            
            return true;
        }
        else return false;
    }
    public void RemoveItem(Item.ItemData _item)
    {
        if (items.Remove(_item))
        {
            acquiredItems--;
            onChangeItem?.Invoke();
        }
    }
    public void RemoveCharacter(Character.CharacterData _character)
    {
        if (characters.Remove(_character))
        {
            acquiredCharacters--;
            onChangeCharacter?.Invoke();
            Debug.Log("Inventory.cs - RemoveCharacter (Object)");
        }
    }
    public void RemoveItem(int _index)
    {
        if (_index >= 0 && _index < items.Count)
        {
            items.RemoveAt(_index);
            acquiredItems--;
            onChangeItem?.Invoke();
        }
    }
    public void RemoveCharacter(int _index)
    {
        if (_index >= 0 && _index < characters.Count)
        {
            characters.RemoveAt(_index);
            acquiredCharacters--;
            onChangeCharacter?.Invoke();
            Debug.Log("Inventory.cs - RemoveCharacter (Index)");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("FieldItem"))
        {
            FieldItems fieldItems = collision.GetComponent<FieldItems>();
            if (AddItem(fieldItems.GetItem()))
            {
                fieldItems.DestroyItem();
            }
        }
    }
private void DetectMob()
    {
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, transform.right, 5f, faintedLayerMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, -transform.right, 5f, faintedLayerMask);

        RaycastHit2D hit;
        if (hitRight.collider != null) hit = hitRight;
        else if (hitLeft.collider != null) hit = hitLeft;
        else return;

        enemy = hit.collider.GetComponent<Enemy>();
        if (enemy == null) return;

        var data = enemy.GetCharacter();
        if (data == null) return;

        if (Input.GetKeyDown(KeyCode.V))
        {
            AddCharacter(data);
            pickupMobCount += 1;
            Destroy(enemy.gameObject);
        }
    }
}
