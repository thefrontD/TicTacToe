using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private int DrawNum = 5;
    private bool isNewPlayerTurn;
    private bool evenPuzzleDraw;

    public NormalState(int DrawNum = 5, bool isNewPlayerTurn = false, bool evenPuzzleDraw = false)
    {
        this.isNewPlayerTurn = isNewPlayerTurn;
        this.evenPuzzleDraw = evenPuzzleDraw;

        if(isNewPlayerTurn){
            if (PlayerManager.Instance.DebuffDictionary[Debuff.DrawCardDecrease] != 0)
                this.DrawNum = DrawNum - 1;
            else
                this.DrawNum = DrawNum;
        }
    }

    public override void DoAction(States state)
    {
        return;
    }

    public override void Enter()
    {
        if (PlayerManager.Instance.TutorialPhase == 3 || PlayerManager.Instance.TutorialPhase == 4 ||
            PlayerManager.Instance.TutorialPhase == 5 ||
            PlayerManager.Instance.TutorialPhase == 6 || PlayerManager.Instance.TutorialPhase == 9 ||
            PlayerManager.Instance.TutorialPhase == 11 || PlayerManager.Instance.TutorialPhase == 12 ||
            PlayerManager.Instance.TutorialPhase == 15 ||
            PlayerManager.Instance.TutorialPhase == 18 || PlayerManager.Instance.TutorialPhase == 19 ||
            PlayerManager.Instance.TutorialPhase == 20)
        {
            TutorialManager.Instance.toNextTutorial(PlayerManager.Instance.TutorialPhase);
        }

        if (PlayerManager.Instance.TutorialPhase == 14 && PlayerManager.Instance.TutorialSubPhase == 3)
        {
            PlayerManager.Instance.TutorialSubPhase = 0;
            TutorialManager.Instance.toNextTutorial(PlayerManager.Instance.TutorialPhase);
        }

        if (PlayerManager.Instance.TutorialPhase == 7)
        {
            TutorialManager.Instance.toNextTutorial(PlayerManager.Instance.TutorialPhase);
        }
        else if (PlayerManager.Instance.TutorialPhase == 10)
        {
            TutorialManager.Instance.toNextTutorial(PlayerManager.Instance.TutorialPhase);
        }
        else if (PlayerManager.Instance.TutorialPhase == 13)
        {
            TutorialManager.Instance.toNextTutorial(PlayerManager.Instance.TutorialPhase);
        }
        else if (PlayerManager.Instance.TutorialPhase == 17)
        {
            TutorialManager.Instance.toNextTutorial(PlayerManager.Instance.TutorialPhase);
        }
        else if (isNewPlayerTurn)
        {
            PlayerManager.Instance.SetMana(1000);
            if (PlayerManager.Instance.DebuffDictionary[Debuff.Heal] > 0)
                PlayerManager.Instance.DamageToPlayer((int) (PlayerManager.Instance.MaxHp * 0.1));
            
            if(!GameManager.Instance.IsPuzzleMode || evenPuzzleDraw){
                CardManager.Instance.DrawCard(DrawNum);
            }
            /*
            foreach (Debuff debuff in Enum.GetValues(typeof(Debuff)))
                PlayerManager.Instance.SetDebuff(debuff, -1);
            */
        }
        CardManager.Instance.CheckUsable();

        foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
        {
            if(enemy.EnemyShield == 0){
                enemy.EnemyHealShield(enemy.EnemyMaxShield);
            }
        }
    }

    public override void MouseEvent()
    {
        return;
    }

    public override void Update()
    {
        return;
    }

    public override void Exit()
    {
        /*
        if (PlayerManager.Instance.BingoAttack)
        {
            for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
            {
                for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                {
                    BoardManager.Instance.GameBoard[i][j].SetBoardColor(BoardColor.None);
                    BoardManager.Instance.GameBoard[i][j].ActivateBingoEffect(false);
                }
            }
            PlayerManager.Instance.BingoAttack = false;
        }
        */
        foreach (CardUI cardui in CardManager.Instance.HandCardList)
            cardui.HightLightCard(false);
        return;
    }
}
