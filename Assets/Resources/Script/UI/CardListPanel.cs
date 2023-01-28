using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class CardListPanel : MonoBehaviour
{
    [SerializeField] private GameObject CardImagePrefab;
    [SerializeField] private Transform ContentTransform;
    [SerializeField] private List<GameObject> _cardImageList;
    void Start()
    {
        
    }

    public void PrintCard(List<Card> CardList)
    {
        List<Card> Cards = CardList.OrderBy(x => x.CardCost).Reverse().OrderBy(x => x.CardName).OrderBy(x => x.CardType).ToList();
        int count = 0;
        
        foreach(Card card in Cards){
            //Debug.Log("loop count" + count.ToString());
            //Debug.Log("CardData" + CardData.CardName);
            _cardImageList[count].SetActive(true);
            _cardImageList[count].GetComponent<Transform>().SetParent(ContentTransform.gameObject.GetComponent<Transform>());
            _cardImageList[count].GetComponent<CardAcquiring>().SetCard(card);
            _cardImageList[count].GetComponent<CardAcquiring>().SetClickable(false);
            _cardImageList[count].GetComponent<CardAcquiring>().SetImage();
            
            count++;
        }
    }

    public void DeleteCard()
    {
        foreach (GameObject card in _cardImageList)
        {
            card.SetActive(false);
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
