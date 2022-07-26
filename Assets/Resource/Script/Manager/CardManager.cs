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

    private List<GameObject> HandCardList = new List<GameObject>();
    private Queue<GameObject> DeckList = new Queue<GameObject>();
    private List<GameObject> GraveList = new List<GameObject>();

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
            DeckList.Enqueue(newCard);
        }
    }

    public void DrawCard(int drawNum)
    {
        if(drawNum > DeckList.Count + GraveList.Count)
            StartCoroutine(DrawCardCoroutine(DeckList.Count + GraveList.Count));
        else
            StartCoroutine(DrawCardCoroutine(drawNum));
    }

    private IEnumerator DrawCardCoroutine(int drawNum)
    {
        for (int i = 0; i < drawNum; i++)
        {
            if (DeckList.Count == 0)
            {
                GraveToDeck();
                yield return new WaitForSeconds(1f);
            }
            HandCardList.Add(DeckList.Dequeue());
            HandCardList[HandCardList.Count-1].GetComponent<CardUI>().isHand = true;
            DrawCardAnimation(HandCardList[HandCardList.Count-1]);
            yield return new WaitForSeconds(0.3f);
        }

        foreach (GameObject card in HandCardList)
            Debug.Log(card.GetComponent<CardUI>().Idx);    
    }

    private void CardPositionAdjust()
    {
        int CardNum = 0;
        int idx = 11 - HandCardList.Count;

        foreach (GameObject card in HandCardList)
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
        
        card.transform.DOMove(cardPositionList[9 + HandCardList.Count], 0.2f, false);
        card.transform.DORotate(new Vector3(-30, 0, 0), 0.2f, RotateMode.Fast);
    }

    public void AllHandCardtoGrave()
    {
        for (int i=HandCardList.Count-1; i>=0; i--)
            HandtoGrave(i);
    }

    public void HandtoGrave(int idx)
    {
        GraveList.Add(HandCardList[idx]);

        HandCardList[idx].GetComponent<CardUI>().isHand = false;
        HandCardList[idx].transform.DOMove(gravePos, 0.2f, false);
        HandCardList[idx].transform.DORotate(new Vector3(0, 180, 0), 0.2f, RotateMode.Fast);
        
        HandCardList.RemoveAt(idx);
        CardPositionAdjust();
    }

    public void GraveToDeck()
    {
        GraveList.Shuffle();
        foreach (GameObject card in GraveList)
        {
            DeckList.Enqueue(card);
            card.transform.DOMove(deckPos, 0.2f, false);
        }
        GraveList.Clear();
    }
}
