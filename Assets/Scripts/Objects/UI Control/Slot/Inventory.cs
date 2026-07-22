using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    private static readonly string[] DefaultItemBagTypes = { "Absorption", "Equipment", "Etc", "Mission" };

    public class InventoryBag
    {
        public Item.ItemData[] slots;
        private int nextSearchIndex;
        private int count;

        public InventoryBag(int size)
        {
            slots = new Item.ItemData[size];
        }

        public int Count => count;

        public void Resize(int size)
        {
            if (slots.Length == size) return;
            System.Array.Resize(ref slots, size);
            if (count > size) Recount();
            if (nextSearchIndex >= size) nextSearchIndex = 0;
        }

        public int FindAvailableSlot()
        {
            if (slots.Length == 0) return -1;

            for (int offset = 0; offset < slots.Length; offset++)
            {
                int index = (nextSearchIndex + offset) % slots.Length;
                if (slots[index] == null) return index;
            }

            return -1;
        }

        public bool Add(Item.ItemData item)
        {
            int index = FindAvailableSlot();
            if (index < 0) return false;

            item.slotIndex = index;
            slots[index] = item;
            count++;
            nextSearchIndex = (index + 1) % slots.Length;
            return true;
        }

        public bool RemoveAt(int index)
        {
            if (index < 0 || index >= slots.Length || slots[index] == null) return false;
            slots[index] = null;
            count--;
            if (index < nextSearchIndex) nextSearchIndex = index;
            return true;
        }

        private void Recount()
        {
            count = 0;
            for (int i = 0; i < slots.Length; i++)
                if (slots[i] != null) count++;
        }
    }

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
    private readonly Dictionary<string, InventoryBag> itemBags = new Dictionary<string, InventoryBag>();
    public delegate void OnChangeItem();
    public delegate void OnItemSlotCountChange(int val);
    public OnChangeItem onChangeItem;
    public OnItemSlotCountChange onItemSlotCountChange;

    // 리스트
    public List<Character.CharacterData> characters = new List<Character.CharacterData>();
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
            ResizeItemBags(itemSlotCnt);
            onItemSlotCountChange?.Invoke(itemSlotCnt);
        }
    }

    void Start()
    {
        CharacterSlotCnt = characterSlotCnt;
        ItemSlotCnt = itemSlotCnt;
        for (int i = 0; i < DefaultItemBagTypes.Length; i++)
            GetItemBag(DefaultItemBagTypes[i]);
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
        if (_item == null) return false;

        Item.ItemData item = _item.CreateInstance();
        if (!GetItemBag(item.type).Add(item)) return false;

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
        return GetItemBag(type).FindAvailableSlot();
    }

    public bool RemoveItem(string type, int slotIndex)
    {
        if (!GetItemBag(type).RemoveAt(slotIndex)) return false;

        acquiredItems--;
        onChangeItem?.Invoke();
        return true;
    }

    public bool SwapItem(string type, int fromSlotIndex, int toSlotIndex)
    {
        InventoryBag bag = GetItemBag(type);
        if (fromSlotIndex < 0 || fromSlotIndex >= bag.slots.Length) return false;
        if (toSlotIndex < 0 || toSlotIndex >= bag.slots.Length) return false;
        if (fromSlotIndex == toSlotIndex) return false;
        if (bag.slots[fromSlotIndex] == null) return false;

        Item.ItemData fromItem = bag.slots[fromSlotIndex];
        Item.ItemData toItem = bag.slots[toSlotIndex];

        bag.slots[toSlotIndex] = fromItem;
        bag.slots[fromSlotIndex] = toItem;

        fromItem.slotIndex = toSlotIndex;
        if (toItem != null) toItem.slotIndex = fromSlotIndex;

        onChangeItem?.Invoke();
        return true;
    }

    public Item.ItemData GetItemAt(string type, int slotIndex)
    {
        InventoryBag bag = GetItemBag(type);
        if (slotIndex < 0 || slotIndex >= bag.slots.Length) return null;
        return bag.slots[slotIndex];
    }

    public List<Item.ItemData> GetItems(string type)
    {
        InventoryBag bag = GetItemBag(type);
        List<Item.ItemData> result = new List<Item.ItemData>();
        for (int i = 0; i < bag.slots.Length; i++)
            if (bag.slots[i] != null) result.Add(bag.slots[i]);
        return result;
    }

    public int GetItemCount(string type)
    {
        return GetItemBag(type).Count;
    }

    private InventoryBag GetItemBag(string type)
    {
        if (string.IsNullOrEmpty(type)) type = string.Empty;
        if (!itemBags.TryGetValue(type, out InventoryBag bag))
        {
            bag = new InventoryBag(ItemSlotCnt);
            itemBags.Add(type, bag);
        }
        return bag;
    }

    private void ResizeItemBags(int size)
    {
        foreach (InventoryBag bag in itemBags.Values)
            bag.Resize(size);
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
