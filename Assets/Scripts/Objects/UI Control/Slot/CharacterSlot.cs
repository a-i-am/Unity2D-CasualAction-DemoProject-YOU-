using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSlot : MonoBehaviour
{
    [Header("캐릭터 정보")]
    public int characterSlotnum;
    public Image characterIcon;
    public Character.CharacterData characterData;

    [Header("외부 참조")]
    private Pointable pointable;
    private Inventory inven;
    private InventoryUI invenUI;
    [SerializeField] private FollowerSpawner followerSpawner;
    [SerializeField] private Transform player;

    //private Coroutine clickFeedbackCoroutine;

    //public int spawnsOrder;
    // 소환된 몹들의 위치를 저장할 리스트
    //public List<Vector3> List_spawnPositions = new List<Vector3>();
    //private int lastSpawnedIndex = 0; // 마지막으로 Spawn()이 호출된 오브젝트의 번호를 저장
    private void Awake()
    {
        inven = Inventory.Instance;
        invenUI = transform.root.GetComponent<InventoryUI>();
        pointable = GetComponent<Pointable>();
        if (pointable != null)
        {
            pointable.OnClick = OnClick;
            pointable.OnPointerUpAction = OnPointerUp;
        }
    }
    public void OnPointerUp()
    {
        // 드래그 & 드롭
        if (characterData == null) return;
    }
    public void OnClick()
    {
        // 슬롯 클릭 트리거
        if (characterData == null) return;
        
            followerSpawner.SpawnFollower(characterData);
            invenUI.RemoveCharacter(characterData);
    }

    public void UpdateCharacterSlotUI()
    {
        if (characterData != null)
        {
            characterIcon.sprite = characterData.characterImage;
            characterIcon.gameObject.SetActive(true);
        }
    }

    public void RemoveCharacterSlot()
    {
        if (characterData != null)
        {
            characterData = null;
            characterIcon.gameObject.SetActive(false);
        }
    }


}
