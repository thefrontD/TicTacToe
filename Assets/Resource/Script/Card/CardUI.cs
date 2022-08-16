using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EPOOutline;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    private int idx;
    public int Idx { get => idx; set => idx = (value >= 0) ? value : 0; }
    
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
    public bool isHand = false;

    void Start()
    {
        originScale = transform.localScale;
        setPos();
    }

    public void init(Card card)
    {
        this.Card = card;
        
        CardNameText.text = Card.CardName;
        CardDescText.text = Card.CardDesc;
        CardCostText.text = Card.CardCost.ToString();
    }

    private void OnMouseEnter()
    {
        if(isHand)
        {
            transform.DOMove(originPos + MouseOnPos, animationDuration);
            transform.DOScale(originScale * MouseOnScale, animationDuration);
        }
    }
    
    private void OnMouseExit()
    {
        if(isHand)
        {
            transform.DOMove(originPos, animationDuration);
            transform.DOScale(originScale, animationDuration);
        }
    }

    public void setPos()
    {
        originPos = transform.position;
    }
    
    public void setPos(Vector3 pos)
    {
        originPos = pos;
    }

    void OnMouseDown()
    {
        if (PlayerManager.Instance.state.GetType() == typeof(NormalState))
        {
            if(Card.usingCard())
            {
                GetComponent<Outlinable>().enabled = false;
                CardManager.Instance.HandtoGrave(idx);
            }
        }
    }

    /*
    private void OnMouseDrag()
    {
        if(isHand)
        {
            Vector3 trackPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(trackPosition.x, trackPosition.y, 0.5f);
        }
    }
    
    void OnMouseUp()
    {
        if(isHand)
        {
            isDrag = false;
            transform.DOMove(originPos, animationDuration);
            transform.DOScale(originScale, animationDuration);
            this.Card.usingCard();
        }
    }*/
}