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
    public CardType CardType => cardType;

    private TriggerCondition _triggerCondition;
    public TriggerCondition TriggerCondition => _triggerCondition;
    private AdditionalEffectCondition _additionalEffectCondition;
    public AdditionalEffectCondition AdditionalEffectCondition => _additionalEffectCondition;
    private AdditionalEffect _additionalEffect;
    public AdditionalEffect AdditionalEffect => _additionalEffect;
    [JsonProperty] public List<CardPoolAttribute> CardPoolAttributes = new List<CardPoolAttribute>();

    private string cardName;

    public string CardName
    {
        get { return cardName; }
    }

    private string cardDesc;

    public string CardDesc
    {
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

    public Card(string cardName, string cardDesc, int cardCost, List<CardPoolAttribute> CardPoolAttributes, TriggerCondition triggerCondition,
        AdditionalEffectCondition additionalEffectCondition, AdditionalEffect additionalEffect)
    {
        this.cardName = cardName;
        this.cardDesc = cardDesc;
        this.cardCost = cardCost;
        this.CardPoolAttributes = CardPoolAttributes;
        this._triggerCondition = triggerCondition;
        this._additionalEffectCondition = additionalEffectCondition;
        this._additionalEffect = additionalEffect;
    }

    /// <summary>
    /// Card마다 가질 수 있는 사용시의 개별적인 효과는 usingCardSpecific에서 작성할 것
    /// </summary>
    public abstract void usingCardSpecific();

    public bool usingCard()
    {
        if (!PlayerManager.Instance.CardUsable)
            return false;

        if (!CheckCondition())
            return false;
        
        if(PlayerManager.Instance.TutorialTrigger)
            if (!CheckTutorial())
                return false;

        if (!PlayerManager.Instance.SetMana(-CardCost, cardType)) return false;
        usingCardSpecific();
        if (PlayerManager.Instance.StatesQueue.Count == 0) return false;
        else PlayerManager.Instance.ChangeStates(PlayerManager.Instance.StatesQueue.Dequeue());

        return true;
    }

    private bool CheckTutorial()
    {
        switch (PlayerManager.Instance.TutorialPhase)
        {
            case 3:
            case 4: 
            case 5:
            case 18:
            case 21:
                if (cardType == CardType.Attack) return true;
                else return false;
            
            case 6:
            case 11:
            case 15:
            case 20:
                if (cardType == CardType.Color) return true;
                else return false;
            
            case 9:
            case 12:
            case 14:
            case 19: 
                if (cardType == CardType.Move) return true;
                else return false;
            
            default:
                return false;
        }
    }
    
    public bool CheckCondition()
    {
        bool proceed = false;
        int cnt = 0;
        int[] dx = new int[4] {1, 0, -1, 0};
        int[] dy = new int[4] {0, 1, 0, -1};

        switch (this._triggerCondition)
        {
            /*case TriggerCondition.Attacked:
                proceed = PlayerManager.Instance.Attacked;
                break;*/

            case TriggerCondition.PlayerInColoredSpace:
                if (BoardManager.Instance.BoardColors[PlayerManager.Instance.Row][PlayerManager.Instance.Col]
                    != BoardColor.None)
                    proceed = true;
                break;
            case TriggerCondition.PlayerInRedColoredSpace:
                if (BoardManager.Instance.BoardColors[PlayerManager.Instance.Row][PlayerManager.Instance.Col]
                    == BoardColor.Enemy)
                    proceed = true;
                break;
            case TriggerCondition.PlayerInBlueColoredSpace:
                if (BoardManager.Instance.BoardColors[PlayerManager.Instance.Row][PlayerManager.Instance.Col]
                    == BoardColor.Player)
                    proceed = true;
                break;
            case TriggerCondition.PlayerNotInColoredSpace:
                if (BoardManager.Instance.BoardColors[PlayerManager.Instance.Row][PlayerManager.Instance.Col]
                    != BoardColor.Player){
                    proceed = true;
                }
                break;

            case TriggerCondition.ColoredSpaceExists:
                for (int i = 0; i < BoardManager.Instance.BoardColors.Count; i++) // row
                {
                    for (int j = 0; j < BoardManager.Instance.BoardColors[0].Count; j++) // col
                    {
                        if (BoardManager.Instance.BoardColors[i][j] != BoardColor.None)
                        {
                            proceed = true;
                            break;
                        }
                    }

                    if (proceed) break;
                }

                break;
            case TriggerCondition.RedColoredSpaceExists:
                for (int i = 0; i < BoardManager.Instance.BoardColors.Count; i++) // row
                {
                    for (int j = 0; j < BoardManager.Instance.BoardColors[0].Count; j++) // col
                    {
                        if (BoardManager.Instance.BoardColors[i][j] == BoardColor.Enemy)
                        {
                            proceed = true;
                            break;
                        }
                    }

                    if (proceed) break;
                }

                break;
            case TriggerCondition.BlueColoredSpaceExists:
                for (int i = 0; i < BoardManager.Instance.BoardColors.Count; i++) // row
                {
                    for (int j = 0; j < BoardManager.Instance.BoardColors[0].Count; j++) // col
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

            case TriggerCondition.MonsterWillAttack:
                foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
                    if ((int) enemy.EnemyActions.Peek().Item1 / 100 == 0)
                        proceed = true;
                break;
            case TriggerCondition.MonsterWillWall:
                foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
                    if (enemy.EnemyActions.Peek().Item1 == EnemyAction.WallSummon)
                        proceed = true;
                break;
            case TriggerCondition.MonsterWillMinion:
                foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
                    if (enemy.EnemyActions.Peek().Item1 == EnemyAction.MobSummon)
                        proceed = true;
                break;
            case TriggerCondition.MonsterWillShield:
                foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
                    if (enemy.EnemyActions.Peek().Item1 == EnemyAction.ShieldHealing)
                        proceed = true;
                break;

            case TriggerCondition.PlayerWall:  // TODO
                for(int i = 0; i < 4;  i++){
                    if(PlayerManager.Instance.Row + dx[i] < 0 || PlayerManager.Instance.Row + dx[i] > 2 ||
                    PlayerManager.Instance.Col + dy[i] < 0 || PlayerManager.Instance.Col + dy[i] > 2){
                        if(BoardManager.Instance.BoardObjects[PlayerManager.Instance.Row + dx[i]][PlayerManager.Instance.Col + dy[i]]
                        == BoardObject.Wall)
                            proceed = true;
                    }
                }
                break;
            case TriggerCondition.PlayerNotWall:  // TODO
                proceed = true;
                for(int i = 0; i < 4;  i++){
                    if(PlayerManager.Instance.Row + dx[i] < 0 || PlayerManager.Instance.Row + dx[i] > 2 ||
                    PlayerManager.Instance.Col + dy[i] < 0 || PlayerManager.Instance.Col + dy[i] > 2){
                        continue;
                    }else{
                        if(BoardManager.Instance.BoardObjects[PlayerManager.Instance.Row + dx[i]][PlayerManager.Instance.Col + dy[i]]
                        == BoardObject.Wall)
                            proceed = false;
                    }
                }
                break;

            case TriggerCondition.OnlyAttackCardInHand:
                proceed = true;
                foreach (CardUI card in CardManager.Instance.HandCardList)
                {
                    if (card.Card.cardType != CardType.Attack)
                    {
                        proceed = false;
                        break;
                    }
                }

                break;
            case TriggerCondition.OnlyColorCardInHand:
                proceed = true;
                foreach (CardUI card in CardManager.Instance.HandCardList)
                {
                    if (card.Card.cardType != CardType.Color)
                    {
                        proceed = false;
                        break;
                    }
                }

                break;
            case TriggerCondition.OnlyMoveCardInHand:
                proceed = true;
                foreach (CardUI card in CardManager.Instance.HandCardList)
                {
                    if (card.Card.cardType != CardType.Move)
                    {
                        proceed = false;
                        break;
                    }
                }

                break;

            case TriggerCondition.Bingo1:
            case TriggerCondition.Bingo2:
            case TriggerCondition.Bingo3:
                if (BoardManager.Instance.CountBingo(BoardColor.Player) >=
                    ((int) _triggerCondition - 30))
                    proceed = true;
                break;

            case TriggerCondition.CardInHand1:
            case TriggerCondition.CardInHand2:
            case TriggerCondition.CardInHand3:
            case TriggerCondition.CardInHand4:
            case TriggerCondition.CardInHand5:
                if (CardManager.Instance.HandCardList.Count >= ((int) _triggerCondition - 40))
                    proceed = true;
                break;

            case TriggerCondition.AttackCardInHand1:
            case TriggerCondition.AttackCardInHand2:
            case TriggerCondition.AttackCardInHand3:
            case TriggerCondition.AttackCardInHand4:
            case TriggerCondition.AttackCardInHand5:
                foreach (CardUI card in CardManager.Instance.HandCardList)
                {
                    if (card.Card.cardType == CardType.Attack)
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
                foreach (CardUI card in CardManager.Instance.HandCardList)
                {
                    if (card.Card.cardType == CardType.Color)
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
                foreach (CardUI card in CardManager.Instance.HandCardList)
                {
                    if (card.Card.cardType == CardType.Move)
                        cnt++;
                }

                if (cnt >= ((int) _triggerCondition - 70))
                    proceed = true;
                break;

            case TriggerCondition.PlayerHealthExceeds10:
            case TriggerCondition.PlayerHealthExceeds20:
            case TriggerCondition.PlayerHealthExceeds30:
                // 플레이어의 체력이 30을 초과하는가?
                if (PlayerManager.Instance.Hp > ((int) _triggerCondition - 100))
                    proceed = true;
                break;

            case TriggerCondition.SevenColoredSpace:
                int count = 0;
                
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                        if (BoardManager.Instance.BoardColors[i][j] == BoardColor.Player)
                            count++;

                if (count == 7)
                    proceed = true;
                break;
            
            case TriggerCondition.None:
                proceed = true;
                break;
        }

        return proceed;
    }
}