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

    private TriggerCondition _triggerCondition;
    public TriggerCondition TriggerCondition => _triggerCondition;
    private AdditionalEffectCondition _additionalEffectCondition;
    public AdditionalEffectCondition AdditionalEffectCondition => _additionalEffectCondition;
    private AdditionalEffect _additionalEffect;
    public AdditionalEffect AdditionalEffect => _additionalEffect;
    
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

    public Card(string cardName, string cardDesc, int cardCost, TriggerCondition triggerCondition, 
        AdditionalEffectCondition additionalEffectCondition, AdditionalEffect additionalEffect){
        this.cardName = cardName;
        this.cardDesc = cardDesc;
        this.cardCost = cardCost;
    }
    
    /// <summary>
    /// Card마다 가질 수 있는 사용시의 개별적인 효과는 usingCardSpecific에서 작성할 것
    /// </summary>
    public abstract void usingCardSpecific();

    public bool usingCard()
    {
        bool proceed = false;
        int cnt = 0;
        int[] dx = new int[4] {1, 0, -1, 0};
        int[] dy = new int[4] {0, 1, 0, -1};

        #region TriggerCondition
        switch (this._triggerCondition)
        {
            case TriggerCondition.Attacked:
                proceed = true;
                break;
            
            case TriggerCondition.PlayerInColoredSpace:
                if (BoardManager.Instance.BoardColors[PlayerManager.Instance.Row][PlayerManager.Instance.Col]
                    == BoardColor.Player)
                    proceed = true;
                break;
            
            case TriggerCondition.ColoredSpaceExists:
                for (int i = 0; i < BoardManager.Instance.BoardColors.Count; i++)  // row
                {
                    for (int j = 0; j < BoardManager.Instance.BoardColors[0].Count; j++)  // col
                    {
                        if (BoardManager.Instance.BoardColors[i][j] == BoardColor.Player)
                        {
                            proceed = true;
                            break;
                        }
                    }
                    if (proceed) break;
                }
                break;

            case TriggerCondition.EnemyWillAttack:
                foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
                    if ((int)enemy.EnemyActions.Peek()/10 == 0)
                        proceed = true;
                break;
            case TriggerCondition.EnemyWillWall:
                foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
                    if (enemy.EnemyActions.Peek() == EnemyAction.WallSummon)
                        proceed = true;
                break;
            case TriggerCondition.EnemyWillMinion:
                foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
                    if (enemy.EnemyActions.Peek() == EnemyAction.MobSummon)
                        proceed = true;
                break;
            case TriggerCondition.EnemyWillShield:
                foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
                    if (enemy.EnemyActions.Peek() == EnemyAction.ArmorHealing)
                        proceed = true;
                break;
            
            case TriggerCondition.PlayerWall:
                proceed = true;
                break;
            case TriggerCondition.PlayerNotWall:
                proceed = true;
                break;

            case TriggerCondition.OnlyAttackCardInHand:
                proceed = true;
                foreach (GameObject card in CardManager.Instance.HandCardList)
                {
                    if (card.GetComponent<CardUI>().Card.cardType != CardType.Attack)
                    {
                        proceed = false;
                        break;
                    }
                } 
                break;
            case TriggerCondition.OnlyColorCardInHand:
                proceed = true;
                foreach (GameObject card in CardManager.Instance.HandCardList)
                {
                    if (card.GetComponent<CardUI>().Card.cardType != CardType.Color)
                    {
                        proceed = false;
                        break;
                    }
                }
                break;
            case TriggerCondition.OnlyMoveCardInHand:
                proceed = true;
                foreach (GameObject card in CardManager.Instance.HandCardList)
                {
                    if (card.GetComponent<CardUI>().Card.cardType != CardType.Move)
                    {
                        proceed = false;
                        break;
                    }
                }
                break;
            
            case TriggerCondition.Bingo1:
            case TriggerCondition.Bingo2:
            case TriggerCondition.Bingo3:
                if (BoardManager.Instance.CheckBingo(BoardColor.Player) >=
                    ((int) _triggerCondition - 30))
                    proceed = true;
                break;

            case TriggerCondition.CardInHand1:
            case TriggerCondition.CardInHand2:
            case TriggerCondition.CardInHand3:
            case TriggerCondition.CardInHand4:
            case TriggerCondition.CardInHand5:    
                if(CardManager.Instance.HandCardList.Count >= ((int) _triggerCondition - 40))
                    proceed = true;
                break;
            
            case TriggerCondition.AttackCardInHand1:
            case TriggerCondition.AttackCardInHand2:
            case TriggerCondition.AttackCardInHand3:
            case TriggerCondition.AttackCardInHand4:
            case TriggerCondition.AttackCardInHand5:
                foreach (GameObject card in CardManager.Instance.HandCardList)
                {
                    if (card.GetComponent<CardUI>().Card.cardType == CardType.Attack)
                        cnt++;
                }

                if (cnt >= ((int) _triggerCondition - 50))
                    proceed = true;
                break;
            
            case TriggerCondition.ColorCardInHand1:
            case TriggerCondition.ColorCardInHand2:
            case TriggerCondition.ColorCardInHand3:
            case TriggerCondition.ColorCardInHand4:
            case TriggerCondition.ColorCardInHand5:  
                foreach (GameObject card in CardManager.Instance.HandCardList)
                {
                    if (card.GetComponent<CardUI>().Card.cardType == CardType.Color)
                        cnt++;
                }

                if (cnt >= ((int) _triggerCondition - 60))
                    proceed = true;
                break;
            
            case TriggerCondition.MoveCardInHand1:
            case TriggerCondition.MoveCardInHand2:
            case TriggerCondition.MoveCardInHand3:
            case TriggerCondition.MoveCardInHand4:
            case TriggerCondition.MoveCardInHand5:  
                foreach (GameObject card in CardManager.Instance.HandCardList)
                {
                    if (card.GetComponent<CardUI>().Card.cardType == CardType.Move)
                        cnt++;
                }

                if (cnt >= ((int) _triggerCondition - 70))
                    proceed = true;
                break;

            case TriggerCondition.PlayerHealthExceeds30:
                // 플레이어의 체력이 30을 초과하는가?
                if (PlayerManager.Instance.Hp > ((int) _triggerCondition - 100))
                    proceed = true;
                break;

            case TriggerCondition.None:
                proceed = true;
                break;
        }
        if (!proceed) return false;
        #endregion
        
        usingCardSpecific();
        if (PlayerManager.Instance.StatesQueue.Count == 0) return false;
        else PlayerManager.Instance.ChangeStates(PlayerManager.Instance.StatesQueue.Dequeue());
        return true;
    }
}

