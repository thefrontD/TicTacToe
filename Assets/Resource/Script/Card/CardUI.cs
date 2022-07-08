using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public Card Card;
    [SerializeField] private float animationDuration;
    [SerializeField] private Vector3 MouseOnPos;
    [SerializeField] private float MouseOnScale;
    private Vector3 originPos;
    private Vector3 originScale;
    [SerializeField] private SpriteRenderer CardBackground;
    private SpriteRenderer CardImage;
    [SerializeField] private TextMeshPro CardCostText;
    [SerializeField] private TextMeshPro CardNameText;
    [SerializeField] private TextMeshPro CardDescText;
    //[SerializeField] private TextMeshPro CardEffectExplanation;
    private bool isDrag = false;

    void Start()
    {
        originScale = transform.localScale;
        setPos();
    }

    public void init(Card card)
    {
        this.Card = card;

        setPos();

        CardNameText.text = Card.CardName;
        CardDescText.text = Card.CardDesc;
        CardCostText.text = Card.CardCost.ToString();
    }

    private void OnMouseEnter()
    {
        transform.DOMove(originPos + MouseOnPos, animationDuration);
        transform.DOScale(originScale * MouseOnScale, animationDuration);
    }
    
    private void OnMouseExit()
    {
        if(!isDrag)
        {
            transform.DOMove(originPos, animationDuration);
            transform.DOScale(originScale, animationDuration);
        }
    }

    private void setPos()
    {
        originPos = transform.position;
    }

    void OnMouseDown()
    {
        isDrag = true;
    }

    private void OnMouseDrag()
    {
        Vector3 trackPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(trackPosition.x, trackPosition.y, -0.5f);
    }
    
    void OnMouseUp()
    {
        isDrag = false;
        transform.DOMove(originPos, animationDuration);
        transform.DOScale(originScale, animationDuration);
    }
}