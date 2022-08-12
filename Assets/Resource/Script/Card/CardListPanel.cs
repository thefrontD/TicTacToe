using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardListPanel : MonoBehaviour
{
    public GameObject CardImagePrefab;
    public List<Card> PlayerCard;
    void Start()
    {
        PlayerCard = CardData.Instance._load("PlayerCard.json");
        PrintCard();
    }

    void PrintCard(){
        int count = 0;
        foreach(Card CardData in PlayerCard){
            Vector3 coord = new Vector3(-250 + count%5*125, -100 - count/5*200, 0);
            GameObject CardImage;
            CardImage = Instantiate(CardImagePrefab, coord, Quaternion.identity);
            CardImage.GetComponent<Transform>().SetParent(transform.Find("Viewport").gameObject.GetComponent<Transform>().Find("Content").gameObject.GetComponent<Transform>());
            CardImage.GetComponent<RectTransform>().anchoredPosition = coord;
            //find appropriate image and replace image
            //Debug.Log(CardImage.GetComponent<Image>().sprite );
            Sprite ImageTo = Resources.Load<Sprite>("Images/BlueBox.png");
            CardImage.GetComponent<Image>().sprite = ImageTo;
            //Debug.Log(CardImage.GetComponent<Image>().sprite );
            //이미지가 변하지 않음 수정필요

            count ++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
