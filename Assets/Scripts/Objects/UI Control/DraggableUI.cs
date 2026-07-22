using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform playerCanvas;
    private Transform previousParent;
    private RectTransform rect;
    private CanvasGroup canvasGroup;
    public ItemSlot SourceSlot { get; private set; }
    public CharacterSlot SourceCharacterSlot { get; private set; }

    private void Awake()
    {
        playerCanvas = transform.root.GetComponent<Canvas>()?.transform; 
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 직전에 소속되어 있던 부모 Transform 정보 저장
        previousParent = transform.parent;
        SourceSlot = previousParent == null ? null : previousParent.GetComponentInParent<ItemSlot>();
        SourceCharacterSlot = previousParent == null ? null : previousParent.GetComponentInParent<CharacterSlot>();

        // 현재 드래그중인 UI가 화면의 최상단에 출력되도록 하기 위해
        transform.SetParent(playerCanvas); 
        transform.SetAsLastSibling(); // 가장 앞에 보이도록 마지막 자식으로 설정

        // 드래그 가능한 오브젝트가 하나가 아니고, 자식들을 가지고 있을 수도 있기에 CanvasGroup으로 통제
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        // 현재 스크린상의 마우스 위치를 UI 위치로 설정(UI가 마우스를 쫓아다니는 상태)
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        RestoreVisual();
        SourceSlot = null;
        SourceCharacterSlot = null;
    }

    public void RestoreVisual()
    {
        if (previousParent != null && transform.parent == playerCanvas)
        {
            transform.SetParent(previousParent);
            rect.position = previousParent.GetComponent<RectTransform>().position;
        }

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }

    public void ClearSourceSlot()
    {
        SourceSlot = null;
        SourceCharacterSlot = null;
    }
}
