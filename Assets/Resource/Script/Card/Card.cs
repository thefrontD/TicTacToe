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
        usingCardSpecific();
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

public class MoveCard : Card
{
    private MoveCardEffect MoveCardEffect;
    private TriggerCondition triggerCondition;
    private MoveDirection moveDirection;     // 상하좌우로 이동, 대각선으로 이동, 어느 칸으로든 이동 등
    private int moveAmount;  // 한 번에 이동하는 양
    private Action afterPlayAction;

    public MoveCard(string cardName, string cardDesc, int cardCost, List<States> statesList,
        MoveCardEffect moveCardEffect, TriggerCondition triggerCondition, MoveDirection moveDirection, int moveAmount, Action afterPlayAction) : base(cardName, cardDesc, cardCost, statesList)
    {
        this.MoveCardEffect = moveCardEffect;
        this.triggerCondition = triggerCondition;
        this.moveDirection = moveDirection;
        this.moveAmount = moveAmount;  // 기본 1
        this.afterPlayAction = afterPlayAction;
        Debug.Log(this.CardName);
    }

    public override void usingCardSpecific()
    {
        // 1초 동안 바람 이펙트
        // 10개의 LinearWind, 4개의 LoopWind를 소환하여 1초 동안 애니메이션을 재생하다가 사라지도록 한다.
        GameObject[] linearWinds = new GameObject[10];
        for (int i = 0; i < linearWinds.Length; i++)
        {
            //linearWinds[i] = 
        }
        // 갈 수 있는 칸에 O 표시를 해 놓음
        // 
        //BoardManager.Instance.MovePlayer()
    }
}

public class ColorCard : Card
{
    private ColorCardEffect ColorCardEffect;
    private bool cardUseValidity;

    public ColorCard(string cardName, string cardDesc, int cardCost, List<States> statesList,
        ColorCardEffect colorCardEffect) : base(cardName, cardDesc, cardCost, statesList)
    {
        this.ColorCardEffect = colorCardEffect;
    }

    public ColorState MakeState(bool Selectable, ColorTargetPosition Target){
        return new ColorState(Selectable, Target);
    }
    /*
    public MoveState MakeMoveState(bool Selectable, ColorTargetPosition Target){
        return new ColorState(this.Selectable, this.Target);
    }
    */
    public override void usingCardSpecific()
    {
        this.cardUseValidity = true;
        //BoardManager.Instance.MovePlayer()
        //카드 사용 당시의 효과 당장은 무엇이 들어갈지 모른다.
        //이동/색칠
        if(this.ColorCardEffect == ColorCardEffect.ColorAndMove){
            ColorState newState = MakeState(true, ColorTargetPosition.All);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            //MoveState newMoveState = MakeMoveState(false, TargetChoseBefore);
            //PlayerManager.Instance.StatesQueue.Enqueue(newMoveState);
            return;
        }
        //색칠
        if(this.ColorCardEffect == ColorCardEffect.Color){
            ColorState newState = MakeState(false, ColorTargetPosition.P1);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            return;
        }
        //컬러 테이블 3번
        if(this.ColorCardEffect == ColorCardEffect.Color3){
            ColorState newState = MakeState(false, ColorTargetPosition.P4);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            return;
        }
        //컬러 테이블 4번
        if(this.ColorCardEffect == ColorCardEffect.Color4){
            if(/*BoardManager.IsWall(BoardManager.PlayerPosition + (0,0)) &&
            BoardManager.IsWall(BoardManager.PlayerPosition + (1,0)) &&
            BoardManager.IsWall(BoardManager.PlayerPosition + (-1,0)) &&
            BoardManager.IsWall(BoardManager.PlayerPosition + (0,1))&&
            BoardManager.IsWall(BoardManager.PlayerPosition + (0,-1)) */true)
            {
                ColorState newState = MakeState(false, ColorTargetPosition.P5);
                PlayerManager.Instance.StatesQueue.Enqueue(newState);
            }
            else{
                this.cardUseValidity = false;
            }
            return;
        }
        //컬러 테이블 5번
        if(this.ColorCardEffect == ColorCardEffect.Color5){
            ColorState newState = MakeState(false, ColorTargetPosition.C);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            return;
        }
        //컬러 테이블 6번
        if(this.ColorCardEffect == ColorCardEffect.Color6){
            ColorState newState = MakeState(false, ColorTargetPosition.All);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            return;
        }
        //컬러 테이블 7번
        if(this.ColorCardEffect == ColorCardEffect.Color7){
            ColorState newState = MakeState(false, ColorTargetPosition.V);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            return;
        }
        //컬러 테이블 8번
        if(this.ColorCardEffect == ColorCardEffect.Color8){
            ColorState newState = MakeState(false, ColorTargetPosition.H);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            return;
        }
        //컬러 테이블 9번
        if(this.ColorCardEffect == ColorCardEffect.Color9){
            ColorState newState = MakeState(false, ColorTargetPosition.P3V);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            if(/*카드의 사용 취소 가능할 경우 변경*/ true){
                //PlayerManager.Instance.Damage(10);//Damage 10
            }
            return;
        }
        //컬러 테이블 10번
        if(this.ColorCardEffect == ColorCardEffect.Color9){
            ColorState newState = MakeState(false, ColorTargetPosition.P3H);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            if(/*카드의 사용 취소 가능할 경우 변경*/ true){
                //PlayerManager.Instance.Damage(10);//Damage 10
            }
            return;
        }
        //컬러 테이블 11번
        if(this.ColorCardEffect == ColorCardEffect.Color9){
            ColorState newState = MakeState(false, ColorTargetPosition.All);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            if(/*카드의 사용 취소 가능할 경우 변경*/ true){
                //PlayerManager.Instance.DumpAll;// DumpAll
            }
            return;
        }
    }
}