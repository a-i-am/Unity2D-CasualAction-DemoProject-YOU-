// InventoryDatabase.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class InventoryDatabase : Singleton<InventoryDatabase>
{
    private Dictionary<int, ItemMasterData> itemMasterDB = new Dictionary<int, ItemMasterData>();
    private Dictionary<int, CharacterMasterData> characterMasterDB = new Dictionary<int, CharacterMasterData>();

    private AsyncOperationHandle<TextAsset> itemTableHandle;
    private AsyncOperationHandle<TextAsset> characterTableHandle;

    public bool IsInitialized { get; private set; } = false;
    public System.Action OnDatabaseInitialized;

    private void Start()
    {
        InitializeDatabase();
    }

    public void InitializeDatabase()
    {
        itemTableHandle = Addressables.LoadAssetAsync<TextAsset>("ItemTable");
        itemTableHandle.Completed += OnItemTableLoaded;
    }

    private void OnItemTableLoaded(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            ParseItemData(handle.Result);

            characterTableHandle = Addressables.LoadAssetAsync<TextAsset>("CharacterTable");
            characterTableHandle.Completed += OnCharacterTableLoaded;
        }
        else
        {
            Debug.LogError("Failed to load ItemTable via Addressables.");
        }
    }

    private void OnCharacterTableLoaded(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            ParseCharacterData(handle.Result);
            IsInitialized = true;
            OnDatabaseInitialized?.Invoke();
        }
        else
        {
            Debug.LogError("Failed to load CharacterTable via Addressables.");
        }
    }

    private void ParseItemData(TextAsset textAsset)
    {
        string[] lines = textAsset.text.Replace("\r", "").Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] row = lines[i].Split('\t');

            if (int.TryParse(row[0], out int id))
            {
                ItemMasterData data = new ItemMasterData();
                data.itemID = id;
                data.type = row[1];
                data.itemName = row[2];
                data.explain = row[3];

                itemMasterDB[data.itemID] = data;
            }
        }
    }

    private void ParseCharacterData(TextAsset textAsset)
    {
        string[] lines = textAsset.text.Replace("\r", "").Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] row = lines[i].Split('\t');

            if (int.TryParse(row[0], out int id))
            {
                CharacterMasterData data = new CharacterMasterData();
                data.characterID = id;
                data.type = row[1];
                data.characterName = row[2];

                characterMasterDB[data.characterID] = data;
            }
        }
    }

    public ItemMasterData GetMasterData(int id)
    {
        if (itemMasterDB.TryGetValue(id, out var data)) return data;
        return null;
    }

    public CharacterMasterData GetCharacterMasterData(int id)
    {
        if (characterMasterDB.TryGetValue(id, out var data)) return data;
        return null;
    }

    private void OnDestroy()
    {
        if (itemTableHandle.IsValid()) Addressables.Release(itemTableHandle);
        if (characterTableHandle.IsValid()) Addressables.Release(characterTableHandle);
    }
}