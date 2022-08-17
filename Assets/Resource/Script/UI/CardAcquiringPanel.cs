using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<Card> TotalCardList;
    private int[] Count;
    private List<int> CardPool1 = new List<int>();
    //현재 카드 덱에 가장 많은 카드 풀의 카드
    private List<int> CardPool2 = new List<int>();
    //현재 카드 덱에 두번째로 많은 카드 풀의 카드
    private List<int> CardPool3 = new List<int>();
    //위 두 카드 풀에 없는 카드
    void Start()
    {
        TotalCardList = CardData.Instance._load("TotalCardData");
        Count = new int[System.Enum.GetValues(typeof(CardPoolAttribute)).Length];
        //초기화
        for(int i = 0; i<Count.Length; i++)
            Count[i] = 0;
        //player card list의 pool count
        foreach(Card pcard in PlayerManager.Instance.PlayerCard){
            foreach(CardPoolAttribute att in  pcard.CardPoolAttributes){
                if(att == CardPoolAttribute.BasicCardPool)//basic card pool은 카운트 하지 않는다
                    continue;
                Count[(int)att] ++;
            }
        }
        


        //가장 많은 pool, 다음으로 많은 pool 선정
        int first;
        int second;
        if(Count[0] > Count[1]){
            first = 0; second = 1;
        }
        else{
            first = 1; second = 0;
        }
        for(int i = 2; i<Count.Length; i++){
            if(Count[i] > Count[first])
                first = i;
            else if(Count[i] > Count[second])
                second = i;
        }
        Debug.Log($"biggest pool count is index : {first.ToString()} count : {Count[first].ToString()} ");
        Debug.Log($"second biggest pool count is index : {second.ToString()} count : {Count[second].ToString()} ");

        //Cardpool 구성
        foreach(var card in TotalCardList.Select((value, index) => new {value, index})){
            if(card.value.CardPoolAttributes.Contains((CardPoolAttribute)(Count[first])))
                CardPool1.Add(card.index);
            if(card.value.CardPoolAttributes.Contains((CardPoolAttribute)(Count[second])))
                CardPool2.Add(card.index);
            if(!(card.value.CardPoolAttributes.Contains((CardPoolAttribute)(Count[first]))
                ||card.value.CardPoolAttributes.Contains((CardPoolAttribute)(Count[second]))))
                CardPool3.Add(card.index);
        }

        //CardPool랜덤선택(중복제외)
        List<int> CardChosen = new List<int>();
        int newcard;
        newcard = Random.Range(0,CardPool1.Count);
        CardChosen.Add(CardPool1[newcard]);
        Debug.Log("first chosen card is " + CardPool1[newcard].ToString());
        while(CardChosen.Count < 2){
            newcard = Random.Range(0,CardPool2.Count);
            if(!CardChosen.Contains(CardPool2[newcard]))
                CardChosen.Add(CardPool2[newcard]);
            Debug.Log("second chosen card is " + CardPool2[newcard].ToString());
        }
        while(CardChosen.Count < 3){
            newcard = Random.Range(0,CardPool3.Count);
            if(!CardChosen.Contains(CardPool3[newcard]))
                CardChosen.Add(CardPool3[newcard]);
            Debug.Log("third chosen card is " + CardPool3[newcard].ToString());
        }

        //initialize 3개
        for(int i = 0; i < CardNum; i++)
            InitCard(CardPositionList[i], TotalCardList[CardChosen[i]]);
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
