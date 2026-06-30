using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Recorder.OutputPath;

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






    public void OnBeginDrag(PointerEventData eventData)
    {

        previousParent = transform.parent;


        transform.SetParent(playerCanvas);
        transform.SetAsLastSibling();


        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {

        rect.position = eventData.position;

        Debug.Log($"[OnBeginDrag] alpha: {canvasGroup.alpha}, blocksRaycasts: {canvasGroup.blocksRaycasts}");
    }

    public void OnEndDrag(PointerEventData eventData)
    {



        if( transform.parent == playerCanvas )
        {

            transform.SetParent(previousParent);
            rect.position = previousParent.GetComponent<RectTransform>().position;
        }


        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }
}
