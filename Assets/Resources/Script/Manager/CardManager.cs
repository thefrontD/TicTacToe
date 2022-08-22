using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EPOOutline;
using UnityEngine;

/// <summary>
/// Card의 움직임에 대한 Manager
/// 데이터의 움직임이 아니라 GameObject로써의 Card 움직임은 여기서 작성할 것
/// </summary>
public class CardManager : Singleton<CardManager>
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Vector3 deckPos;
    [SerializeField] private Vector3 gravePos;
    [SerializeField] private Vector3 originHandPos;
    [SerializeField] private List<Vector3> cardPositionList;
    private Quaternion _backQuaternion;
    private Quaternion _fronQuaternion;

    private List<CardUI> _handCardList = new List<CardUI>();
    private Queue<CardUI> _deckList = new Queue<CardUI>();
    private List<CardUI> _graveList = new List<CardUI>();

    public List<CardUI> HandCardList => _handCardList;
    public Queue<CardUI> DeckList => _deckList;
    public List<CardUI> GraveList => _graveList;

    void Start()
    {
        _backQuaternion = Quaternion.Euler(0f, 180f, 0f);
        _fronQuaternion = Quaternion.identity;
        Debug.Log(_backQuaternion);
    }

    void Update()
    {
        
    }

    public void SetUp()
    {
        foreach (Card card in PlayerManager.Instance.PlayerCard)
        {
            GameObject newCard = Instantiate(cardPrefab, deckPos, _backQuaternion);
            newCard.transform.eulerAngles =  Vector3.up * 180;
            CardUI cardui = newCard.GetComponent<CardUI>();
            cardui.init(card);
            _deckList.Enqueue(cardui);
        }
    }

    public void Tutorial4(object sender, EventArgs e)
    {
        DrawCard(1);
    }
    
    public void DrawCard(int drawNum)
    {
        if(drawNum > _deckList.Count + _graveList.Count)
            StartCoroutine(DrawCardCoroutine(_deckList.Count + _graveList.Count));
        else
            StartCoroutine(DrawCardCoroutine(drawNum));
    }

    public void CheckUsable()
    {
        foreach (CardUI card in HandCardList)
        {
            if (card.Card.CheckCondition())
                card.gameObject.GetComponent<Outlinable>().enabled = true;
            else
                card.gameObject.GetComponent<Outlinable>().enabled = false;
        }
    }
    
    private IEnumerator DrawCardCoroutine(int drawNum)
    {
        for (int i = 0; i < drawNum; i++)
        {
            if (_deckList.Count == 0)
            {
                GraveToDeck();
                yield return new WaitForSeconds(1f);
            }
            CardUI drawCard = _deckList.Dequeue();
            _handCardList.Add(drawCard);
            drawCard.isHand = true;
            drawCard.SetSortingOrder(0);
            DrawCardAnimation(drawCard);
            yield return new WaitForSeconds(0.3f);
        }

        CheckUsable();
        
        if (PlayerManager.Instance.TutorialPhase == 2)
        {
            yield return new WaitForSeconds(1);
            
            TutorialManager.Instance.toNextTutorial(PlayerManager.Instance.TutorialPhase);
        }
        
        if (PlayerManager.Instance.TutorialPhase == 8)
        {
            yield return new WaitForSeconds(0.6f);
            
            TutorialManager.Instance.toNextTutorial(PlayerManager.Instance.TutorialPhase);
        }

        PlayerManager.Instance.LockTurn = false;
    }

    private void CardPositionAdjust()
    {
        int cardNum = 0;
        int idx = 11 - _handCardList.Count;

        foreach (CardUI card in _handCardList)
        {
            Vector3 cardPosition = cardPositionList[idx];
            card.transform.DOMove(cardPosition, 0.1f, false);
            card.Idx = cardNum;
            card.setPos(cardPosition, idx);
            idx += 2;
            cardNum++;
        }
    }

    private void DrawCardAnimation(CardUI card)
    {
        CardPositionAdjust();
        SoundManager.Instance.PlaySE("Draw");
        card.transform.DOMove(cardPositionList[9 + _handCardList.Count], 0.2f, false);
        card.transform.DORotate(new Vector3(-30, 0, 0), 0.2f, RotateMode.Fast);
    }

    public void AllHandCardtoGrave()
    {
        for (int i=_handCardList.Count-1; i>=0; i--)
            HandtoGrave(i);
    }

    public void HandtoGrave(int idx)
    {
        CardUI card = _handCardList[idx];
        card.ToGrave();
        card.isHand = false;
        card.transform.DOMove(gravePos, 0.2f, false);
        card.transform.DORotate(new Vector3(0, 180, 0), 0.2f, RotateMode.Fast);
        
        _graveList.Add(card);
        _handCardList.RemoveAt(idx);

        CardPositionAdjust();
    }

    public void GraveToDeck()
    {
        _graveList.Shuffle();
        SoundManager.Instance.PlaySE("DeckShuffle");
        foreach (CardUI card in _graveList)
        {
            _deckList.Enqueue(card);
            card.transform.DOMove(deckPos, 0.2f, false);
        }
        _graveList.Clear();
    }
}