public class AttackCard : Card
{
    [JsonProperty] private AttackCardEffect AttackCardEffect;  //이펙트

    [JsonProperty] private int TargetType;                     //공격 가능한 대상의 종류
    [JsonProperty] private int TargetCount;                    //공격 가능한 대상의 수
    [JsonProperty] private int AttackCount;                    //공격 횟수
    [JsonProperty] private int Damage;                         //공격의 피해량

    public int GetTargetType() => TargetType;
    public AttackCard(string cardName, string cardDesc, int cardCost, TriggerCondition triggerCondition,
        AdditionalEffectCondition additionalEffectCondition, AdditionalEffect additionalEffect,
        AttackCardEffect attackCardEffect, int targetType, int targetCount, int attackCount, int damage) 
        : base(cardName, cardDesc, cardCost, triggerCondition, additionalEffectCondition, additionalEffect)
        //카드 이름, 카드 설명, 카드 코스트, StatesList,
        //공격 가능한 대상의 종류, 공격 가능한 대상의 수, 공격 횟수, 공격의 피해량
    {
        Debug.Log(this.CardName);
        this.cardType = CardType.Attack;
        this.AttackCardEffect = attackCardEffect;
        this.TargetType = targetType;
        this.TargetCount = targetCount;
        this.AttackCount = attackCount;
        this.Damage = damage;
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

    public MoveCard(string cardName, string cardDesc, int cardCost, TriggerCondition triggerCondition,
        AdditionalEffectCondition additionalEffectCondition, AdditionalEffect additionalEffect,
        MoveCardEffect moveCardEffect, MoveDirection moveDirection)
        : base(cardName, cardDesc, cardCost, triggerCondition, additionalEffectCondition, additionalEffect)
    {
        this.cardType = CardType.Move;
        this.MoveCardEffect = moveCardEffect;
        this.triggerCondition = triggerCondition;
        this.moveDirection = moveDirection;
        Debug.Log(this.CardName);
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
        PlayerManager.Instance.StatesQueue.Enqueue(state);
        PlayerManager.Instance.StatesQueue.Enqueue(normal);
    }
}

public class ColorCard : Card
{
    [JsonProperty] private ColorCardEffect ColorCardEffect;
    [JsonProperty] private TriggerCondition costChangeCondition;
    [JsonProperty] private TriggerCondition triggerCondition;
    [JsonProperty] public ColorTargetPosition colorTargetPosition;
    [JsonProperty] public ColorTargetNum colorTargetNum;
    [JsonProperty] private AdditionalEffectCondition additionalEffectCondition;
    [JsonProperty] private AdditionalEffect additionalEffect;
    private bool cardUseValidity;

    public ColorCard(string cardName, string cardDesc, int cardCost, TriggerCondition costChangeCondition, TriggerCondition triggerCondition,
        ColorTargetPosition colorTargetPosition, ColorTargetNum colorTargetNum,
        AdditionalEffectCondition additionalEffectCondition, AdditionalEffect additionalEffect,
        ColorCardEffect colorCardEffect) : 
        base(cardName, cardDesc, cardCost, triggerCondition, additionalEffectCondition, additionalEffect)
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
                int BingoNum = BoardManager.Instance.CheckBingo(BoardColor.Player);
                CardCost = CardCost - BingoNum;
                if(CardCost < 0)
                    CardCost = 0;
                break;
            default:
                break;
        }
        
        //추가효과 
        bool additionalEffectQualified = true;

        if(additionalEffectQualified){
            switch(this.additionalEffect){
                case AdditionalEffect.Move:
                    Debug.Log("AdditionalEffect : Move");
                    /*MoveCard moveCard = new MoveCard("move", "movetocoloredblock", 0,
                        TriggerCondition.None, AdditionalEffectCondition.None, 
                        AdditionalEffect.None, MoveCardEffect.Run, 
                        MoveDirection.Colored);
                    MoveState moveState = new MoveState(moveCard);*/
                    //movestate사용에 문제가 있음
                    break;
                case AdditionalEffect.PlayerHp10:
                    Debug.Log("AdditionalEffect : PlayerHp-10");
                    PlayerManager.Instance.Hp -= 10;
                    break;
                case AdditionalEffect.DumpALL:
                    Debug.Log("AdditionalEffect : Dumpall");
                    CardManager.Instance.AllHandCardtoGrave();
                    break;
                default:
                    break;
            }
        }
        // State를 만드는 부분
        ColorState newState = new ColorState(this);
        PlayerManager.Instance.StatesQueue.Enqueue(newState);
        // State를 Enqueue하는 부분
        PlayerManager.Instance.StatesQueue.Enqueue(new NormalState());
    }
}