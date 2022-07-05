using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player의 데이터를 저장하는 부분으로 Player와 관련된 데이터, 메소드는 해당 Manager에 작성 바람
/// States 역시 Player에 종속되는 부분으로 판정함
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    private int Hp;
    public IState state;
    public Queue<States> StatesQueue;
    public List<Card> PlayerCard;

    void Start()
    {
        StatesQueue = new Queue<States>();
        state = new NormalState();
        PlayerCard = new List<Card>();
        state.Enter();
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

    public void ChangeStates()
    {
        state.Exit();
        //state전환 과정
        state.Enter();
    }
}
