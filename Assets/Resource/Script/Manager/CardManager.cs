using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Card의 움직임에 대한 Manager
/// 데이터의 움직임이 아니라 GameObject로써의 Card 움직임은 여기서 작성할 것
/// </summary>
public class CardManager : Singleton<CardManager>
{
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private Vector3 DeckPos;
    [SerializeField] private Vector3 GravePos;
    private Quaternion backQuaternion;
    private Quaternion fronQuaternion;

    private List<GameObject> HandCardList = new List<GameObject>();
    private Queue<GameObject> DeckList = new Queue<GameObject>();
    private List<GameObject> GraveList = new List<GameObject>();

    void Start()
    {
        backQuaternion = Quaternion.Euler(Vector3.down * 180);
        fronQuaternion = Quaternion.Euler(Vector3.up * 180);
    }

    void Update()
    {
        
    }

    public void SetUp()
    {
        foreach (Card card in PlayerManager.Instance.PlayerCard)
        {
            GameObject newCard = Instantiate(CardPrefab, DeckPos, backQuaternion);
            newCard.GetComponent<CardUI>().init(card);
            DeckList.Enqueue(newCard);
        }
    }

    public void DrawCard(int drawNum)
    {
        for (int i = 0; i < drawNum; i++)
        {
            if (DeckList.Count != 0)
            {
                GameObject temp = DeckList.Dequeue();
                HandCardList.Add(temp);
                StartCoroutine(DrawCardAnimation(temp));
            }
            else
            {
                GraveToDeck();
                GameObject temp = DeckList.Dequeue();
                HandCardList.Add(temp);
                StartCoroutine(DrawCardAnimation(temp));
            }
        }
    }

    private IEnumerator DrawCardAnimation(GameObject card)
    {
        return null;
    }

    public void GraveToDeck()
    {
        GraveList.Shuffle();
        foreach (GameObject card in GraveList)
        {
            DeckList.Enqueue(card);
            StartCoroutine(GraveToDeckAnimation(card));
        }
    }

    private IEnumerator GraveToDeckAnimation(GameObject card)
    {
        card.transform.position = DeckPos;
        return null;
    }
}
