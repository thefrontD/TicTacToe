using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using UnityEngine;

/// <summary>
/// Card 기본 클래스, AttackCard를 비롯한 각종 Card로 나눌 예정
/// 각 Card들이 가져야하는 필수적인 부분은 Card class에 abstract method선언 후 작성할 것
/// 실제로 Card를 Rendering 및 Animating 하는 부분은 CardUI라는 Script로 새로 작성 예정
/// </summary>
[JsonConverter(typeof(BaseConverter))]
public abstract class Card {
    
    private string cardName;
    public string CardName{
        get { return cardName; }
    }
    
    private string cardDesc;
    public string CardDesc{
        get { return cardDesc; }
    }

    private int cardCost;
    public int CardCost
    {
        get { return cardCost; }
        set
        {
            if (value < 0) cardCost = 0;
            else cardCost = value;
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public List<States> StatesList;

    public Card(string cardName, string cardDesc, int cardCost, List<States> statesList){
        this.cardName = cardName;
        this.cardDesc = cardDesc;
        this.cardCost = cardCost;
        this.StatesList = statesList;
    }
    
    /// <summary>
    /// Card마다 가질 수 있는 사용시의 개별적인 효과는 usingCardSpecific에서 작성할 것
    /// </summary>
    public abstract void usingCardSpecific();

    public void usingCard()
    {
        foreach (States states in StatesList)
            PlayerManager.Instance.StatesQueue.Enqueue(states);
        
        PlayerManager.Instance.StatesQueue.Enqueue(States.Normal);

        usingCardSpecific();
        
        PlayerManager.Instance.ChangeStates(PlayerManager.Instance.StatesQueue.Dequeue());
    }
}

public class AttackCard : Card
{
    private AttackCardEffect AttackCardEffect;

    public AttackCard(string cardName, string cardDesc, int cardCost, List<States> statesList, 
        AttackCardEffect attackCardEffect) : base(cardName, cardDesc, cardCost, statesList)
    {
        this.AttackCardEffect = attackCardEffect;
        Debug.Log(this.CardName);
    }

    public override void usingCardSpecific()
    {
        
    }
}

public class ColorCard : Card
{
    private ColorCardEffect ColorCardEffect;

    private bool IsSelectable;

    private bool IsMultiTarget;

    //used when Target is not selectable
    private ColorTargetPosition Target;

    public ColorCard(string cardName, string cardDesc, int cardCost, List<States> statesList,
        ColorCardEffect colorCardEffect, ColorTargetPosition Target) : base(cardName, cardDesc, cardCost, statesList)
    {
        this.ColorCardEffect = colorCardEffect;
        Debug.Log(this.CardName);
        Debug.Log(this.ColorCardEffect);
    }

    public override void usingCardSpecific()
    {
        //카드 사용 당시의 효과 당장은 무엇이 들어갈지 모른다.
        if(this.ColorCardEffect == ColorCardEffect.Color1){
            return;
        }
        if(this.ColorCardEffect == ColorCardEffect.Color2){
            return;
        }
    }

    public bool getIsSelectable(){
        return IsSelectable;
    }

    public bool getIsMultiTarget(){
        return IsMultiTarget;
    }

    public ColorTargetPosition getTarget(){
        return Target;
    }


}