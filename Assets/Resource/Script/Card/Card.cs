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
    private AttackCardEffect AttackCardEffect;  //이펙트
    private int TriggerCondition;               //발동 조건
    private int TargetType;                     //공격 가능한 대상의 종류
    private int TargetCount;                    //공격 가능한 대상의 수
    private int AttackCount;                    //공격 횟수
    private int Damage;                         //공격의 피해량
    private int EffectTriggerCondition;         //추가 효과 발동 조건
    private int Effect;                         //추가 효과


    public AttackCard(string cardName, string cardDesc, int cardCost, List<States> statesList, 
        AttackCardEffect attackCardEffect, int triggerCondition, int targetType, int targetCount, int attackCount, int damage, int effectTriggerCondition, int effect) : base(cardName, cardDesc, cardCost, statesList)
        //카드 이름, 카드 설명, 카드 코스트, StatesList,
        //이펙트, 발동 조건, 공격 가능한 대상의 종류, 공격 가능한 대상의 수, 공격 횟수, 공격의 피해량, 추가 효과 발동 조건, 추가 효과 
    {
        Debug.Log(this.CardName);
        this.AttackCardEffect = attackCardEffect;
        this.TriggerCondition = triggerCondition;
        this.TargetType = targetType;
        this.TargetCount = targetCount;
        this.AttackCount = attackCount;
        this.Damage = damage;
        this.EffectTriggerCondition = effectTriggerCondition;
        this.Effect = effect;
    }

    public override void usingCardSpecific()
    {
        
    }
}