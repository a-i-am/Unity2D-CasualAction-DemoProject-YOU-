using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointable : MonoBehaviour, IPointerUpHandler, IPointerClickHandler
{
    public Action OnClick;
    public Action OnPointerUpAction;

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpAction?.Invoke();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }

}
