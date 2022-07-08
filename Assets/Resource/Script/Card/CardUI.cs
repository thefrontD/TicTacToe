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
    [SerializeField] private Vector3 MouseOnScale;
    private Vector3 originPos;
    private Vector3 originScale;
    [SerializeField] private SpriteRenderer CardBackground;
    private SpriteRenderer CardImage;
    [SerializeField] private TextMeshPro CardCostText;
    [SerializeField] private TextMeshPro CardNameText;
    [SerializeField] private TextMeshPro CardDescText;
    //[SerializeField] private TextMeshPro CardEffectExplanation;

    public void init(Card card)
    {
        this.Card = card;

        originScale = transform.localScale;

        CardNameText.text = Card.CardName;
        CardDescText.text = Card.CardDesc;
        CardCostText.text = Card.CardCost.ToString();
    }

    private void OnMouseEnter()
    {
        originPos = transform.position;
        transform.DOMove(originPos + MouseOnPos, animationDuration);
        transform.DOScale(MouseOnScale, animationDuration);
    }
    
    private void OnMouseExit()
    {
        transform.DOMove(originPos, animationDuration);
        transform.DOScale(originScale, animationDuration);
    }
}
