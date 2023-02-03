using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using UnityEngine;

public class MoveCard : Card
{
    [JsonProperty] public MoveCardEffect MoveCardEffect;
    public TriggerCondition triggerCondition;
    [JsonProperty] public MoveDirection moveDirection;     // 상하좌우로 이동, 대각선으로 이동, 어느 칸으로든 이동 등

    public MoveCard(string cardName, string cardDesc, int cardCost, List<CardPoolAttribute> cardPoolAttributes, TriggerCondition triggerCondition,
        AdditionalEffectCondition additionalEffectCondition, AdditionalEffect additionalEffect,
        MoveCardEffect moveCardEffect, MoveDirection moveDirection)
        : base(cardName, cardDesc, cardCost, cardPoolAttributes, triggerCondition, additionalEffectCondition, additionalEffect)
    {
        this.cardType = CardType.Move;
        this.MoveCardEffect = moveCardEffect;
        this.triggerCondition = triggerCondition;
        this.moveDirection = moveDirection;
        //Debug.Log(this.CardName);
    }

    public override void usingCardSpecific()
    {
        // 카드를 낼 때 나오는 이펙트 (이동 이펙트랑은 다름)
        // 1초 동안 바람 이펙트 - 10개의 LinearWind, 4개의 LoopWind를 소환하여 1초 동안 애니메이션을 재생하다가 사라지도록 한다.
        //GameObject[] linearWinds = new GameObject[10];
        //for (int i = 0; i < linearWinds.Length; i++)
        //{
        //    //linearWinds[i] = 
        //}

        // State를 만드는 부분
        MoveState state = new MoveState(this);
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