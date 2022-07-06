using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public Card Card;
    
    [SerializeField] private SpriteRenderer CardBackground;
    private SpriteRenderer CardImage;
    [SerializeField] private TextMeshProUGUI CardCostText;
    [SerializeField] private TextMeshProUGUI CardNameText;
    [SerializeField] private TextMeshProUGUI CardDescText;
    [SerializeField] private TextMeshProUGUI CardEffectExplanation;

    void init(Card card)
    {
        this.Card = card;

        CardNameText.text = Card.CardName;
        CardDescText.text = Card.CardDesc;
        CardCostText.text = Card.CardCost.ToString();
    }

    private void OnMouseOver()
    {
        
    }
}
