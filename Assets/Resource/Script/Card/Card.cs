using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
public abstract class Card 
{
    protected CardType cardType;

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

    //[JsonConverter(typeof(StringEnumConverter))]
    //public List<States> StatesList;

    public Card(string cardName, string cardDesc, int cardCost){
        this.cardName = cardName;
        this.cardDesc = cardDesc;
        this.cardCost = cardCost;
        //this.StatesList = statesList;
    }
    
    /// <summary>
    /// Card마다 가질 수 있는 사용시의 개별적인 효과는 usingCardSpecific에서 작성할 것
    /// </summary>
    public abstract void usingCardSpecific();

    public bool usingCard()
    {
        usingCardSpecific();
        if (PlayerManager.Instance.StatesQueue.Count == 0) return false;
        else PlayerManager.Instance.ChangeStates(PlayerManager.Instance.StatesQueue.Dequeue());
        return true;
    }
}

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

    public AttackCard(string cardName, string cardDesc, int cardCost, 
        AttackCardEffect attackCardEffect, int targetType, int targetCount, int attackCount, int damage) 
        : base(cardName, cardDesc, cardCost)
        //카드 이름, 카드 설명, 카드 코스트, StatesList,
        //공격 가능한 대상의 종류, 공격 가능한 대상의 수, 공격 횟수, 공격의 피해량
    {
        Debug.Log(this.CardName);
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
        PlayerManager.Instance.StatesQueue.Enqueue(state);
        PlayerManager.Instance.StatesQueue.Enqueue(normal);
    }
}

public class MoveCard : Card
{
    [JsonProperty] public MoveCardEffect MoveCardEffect;
    [JsonProperty] public TriggerCondition triggerCondition;
    [JsonProperty] public MoveDirection moveDirection;     // 상하좌우로 이동, 대각선으로 이동, 어느 칸으로든 이동 등

    public MoveCard(string cardName, string cardDesc, int cardCost,
        MoveCardEffect moveCardEffect, TriggerCondition triggerCondition, MoveDirection moveDirection)
        : base(cardName, cardDesc, cardCost)
    {
        this.cardType = CardType.Move;
        this.MoveCardEffect = moveCardEffect;
        this.triggerCondition = triggerCondition;
        this.moveDirection = moveDirection;
        Debug.Log(this.CardName);
    }

    public override void usingCardSpecific()
    {
        // 카드를 낼 수 있는지를 판정하는 부분
        bool proceed = false;
        switch (this.triggerCondition)
        {
            case TriggerCondition.ColoredSpaceExists:
                //색칠된 칸이 있는가?
                List<List<BoardColor>> boardColors = BoardManager.Instance.BoardColors;
                
                for (int i = 0; i < boardColors.Count; i++)  // row
                {
                    for (int j = 0; j < boardColors[0].Count; j++)  // col
                    {
                        if (boardColors[i][j] == BoardColor.Player)
                        {
                            proceed = true;
                            break;
                        }
                    }
                    if (proceed) break;
                }
                break;

            case TriggerCondition.EnemyWillAttack:
                // 적이 이번 턴에 공격을 하려고 하는가?
                //List<Enemy> enemyList = EnemyManager.EnemyList;
                List<Enemy> enemyList = new List<Enemy>();

                foreach (Enemy enemy in enemyList)
                    // if (enemy.State = EnemyState.Attack)
                        proceed = true;
                break;

            case TriggerCondition.PlayerHealthExceeds30:
                // 플레이어의 체력이 30을 초과하는가?
                if (PlayerManager.Instance.Hp > 30)
                    proceed = true;
                break;

            case TriggerCondition.MoveCardInHand:
                // 패에 다른 이동 카드가 있는가?
                foreach (Card card in PlayerManager.Instance.PlayerCard)
                {
                    if (card is MoveCard /* 또는 AttackMoveCard */)  // GetType은 실제 type을 반환한다.
                    {
                        proceed = true;
                        break;
                    }
                }
                break;

            case TriggerCondition.None:
                proceed = true;
                break;

            default:
                return;
        }
        if (!proceed) return;

        // 카드를 낼 때 나오는 이펙트 (이동 이펙트랑은 다름)
        // 1초 동안 바람 이펙트 - 10개의 LinearWind, 4개의 LoopWind를 소환하여 1초 동안 애니메이션을 재생하다가 사라지도록 한다.
        //GameObject[] linearWinds = new GameObject[10];
        //for (int i = 0; i < linearWinds.Length; i++)
        //{
        //    //linearWinds[i] = 
        //}

        // Player의 마나를 감소시키는 부분
        if (PlayerManager.Instance.SetMana(-3)) return;

        // State를 만드는 부분
        MoveState state = new MoveState(this);
        NormalState normal = new NormalState();  // 다 끝나고 다시 normal state로 돌아온다.

        // State를 Enqueue하는 부분
        PlayerManager.Instance.StatesQueue.Enqueue(state);
        PlayerManager.Instance.StatesQueue.Enqueue(normal);
    }
}

