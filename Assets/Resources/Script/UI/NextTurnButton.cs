using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class NextTurnButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private bool _isRotate = false;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if(!_isRotate)
            transform.DORotate(new Vector3(0, 0, -90), 0.5f).SetEase(Ease.InOutQuad);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if(!_isRotate)
            transform.DORotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.InOutQuad);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        StartCoroutine(LockRotation());
        transform.DORotate(new Vector3(0, 0, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad);
    }

    private IEnumerator LockRotation()
    {
        _isRotate = true;
        yield return new WaitForSeconds(1.1f);
        _isRotate = false;
    }
}
