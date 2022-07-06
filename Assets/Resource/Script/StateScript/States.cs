using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 클래스 작성 전에 Interface에 IState interface의 주석 참조할 것
/// </summary>
public class NormalState : IState
{
    private int DrawNum;
    
    public NormalState(int DrawNum = 5)
    {
        this.DrawNum = DrawNum;
    }

    public void DoAction(States state)
    {
        
    }

    public void Enter()
    {
        
    }

    public void MouseEvent()
    {
        
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}