public class ColorCard : Card
{
    [JsonProperty] private ColorCardEffect ColorCardEffect;
    private bool cardUseValidity;

    public ColorCard(string cardName, string cardDesc, int cardCost,
        ColorCardEffect colorCardEffect) : base(cardName, cardDesc, cardCost)
    {
        this.cardType = CardType.Color;
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
        //BoardManager.Instance.MovePlayer()
        //카드 사용 당시의 효과 당장은 무엇이 들어갈지 모른다.

        //이동/색칠
        if(this.ColorCardEffect == ColorCardEffect.ColorAndMove)
        {
            if (!PlayerManager.Instance.SetMana(-CardCost)) return;
            ColorState newState = MakeState(true, ColorTargetPosition.All);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());
            //MoveState newMoveState = MakeMoveState(false, TargetChoseBefore);
            //PlayerManager.Instance.StatesQueue.Enqueue(newMoveState);
            return;
        }
        //색칠
        if(this.ColorCardEffect == ColorCardEffect.Color){
            if (!PlayerManager.Instance.SetMana(-CardCost)) return;
            ColorState newState = MakeState(false, ColorTargetPosition.P1);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());
            return;
        }
        //컬러 테이블 3번
        if(this.ColorCardEffect == ColorCardEffect.Color3){
            if (!PlayerManager.Instance.SetMana(-CardCost)) return;
            ColorState newState = MakeState(false, ColorTargetPosition.P4);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());
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
                if (!PlayerManager.Instance.SetMana(-CardCost)) return;
                ColorState newState = MakeState(false, ColorTargetPosition.P5);
                PlayerManager.Instance.StatesQueue.Enqueue(newState);
                PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());
            }
            else{
                this.cardUseValidity = false;
            }
            return;
        }
        //컬러 테이블 5번
        if(this.ColorCardEffect == ColorCardEffect.Color5){
            if(/*BoardManager.CountBingo() > 0*/true){
                if (!PlayerManager.Instance.SetMana(-CardCost)) return;
                ColorState newState = MakeState(false, ColorTargetPosition.C);
                PlayerManager.Instance.StatesQueue.Enqueue(newState);
                PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());
            }
            return;
        }
        //컬러 테이블 6번
        if(this.ColorCardEffect == ColorCardEffect.Color6){
            //조건에 따라 마나 소모량 변화
            int cost = CardCost;
            if(cost < 0)
                cost = 0;
            if (!PlayerManager.Instance.SetMana(-CardCost)) return;
            ColorState newState = MakeState(false, ColorTargetPosition.All);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());
            return;
        }
        //컬러 테이블 7번
        if(this.ColorCardEffect == ColorCardEffect.Color7){
            if(/*PlayerManager.Instance.AttackedBefore()*/ true){
                if (!PlayerManager.Instance.SetMana(-CardCost)) return;
                ColorState newState = new ColorState(false, ColorTargetPosition.V);
                PlayerManager.Instance.StatesQueue.Enqueue(newState);
                PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());
            }
            return;
        }
        //컬러 테이블 8번
        if(this.ColorCardEffect == ColorCardEffect.Color8){
            if(/*PlayerManager.Instance.AttackedBefore()*/ true){
                if (!PlayerManager.Instance.SetMana(-CardCost)) return;
                ColorState newState = MakeState(false, ColorTargetPosition.H);
                PlayerManager.Instance.StatesQueue.Enqueue(newState);
                PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());
            }
            return;
        }
        //컬러 테이블 9번
        if(this.ColorCardEffect == ColorCardEffect.Color9){
            if (!PlayerManager.Instance.SetMana(-CardCost)) return;
            ColorState newState = MakeState(false, ColorTargetPosition.P3V);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());
            PlayerManager.Instance.SetHp(-10);//Damage 10
            return;
        }
        //컬러 테이블 10번
        if(this.ColorCardEffect == ColorCardEffect.Color10){
            if (!PlayerManager.Instance.SetMana(-CardCost)) return;
            ColorState newState = MakeState(false, ColorTargetPosition.P3H);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());
            PlayerManager.Instance.SetHp(-10);//Damage 10
            return;
        }
        //컬러 테이블 11번
        if(this.ColorCardEffect == ColorCardEffect.Color11){
            if (!PlayerManager.Instance.SetMana(-CardCost)) return;
            ColorState newState = MakeState(false, ColorTargetPosition.All);
            PlayerManager.Instance.StatesQueue.Enqueue(newState);
            PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());
            if(/*카드의 사용 취소 가능할 경우 변경*/ true){
                //PlayerManager.Instance.DumpAll;// DumpAll
            }
            return;
        }
    }
}