using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using UnityEngine;

public class AttackCard : Card
{
    [JsonProperty] private AttackCardEffect AttackCardEffect;  //이펙트

    [JsonProperty] private int _targetType;                     //공격 가능한 대상의 종류
    public int TargetType => _targetType;
    [JsonProperty] private int _targetCount;                    //공격 가능한 대상의 수
    public int TargetCount => _targetCount;
    [JsonProperty] private int _attackCount;                    //공격 횟수
    public int AttackCount => _attackCount;
    [JsonProperty] private int _damage;                         //공격의 피해량 
    public int Damage => _damage;

    public int GetTargetType() => TargetType;
    public AttackCard(string cardName, string cardDesc, int cardCost, List<CardPoolAttribute> cardPoolAttributes, TriggerCondition triggerCondition,
        AdditionalEffectCondition additionalEffectCondition, AdditionalEffect additionalEffect,
        AttackCardEffect attackCardEffect, int targetType, int targetCount, int attackCount, int damage) 
        : base(cardName, cardDesc, cardCost, cardPoolAttributes, triggerCondition, additionalEffectCondition, additionalEffect)
    //카드 이름, 카드 설명, 카드 코스트, StatesList,
    //공격 가능한 대상의 종류, 공격 가능한 대상의 수, 공격 횟수, 공격의 피해량
    {
        //Debug.Log(this.CardName);
        this.cardType = CardType.Attack;
        this.AttackCardEffect = attackCardEffect;
        this._targetType = targetType;
        this._targetCount = targetCount;
        this._attackCount = attackCount;
        this._damage = damage;
    }

    public override void usingCardSpecific()
    {
        //발동조건, 이펙트
        // State를 만드는 부분
        AttackState state = new AttackState(this);
        NormalState normal = new NormalState();  // 다 끝나고 다시 normal state로 돌아온다.

        // State를 Enqueue하는 부분
        if(GameManager.Instance.IsPuzzleMode) 
        {
            PlayerManager.Instance.StatesQueue.Enqueue(state);
            PlayerManager.Instance.StatesQueue.Enqueue(new EnemyState());
        }
        else 
        {
            PlayerManager.Instance.StatesQueue.Enqueue(state);
            PlayerManager.Instance.StatesQueue.Enqueue(normal);
        }
    }
}