using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Player의 데이터를 저장하는 부분으로 Player와 관련된 데이터, 메소드는 해당 Manager에 작성 바람
/// States 역시 Player에 종속되는 부분으로 판정함
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private int Hp;

    public BaseState state;
    public Queue<States> StatesQueue;
    public List<Card> PlayerCard;

    void Start()
    {
        PlayerCard = new List<Card>();
        
        //List<States> statesList = new List<States>(){States.Attack};

        //PlayerCard.Add(new AttackCard("Alpha", "Alpha is Greek A", 1,
        //    statesList,  AttackCardEffect.Alpha));

        List<States> moveStatesList = new List<States>() { States.Move };
        PlayerCard.Add(new MoveCard("Idong", "Move one space.", 1,
            moveStatesList, MoveCardEffect.Run, TriggerCondition.Any, MoveDirection.UDLR, 1, () => Debug.Log("Well Played")));


        StatesQueue = new Queue<States>();
        state = new NormalState();
        //PlayerCard = CardData.Instance._load("PlayerCard.json");
        state.Enter();
        //CardData.Instance.saveData(PlayerCard, "PlayerCard.json");
        
        CardManager.Instance.SetUp();
    }

    void Update()
    {
         if(Input.GetMouseButton(0))
             state.MouseEvent();
    }

    void FixedUpdate()
    {
        state.Update();
    }

    public void ChangeStates(States states)
    {
        state.Exit();
        //state전환 과정 이 부분은 세부 State 구현이 나와야 가능할 것으로 예상됨
        state.Enter();
    }
}
