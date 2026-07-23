using Assets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSlot : MonoBehaviour, IDropHandler
{
    [Header("캐릭터 정보")]
    public int characterSlotnum;
    public Image characterIcon;
    public Character.CharacterData characterData;

    [Header("외부 참조")]
    private Pointable pointable;
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
        invenUI = transform.root.GetComponent<InventoryUI>();
        pointable = GetComponent<Pointable>();
        if (pointable != null)
        {
            pointable.OnClick = OnClick;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableUI draggable = eventData.pointerDrag == null ? null : eventData.pointerDrag.GetComponent<DraggableUI>();
        CharacterSlot sourceSlot = draggable == null ? null : draggable.SourceCharacterSlot;
        if (sourceSlot == null && eventData.pointerDrag != null)
            sourceSlot = eventData.pointerDrag.GetComponentInParent<CharacterSlot>();
        if (sourceSlot == null || sourceSlot == this || invenUI == null) return;
        if (draggable != null) draggable.RestoreVisual();
        invenUI.SwapCharacterSlot(sourceSlot.characterSlotnum, characterSlotnum);
        if (draggable != null) draggable.ClearSourceSlot();
    }
    public void OnClick()
    {
        // 슬롯 클릭 트리거
        if (characterData == null) return;
        
            if (!followerSpawner.SpawnFollower(characterData)) return;
            Inventory.Instance.RememberFollower(characterData);
            invenUI.RemoveCharacterSlotAt(characterSlotnum);
        // onChangeCharacter.Invoke(); // 이벤트 방식
        /*
         * Invoke()를 호출하면, 그 자리에 연결된 메서드들이 즉시 순차적으로 호출됨
         * 따라서 RedrawCharacterSlotUI()는 OnClick()의 모든 동작이 끝난 후에 실행되는 것이 아니라, RemoveCharacter() 중간에 실행
        */

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
        characterData = null;
        characterIcon.gameObject.SetActive(false);
    }


}
