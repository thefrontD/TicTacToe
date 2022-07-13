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