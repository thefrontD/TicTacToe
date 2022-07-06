using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    private List<GameObject> CardList = new List<GameObject>();

    void Start()
    {
        backQuaternion = Quaternion.Euler(Vector3.down * 180);
        fronQuaternion = Quaternion.Euler(Vector3.up * 180);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp()
    {
        foreach (Card card in PlayerManager.Instance.PlayerCard)
        {
            GameObject newCard = Instantiate(CardPrefab, DeckPos, backQuaternion);
            newCard.GetComponent<CardUI>().init(card);
            CardList.Add(newCard);
        }
    }
}
