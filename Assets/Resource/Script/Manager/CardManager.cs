using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR;

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

    private List<GameObject> _handCardList = new List<GameObject>();
    private Queue<GameObject> _deckList = new Queue<GameObject>();
    private List<GameObject> _graveList = new List<GameObject>();

    public List<GameObject> HandCardList => _handCardList;
    public Queue<GameObject> DeckList => _deckList;
    public List<GameObject> GraveList => _graveList;

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
            newCard.GetComponent<CardUI>().init(card);
            _deckList.Enqueue(newCard);
        }
    }

    public void DrawCard(int drawNum)
    {
        if(drawNum > _deckList.Count + _graveList.Count)
            StartCoroutine(DrawCardCoroutine(_deckList.Count + _graveList.Count));
        else
            StartCoroutine(DrawCardCoroutine(drawNum));
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
            _handCardList.Add(_deckList.Dequeue());
            _handCardList[_handCardList.Count-1].GetComponent<CardUI>().isHand = true;
            DrawCardAnimation(_handCardList[_handCardList.Count-1]);
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void CardPositionAdjust()
    {
        int CardNum = 0;
        int idx = 11 - _handCardList.Count;

        foreach (GameObject card in _handCardList)
        {
            card.transform.DOMove(cardPositionList[idx], 0.1f, false);
            card.GetComponent<CardUI>().Idx = CardNum++;
            card.GetComponent<CardUI>().setPos(cardPositionList[idx]);
            idx += 2;
        }
    }

    private void DrawCardAnimation(GameObject card)
    {
        CardPositionAdjust();
        
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
        _graveList.Add(_handCardList[idx]);

        _handCardList[idx].GetComponent<CardUI>().isHand = false;
        _handCardList[idx].transform.DOMove(gravePos, 0.2f, false);
        _handCardList[idx].transform.DORotate(new Vector3(0, 180, 0), 0.2f, RotateMode.Fast);
        
        _handCardList.RemoveAt(idx);
        CardPositionAdjust();
    }

    public void GraveToDeck()
    {
        _graveList.Shuffle();
        foreach (GameObject card in _graveList)
        {
            _deckList.Enqueue(card);
            card.transform.DOMove(deckPos, 0.2f, false);
        }
        _graveList.Clear();
    }
}
