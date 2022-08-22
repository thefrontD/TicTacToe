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
    [SerializeField] private SpriteRenderer CardImage;
    [SerializeField] private TextMeshPro CardCostText;
    [SerializeField] private TextMeshPro CardNameText;
    [SerializeField] private TextMeshPro CardDescText;
    //[SerializeField] private TextMeshPro CardEffectExplanation;
    private bool isDrag = false;
    private int _originIdx = 0;
    public bool isHand = false;

    void Start()
    {
        originScale = transform.localScale;
        setPos();
    }

    public void init(Card card)
    {
        this.Card = card;

        /* ���ǻ���:
         * ī�� �̸����� ��η� ����� �� ���� Ư�����ڵ��� �� �ֽ��ϴ�.
         * ���� ī�� sprite�� ���ϸ��� ������ ���� �����߽��ϴ�.
         * ':' -> '-'  ex) "��ĥ : ���ڰ�" -> "��ĥ - ���ڰ�"
         * '/' -> '_'  ex) "��ĥ/�̵�" -> "��ĥ_�̵�"
         */
        // TODO: ����/�̵�, ����/��ĥ, ���� : ����, ���� : ī��, ���� : ��ȭ, ����� ����, ����, �̵�, �밢�� �̵�, ���� : ȸ�� ��Ʈ�� ����!!
        // TODO: ��Ʈ ���� �ذ� (SF �Թڴ� / �ȴϰ���), ī�� �޸� �����?
        CardBackground.sprite = Resources.Load<Sprite>($"Images/Cards/{card.CardType}/BackGround");
        string cardPathName = card.CardName.Replace(':', '-').Replace('/', '_');
        print(cardPathName);
        CardImage.sprite = Resources.Load<Sprite>($"Images/Cards/{card.CardType}/{cardPathName}");

        SetSortingOrder(0);

        CardNameText.text = Card.CardName;
        CardDescText.text = Card.CardDesc;
        CardCostText.text = Card.CardCost.ToString();
    }

    private void OnMouseEnter()
    {
        if(isHand)
        {
            SoundManager.Instance.PlaySE("MouseOver");
            transform.DOMove(originPos + MouseOnPos, animationDuration);
            transform.DOScale(originScale * MouseOnScale, animationDuration);
            SetSortingOrder(50);
        }
    }
    
    private void OnMouseExit()
    {
        if(isHand)
        {
            transform.DOMove(originPos, animationDuration);
            transform.DOScale(originScale, animationDuration);
            SetSortingOrder(_originIdx);
        }
    }

    public void setPos()
    {
        originPos = transform.position;
    }
    
    public void setPos(Vector3 pos, int idx)
    {
        originPos = pos;
        SetSortingOrder(idx);
        _originIdx = idx;
    }


    private void SetSortingOrder(int idx)
    {
        CardBackground.sortingOrder = idx;
        CardImage.sortingOrder = idx;
        CardCostText.sortingOrder = idx;
        CardNameText.sortingOrder = idx;
        CardDescText.sortingOrder = idx+1;
    }
    
    public void ToGrave()
    {
        SetSortingOrder(0);
        _originIdx = idx;
    }

    void OnMouseDown()
    {
        if (PlayerManager.Instance.state.GetType() == typeof(NormalState) && isHand)
        {
            if(Card.usingCard())
            {
                SoundManager.Instance.PlaySE("UsingCard");
                GetComponent<Outlinable>().enabled = false;
                CardManager.Instance.HandtoGrave(idx);
            }
            else if(PlayerManager.Instance.TutorialPhase == 16)
            {
                TutorialManager.Instance.toNextTutorial(PlayerManager.Instance.TutorialPhase);
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