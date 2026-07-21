using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class InventoryDatabase : Singleton<InventoryDatabase>
{
    [Header("JSON 텍스트 에셋")]
    [SerializeField] private TextAsset itemDBText;
    [SerializeField] private TextAsset characterDBText;
    // JSON 데이터로 DB 구성
    private List<Character.CharacterData> myCharacterList = new List<Character.CharacterData>();
    private List<Item.ItemData> myItemList = new List<Item.ItemData>();

    // 델리게이트
    public delegate void OnCharacterSubTab();
    public delegate void OnItemSubTab();
    public OnCharacterSubTab onCharacterSubTab;
    public OnItemSubTab onItemSubTab;

    [Header("슬롯")]
    [SerializeField] private GameObject characterInvenSlotUI;
    [SerializeField] private GameObject itemInvenSlotUI;
    [SerializeField] private GameObject characterSlotNumText;
    [SerializeField] private GameObject itemSlotNumText;

    [Header("인벤토리 데이터 분류")]
    public string itemCurSubType = "Absorption";
    public string characterCurSubType = "A";
    [SerializeField] private string curMainTabType = "Character";

    [Header("인벤토리 UI")]
    [SerializeField] private GameObject characterSubTab;
    [SerializeField] private GameObject itemSubTab;
    [SerializeField] private Image[] mainTabImage;
    [SerializeField] private Image[] characterSubTabImage;
    [SerializeField] private Image[] itemSubTabImage;
    [SerializeField] private Sprite mainTabIdleSprite, mainTabSelectSprite;
    [SerializeField] private Sprite subTabIdleSprite, subTabSelectSprite;

    [Header("캐릭터/아이템 DB")]
    public List<Item.ItemData> allItemList = new List<Item.ItemData>();
    [SerializeField] private List<Character.CharacterData> allCharacterList = new List<Character.CharacterData>();
    //[SerializeField] private List<ItemInfo> itemInfos = new List<ItemInfo>();

    [Header("필드 아이템 배치")]
    [SerializeField] private GameObject fieldItemPrefab;
    [SerializeField] private Vector3[] pos;

    void Start()
    {
        // Start Parse DB Data  // JSON txt DB 직렬화-역직렬화
            SerializeCharactersDB();
            SerializeItemsDB();
            DeserializeCharactersDB();
            DeserializeItemsDB();
        
       // Start Load DB Resources
            LoadCharactersResources();
            LoadItemsResources();

        // Start Sync UI Data
        MainTabClick(curMainTabType);

        SpawnInitialFieldItems();
    }

    #region  Parse DB Data
    void SerializeCharactersDB()
    {
        string jdata = JsonConvert.SerializeObject(allCharacterList, Formatting.Indented); // JSON으로 리스트를 string으로 변환
        File.WriteAllText(Application.dataPath + "/Resources/Text/MyCharacterText.txt", jdata);
    }
    void SerializeItemsDB()
    {
        string jdata = JsonConvert.SerializeObject(allItemList, Formatting.Indented); // JSON으로 리스트를 string으로 변환
        File.WriteAllText(Application.dataPath + "/Resources/Text/MyItemText.txt", jdata);
    }
    void DeserializeCharactersDB()
    {
        // JSON파일 로드
        string path = Application.dataPath + "/Resources/Text/MyCharacterText.txt";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            myCharacterList = JsonConvert.DeserializeObject<List<Character.CharacterData>>(data);
        }
    }
    void DeserializeItemsDB()
    {
        string path = Application.dataPath + "/Resources/Text/MyItemText.txt";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            myItemList = JsonConvert.DeserializeObject<List<Item.ItemData>>(data);
        }
    }
    private void LoadCharactersResources()
    {
        string[] characterLine = characterDBText.text.Substring(0, characterDBText.text.Length - 1).Split('\n');

        for (int i = 0; i < characterLine.Length; i++)
        {
            string[] row = characterLine[i].Split('\t');
            for (int j = 0; j < row.Length; j++) row[j] = row[j].Trim();
            Sprite characterImage = Resources.Load<Sprite>(row[5]); // 캐릭터 이미지 경로에서 스프라이트 로드
            Follower characterPrefab = Resources.Load<Follower>(row[6]); // 캐릭터 프리팹 경로에서 로드

            allCharacterList.Add(new Character.CharacterData(
                row[0],  // type
                row[1],  // name
                row[2],  // explain
                row[3],  // number
                row[4] == "FALSE",  // isUsing
                characterImage, // Image
                characterPrefab // Prefab
            ));
        }

    }
    private void LoadItemsResources()
    {
        // 텍스트 파일 읽기
        itemDBText = Resources.Load<TextAsset>("Text/ItemDatabase");

        // 텍스트를 줄 단위로 파싱
        string[] itemLines = itemDBText.text.Substring(0, itemDBText.text.Length - 1).Split('\n');
        
        for (int i = 0; i < itemLines.Length; i++)
        {
            string[] row = itemLines[i].Split('\t');
            for (int j = 0; j < row.Length; j++) row[j] = row[j].Trim();
            string fullPath = row[5];
            string spritePath = fullPath.Substring(0, fullPath.LastIndexOf('/')).Replace("Assets/Resources/", "");
            string spriteName = fullPath.Substring(fullPath.LastIndexOf('/') + 1);

            Sprite itemImage = LoadSpriteFromMultiple(spritePath, spriteName);

            Item.ItemData newItem = new Item.ItemData(
                row[0],
                row[1],
                row[2],
                row[3],
                row[4] == "FALSE",
                itemImage,
                row[6]
                );
            SetItemEffect(newItem);
            allItemList.Add(newItem);
        }
    }
    private Sprite LoadSpriteFromMultiple(string path, string spriteName)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        Sprite target = sprites.FirstOrDefault(s => s.name == spriteName);
        return target;
    }

    #endregion

    #region Get Ready ON Data 
    private void SetItemEffect(Item.ItemData itemData)
    {
        switch (itemData.tabName?.Trim())
        {
            case "Absorption":
                // 회복 아이템이면, HealingEffect 스크립터블 오브젝트를 찾아서 연결
                ItemHealingEffect healingEffect = Resources.Load<ItemHealingEffect>("ScriptableObjects/BigEft");
                if (healingEffect != null)
                    itemData.efts.Add(healingEffect);
                break;

            case "Equipment":
                // 장비 아이템은 아직 별도 효과 없지만, 나중에 EquipmentEffect 만들 수 있음
                break;

            case "Mission":
                // 퀘스트/미션용 아이템은 효과 필요 없을 수도 있음
                break;

            default:
                Debug.LogWarning($"[InventoryDatabase] 알 수 없는 탭 이름: {itemData.tabName}");
                break;
        }
    }
    public void SpawnInitialFieldItems()
    {
        InstantiateFieldItems();
    }

    private void InstantiateFieldItems()
    {
        FieldItemSpawner.Spawn(fieldItemPrefab, pos, allItemList);
    }

    #endregion

    #region Sync UI Data
    public void MainTabClick(string tabName)
    {
        // 미리 설정된 인벤토리 탭 타입에 맞게 리스트 분류

        curMainTabType = tabName;

        int tabNum = 0;
        switch (tabName)
        {
            case "Character": tabNum = 0; break;
            case "Item": tabNum = 1; break;
            default: tabNum = 2; break;
        }
        for (int i = 0; i < mainTabImage.Length; i++)
            mainTabImage[i].sprite = i == tabNum ? mainTabSelectSprite : mainTabIdleSprite;

        switch (tabNum)
        {
            case 0:
                CharacterSubTabClick(characterCurSubType);

                itemInvenSlotUI.SetActive(false);
                itemSlotNumText.SetActive(false);
                characterInvenSlotUI.SetActive(true);
                characterSlotNumText.SetActive(true);
                characterSubTab.SetActive(true);
                itemSubTab.SetActive(false);
                break;
            case 1:
                ItemSubTabClick(itemCurSubType);

                characterInvenSlotUI.SetActive(false);
                characterSlotNumText.SetActive(false);
                itemInvenSlotUI.SetActive(true);
                itemSlotNumText.SetActive(true);
                itemSubTab.SetActive(true);
                characterSubTab.SetActive(false);
                break;
            default:
                characterInvenSlotUI.SetActive(false);
                itemInvenSlotUI.SetActive(false);
                characterSlotNumText.SetActive(false);
                itemSlotNumText.SetActive(false);
                characterSubTab.SetActive(false);
                itemSubTab.SetActive(false);
                break;
        }
    }
    public void CharacterSubTabClick(string tabName)
    {
        characterCurSubType = tabName;
        onCharacterSubTab?.Invoke();

        int tabNum = 0;
        switch (tabName)
        {
            case "A": tabNum = 0; break;
            case "B": tabNum = 1; break;
            case "C": tabNum = 2; break;
            case "D": tabNum = 3; break;
            default: tabNum = 4; break;
        }

        for (int i = 0; i < characterSubTabImage.Length; i++)
            characterSubTabImage[i].sprite = i == tabNum ? subTabSelectSprite : subTabIdleSprite;

        characterInvenSlotUI.SetActive(true);
        characterSlotNumText.SetActive(true);
        itemInvenSlotUI.SetActive(false);
        itemSlotNumText.SetActive(false);
    }
    public void ItemSubTabClick(string tabName)
    {
        itemCurSubType = tabName;
        onItemSubTab?.Invoke();

        int tabNum = 0;
        switch (tabName)
        {
            case "Absorption": tabNum = 0; break;
            case "Equipment": tabNum = 1; break;
            case "Etc": tabNum = 2; break;
            case "Mission": tabNum = 3; break;
            default: tabNum = 4; break;
        }

        for (int i = 0; i < itemSubTabImage.Length; i++)
            itemSubTabImage[i].sprite = i == tabNum ? subTabSelectSprite : subTabIdleSprite;

        itemInvenSlotUI.SetActive(true);
        itemSlotNumText.SetActive(true);
        characterInvenSlotUI.SetActive(false);
        characterSlotNumText.SetActive(false);
    }
    #endregion
}
