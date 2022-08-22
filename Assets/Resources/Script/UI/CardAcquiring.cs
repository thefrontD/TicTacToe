using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class CardAcquiring : MonoBehaviour, IPointerClickHandler
{
    Card card;
    private bool Clickable = true;
    void Start()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(Clickable == true)
        {
            Debug.Log("Card Selected");
            Debug.Log(card);
            if(!PlayerManager.Instance.TutorialTrigger)
                PlayerManager.Instance.PlayerCard.Add(card);
            else
                transform.parent.GetComponent<CardAcquiringPanel>().TutorialCardList.Add(card);
            transform.parent.GetComponent<CardAcquiringPanel>().EndCardAcquiring();
        }
        return;
    }

    public void SetCard(Card card)
    {
        this.card = card;
    }

    public void SetImage()
    {
        //set bg
        Debug.Log(card.CardType.ToString());
        Sprite ImageTo = LoadImage(Application.dataPath +"/Resources/Images/Cards/"+ card.CardType.ToString() +"/BackGround.png");
        transform.GetComponent<Image>().sprite = ImageTo;
        //set mana
        transform.Find("CostText").GetComponent<TextMeshProUGUI>().text = card.CardCost.ToString();
        
        string cardImagePath = card.CardName.Replace(':', '-').Replace('/', '_');
        //set cotent
        try{
            ImageTo = LoadImage(Application.dataPath +"/Resources/Images/Cards/"+ card.CardType.ToString() + "/" + cardImagePath + ".png");
        }
        catch(Exception e){
            Debug.Log("Image Not Found: card name: " +  card.CardName.ToString());
        }
        if(ImageTo != null)
            transform.Find("Object").GetComponent<Image>().sprite = ImageTo;

        //set title
        transform.Find("CardName").GetComponent<TextMeshProUGUI>().text = card.CardName.ToString();
        //set desc
        transform.Find("CardDesc").GetComponent<TextMeshProUGUI>().text = card.CardDesc.ToString();
    }

    Sprite LoadImage(string path)
    {
        byte[] byteTexture = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(0,0);
        texture.LoadImage(byteTexture);
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        return sprite;
    }

    public void SetClickable(bool cond)
    {
        this.Clickable = cond;
    }

    void Update()
    {
        
    }

}
