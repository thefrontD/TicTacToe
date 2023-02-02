using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using UnityEngine;

public class ColorCard : Card
{
    [JsonProperty] private ColorCardEffect ColorCardEffect;
    [JsonProperty] private TriggerCondition costChangeCondition;
    private TriggerCondition triggerCondition;
    [JsonProperty] public ColorTargetPosition colorTargetPosition;
    [JsonProperty] public ColorTargetNum colorTargetNum;
    private AdditionalEffectCondition additionalEffectCondition;
    private AdditionalEffect additionalEffect;
    private bool cardUseValidity;

    public ColorCard(string cardName, string cardDesc, int cardCost, List<CardPoolAttribute> cardPoolAttributes, TriggerCondition costChangeCondition, TriggerCondition triggerCondition,
        ColorTargetPosition colorTargetPosition, ColorTargetNum colorTargetNum,
        AdditionalEffectCondition additionalEffectCondition, AdditionalEffect additionalEffect,
        ColorCardEffect colorCardEffect) : 
        base(cardName, cardDesc, cardCost, cardPoolAttributes, triggerCondition, additionalEffectCondition, additionalEffect)
    {
        this.cardType = CardType.Color;
        this.costChangeCondition = costChangeCondition;
        this.colorTargetPosition = colorTargetPosition;
        this.colorTargetNum = colorTargetNum;
        this.ColorCardEffect = colorCardEffect;
        //base에 누락인지 모르겠지만 없으므로 일단
        this.triggerCondition = triggerCondition;
        this.additionalEffectCondition = additionalEffectCondition;
        this.additionalEffect = additionalEffect;
    }
    
    public override void usingCardSpecific()
    {
        this.cardUseValidity = true;
        /*
        카드 사용이 취소되는 경우
        카드 사용 > 조건 불만족 > 자동 취소
        카드 사용 > 마우스 선택 과정에서 취소버튼?(아직 기획상에는 없는 기능)
        구현
        Card.usingCard -> ColorCard.usingCardSpecific
        어디에서 처리하든, 마나 소모값, 카드 사용중에 플레이어가 소모하는 HP,
        패 버림, 이동, 색칠 등의 값을, '돌이킬 수 있도록' 전달해야 함

        **아직 카드사용 조건이 맞지 않는 경우 외의 취소 액션은 기획된 바 없음
        일단은 단순하게 조건 확인->사용 으로 구현한 후 이후 해결
        */
        //CostChange Condition은 PlayerCard.json파일에 없다 없어서 직접 만들거나 변경 요청해야 테스트 할 수 있다.

        switch(this.costChangeCondition){
            case TriggerCondition.Bingo1:
                int BingoNum = BoardManager.Instance.CountBingo(BoardColor.Player);
                CardCost = CardCost - BingoNum;
                if(CardCost < 0)
                    CardCost = 0;
                break;
            default:
                break;
        }
        
        ////추가효과 
        //bool additionalEffectQualified = true;

        //if(additionalEffectQualified){
        //    switch(this.additionalEffect){
        //        case AdditionalEffect.Move:
        //            Debug.Log("AdditionalEffect : Move");
        //            /*MoveCard moveCard = new MoveCard("move", "movetocoloredblock", 0,
        //                TriggerCondition.None, AdditionalEffectCondition.None, 
        //                AdditionalEffect.None, MoveCardEffect.Run, 
        //                MoveDirection.Colored);
        //            MoveState moveState = new MoveState(moveCard);*/
        //            //movestate사용에 문제가 있음
        //            break;
        //        case AdditionalEffect.PlayerHp10:
        //            Debug.Log("AdditionalEffect : PlayerHp-10");
        //            PlayerManager.Instance.SetHp(-10);
        //            break;
        //        case AdditionalEffect.DumpALL:
        //            Debug.Log("AdditionalEffect : Dumpall");
        //            CardManager.Instance.AllHandCardtoGrave();
        //            break;
        //        default:
        //            break;
        //    }
        //}
        // State를 만드는 부분
        ColorState newState = new ColorState(this);
        PlayerManager.Instance.StatesQueue.Enqueue(newState);
        // State를 Enqueue하는 부분
        PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());

        if(GameManager.Instance.IsPuzzleMode) 
        {
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            PlayerManager.Instance.StatesQueue.Enqueue(new EnemyState());
        }
        else 
        {
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
        // State를 Enqueue하는 부분
            PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());
        }
    }
}