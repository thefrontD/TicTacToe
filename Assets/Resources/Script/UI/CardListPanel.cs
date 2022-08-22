using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CardListPanel : MonoBehaviour
{
    public GameObject CardImagePrefab;
    public List<Card> PlayerCard;
    void Start()
    {
        PlayerCard = CardData.Instance._load("PlayerCard0");
        PrintCard();
    }

    void PrintCard(){
        int count = 0;
        string datapath = Application.dataPath;
        Debug.Log(datapath);
        foreach(Card CardData in PlayerCard){
            //Debug.Log("loop count" + count.ToString());
            //Debug.Log("CardData" + CardData.CardName);
            Vector3 coord = new Vector3(-250 + count%5*125, -100 - count/5*200, 0);
            GameObject CardImage;
            CardImage = Instantiate(CardImagePrefab, coord, Quaternion.identity);
            CardImage.GetComponent<Transform>().SetParent(transform.Find("Viewport").gameObject.GetComponent<Transform>().Find("Content").gameObject.GetComponent<Transform>());
            CardImage.GetComponent<RectTransform>().anchoredPosition = coord;
            CardImage.GetComponent<CardAcquiring>().SetCard(CardData);
            CardImage.GetComponent<CardAcquiring>().SetClickable(false);
            CardImage.GetComponent<CardAcquiring>().SetImage();
            //Debug.Log(CardImage.GetComponent<Image>().sprite );
            //이미지가 변하지 않음 수정필요

            count ++;
        }
    }

    Sprite LoadImage(string path){
        byte[] byteTexture = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(0,0);
        texture.LoadImage(byteTexture);
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        return sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
