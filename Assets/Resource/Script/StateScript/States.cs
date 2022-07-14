using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// State에 대한 추상 클래스
/// </summary>
public abstract class BaseState
{
    /// <summary>
    /// 카드 사용시에 일어나야 하는 부분을 여기에 작성할 것
    /// </summary>
    public abstract void DoAction(States state);

    /// <summary>
    /// 처음 State로 진입시 호출되는 함수
    /// 각종 UI나 이펙트 Rendering 작업이 여기서 부탁호출 되도록 작성할 것
    /// </summary>
    public abstract void Enter();
    /// <summary>
    /// 해당 State에서 마우스를 클릭했을때 일어나는 상황을 작성할 것
    /// </summary>
    public abstract void MouseEvent();
    /// <summary>
    /// 해당 State에서 Update함수에 호출되야하는 요소를 작성할 것
    /// </summary>
    public abstract void Update();
    /// <summary>
    /// 다음 State로 전환시에 호출되는 함수
    /// Enter에서 Rendering한 요소들을 지울 것
    /// </summary>
    public abstract void Exit();
}
public class NormalState : BaseState
{
    private int DrawNum;
    
    public NormalState(int DrawNum = 5)
    {
        this.DrawNum = DrawNum;
    }

    public override void DoAction(States state)
    {
        
    }

    public override void Enter()
    {
        
    }

    public override void MouseEvent()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        
    }
}
public class AttackState : BaseState
{
    private AttackCard Card;

    public AttackState(AttackCard card)
    {
        this.Card = card;
    }

    public override void DoAction(States state)
    {
        
    }

    public override void Enter()
    {
        //공격 가능한 대상의 테두리를 밝은 파란 테두리로 표시
        //카드 데이터의 공격 가능한 대상의 종류
        //몬스터 공격 가능한 경우(001)
        int targetType = Card.GetTargetType();
        bool isMonster = targetType % 10 != 0;
        bool isWall = (targetType / 10) % 10 != 0;
        bool isMinion = (targetType / 100) % 10 != 0;

        if(isMonster)
        {

        }
        if(isWall)
        {
            //플레이어 주변에 Wall이 있으면 하이라이트(Clickable)
            //플레이어 위치 주변 4칸에 Wall이 있는지 체크

        }
        if(isMinion)
        {
            //하수인 공격 가능한 경우 -> 플레이어 주변의 하수인(100)
        }
    }

    public override void MouseEvent()
    {
        //빛나는 개체 클릭
        //공격 가능한 적 개수만큼 선택하면
        //선택된 모든 개체에게 공격 횟수만큼 공격한다(대미지를 준다)
    }

    public override void Update()
    {
        //취소하면 normal state로 돌아감
    }

    public override void Exit()
    {
        
    }
}

/*
public class ColorState : BaseState
{
    public ColorState(bool Selectable, int TargetNum, ColorTargetPosition Target) { }
    public override void DoAction(States state)
    {
        throw new NotImplementedException();
    }

    public override void Enter()
    {
        throw new NotImplementedException();
    }

    public override void Exit()
    {
        throw new NotImplementedException();
    }

    public override void MouseEvent()
    {
        throw new NotImplementedException();
    }

    public override void Update()
    {
        throw new NotImplementedException();
    }
}
*/