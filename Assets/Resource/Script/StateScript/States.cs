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

public class ColorState : BaseState
{
    
    private bool Selectable;
    private ColorTargetPosition Target;
    
    public ColorState(bool Selectable, ColorTargetPosition Target)
    {
        this.Selectable = Selectable;
        this.Target = Target;
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