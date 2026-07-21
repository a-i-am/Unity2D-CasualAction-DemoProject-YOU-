using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    [Header("외부 참조")]
    [HideInInspector] public InventoryUI invenUI;
    private Enemy enemy;

    // Enum State
    private EnemyState enemyState;
    // Delegate
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
    [SerializeField] private int itemSlotCnt;

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

    public bool AddCharacter(Character.CharacterData _character)
    {
        if (_character == null || characters.Count >= CharacterSlotCnt) return false;
        Character.CharacterData character = _character.CreateInstance();
        character.slotIndex = FindAvailableCharacterSlot(character.type);
        if (character.slotIndex < 0) return false;
        characters.Add(character);
        acquiredCharacters++;
        onChangeCharacter?.Invoke();
        return true;
    }

    public bool AddItem(Item.ItemData _item)
    {
        if (_item == null || items.Count >= ItemSlotCnt) return false;
        Item.ItemData item = _item.CreateInstance();
        item.slotIndex = FindAvailableItemSlot(item.type);
        if (item.slotIndex < 0) return false;
        items.Add(item);
        acquiredItems++;
        onChangeItem?.Invoke();
        return true;
    }

    private int FindAvailableCharacterSlot(string type)
    {
        for (int i = 0; i < CharacterSlotCnt; i++)
            if (!characters.Exists(character => character.type == type && character.slotIndex == i)) return i;
        return -1;
    }

    private int FindAvailableItemSlot(string type)
    {
        for (int i = 0; i < ItemSlotCnt; i++)
            if (!items.Exists(item => item.type == type && item.slotIndex == i)) return i;
        return -1;
    }

    public void RemoveItem(int _index)
    {
        if (_index < 0 || _index >= items.Count) return;
        items.RemoveAt(_index);
        acquiredItems--;
        onChangeItem?.Invoke();
    }

    public void RemoveCharacter(int _index)
    {
        if (_index >= 0 && _index < characters.Count)
        {
            characters.RemoveAt(_index);
            acquiredCharacters--;
            onChangeCharacter?.Invoke();
            Debug.Log("Inventory.cs - RemoveCharacter");
        }
        else return;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        InventoryPickupCollector.TryCollect(this, collision);
    }
    private void DetectMob()
    {
        // 플레이어의 앞 방향으로 레이캐스트를 발사하여 적을 감지합니다.
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, transform.right, 5f, LayerMask.GetMask("Fainted"));
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, -transform.right, 5f, LayerMask.GetMask("Fainted"));

        RaycastHit2D hit = hitRight.collider != null ? hitRight : hitLeft.collider != null ? hitLeft : new RaycastHit2D();

        if (hit.collider != null)
        {
            enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null && enemyState != EnemyState.Fainted)
            {
                if (Input.GetKeyDown(KeyCode.V)) // Collect
                {
                    AddCharacter(enemy.GetCharacter());

                    pickupMobCount += 1;

                    if (enemy != null)
                    {
                        Destroy(enemy.gameObject);
                        Debug.Log("enemy 획득!");
                    }
                }
            }
        }
    }
}
