using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverUIComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private MouseOverUIType UIType;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);

        MouseOverUIManager.Instance.DisplayUI(UIType, pos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseOverUIManager.Instance.EraseUI();
    }
}
