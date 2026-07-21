using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform playerCanvas;
    private Transform previousParent;
    private RectTransform rect;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        playerCanvas = transform.root.GetComponent<Canvas>()?.transform; 
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    //public void Setup(Transform playerCanvas)
    //{
    //    this.playerCanvas = playerCanvas;
    //}

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 직전에 소속되어 있던 부모 Transform 정보 저장
        previousParent = transform.parent;

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

        Debug.Log($"[OnBeginDrag] alpha: {canvasGroup.alpha}, blocksRaycasts: {canvasGroup.blocksRaycasts}");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그를 시작하면 부모가 playerCanvas로 설정되기 때문에
        // 드래그 종료 시 부모가 playerCanvas이면 아이템 슬롯이 아닌 엉뚱한 곳에
        // 드롭을 했다는 뜻이기 때문에 드래그 직전 소속된 아이템 슬롯으로 아이템 이동
        if( transform.parent == playerCanvas )
        {
            // 마지막에 소속된 previousParent의 자식으로 설정하고, 해당 위치로 설정
            transform.SetParent(previousParent);
            rect.position = previousParent.GetComponent<RectTransform>().position;
        }

        // 알파값을 1로 설정하고, 광선 충돌처리가 되도록 한다
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }
}
