using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//전투 승리 후 나타날 카드 획득 창 관리
//3개의 카드를 중복없이 고른다
//position 3개는 array로 가지고 있는다.
//위치에 Instantiation 하고 카드 정보 업데이트 한다
//클릭했을 때 어떻게 진행하게 할 것인지 고민해본다
//
public class CardAcquiringPanel : MonoBehaviour
{
    [SerializeField] private List<Vector3> CardPositionList;
    [SerializeField] private int CardNum = 3;
    [SerializeField] private GameObject NewCardPrefab;
    private List<Card> CardPool;
    void Start()
    {
        CardPool = CardData.Instance._load("CardPool");

        //랜덤선택
        List<int> CardChosen = new List<int>();
        int newcard = -1;
        while(CardChosen.Count < CardNum){
            newcard = Random.Range(0,CardPool.Count);
            if(!CardChosen.Contains(newcard))
                CardChosen.Add(newcard);
            Debug.Log(newcard);
        }

        //initialize 3개
        for(int i = 0; i < CardNum; i++)
            InitCard(CardPositionList[i], CardPool[CardChosen[i]]);
    }

    void InitCard(Vector3 position, Card card){
        Debug.Log("InitCard");
        GameObject CardImage = Instantiate(NewCardPrefab, position, Quaternion.identity);
        CardImage.GetComponent<Transform>().SetParent(transform);
        CardImage.GetComponent<RectTransform>().anchoredPosition = position;
        CardImage.GetComponent<CardAcquiring>().SetCard(card);
        return;
    }

    public void EndCardAcquiring(){
        transform.gameObject.SetActive(false);
    }

    void Update()
    {
        
    }
}
