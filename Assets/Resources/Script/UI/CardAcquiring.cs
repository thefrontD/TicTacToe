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
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI cardCost;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardDesc;

    void Start()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(Clickable == true)
        {
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
        Sprite ImageTo = Resources.Load<Sprite>($"Images/Cards/{card.CardType}/BackGround");
        transform.GetComponent<Image>().sprite = ImageTo;
        //set mana
        cardCost.text = card.CardCost.ToString();
        
        string cardImagePath = card.CardName.Replace(':', '-').Replace('/', '_');
        //set cotent
        try{
            ImageTo = Resources.Load<Sprite>($"Images/Cards/{card.CardType}/" + cardImagePath);
        }
        catch(Exception e){
            Debug.Log("Image Not Found: card name: " +  card.CardName.ToString());
        }
        if(ImageTo != null)
            cardImage.sprite = ImageTo;

        //set title
        cardName.text = card.CardName.ToString();
        //set desc
        cardDesc.text = card.CardDesc.ToString();
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
