using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverUIComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private MouseOverUIType UIType;
    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseOverUIManager.Instance.DisplayUI(UIType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseOverUIManager.Instance.EraseUI();
    }
}
