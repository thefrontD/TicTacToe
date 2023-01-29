using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyState : BaseState
{
    public override void DoAction(States state)
    {
        return;
    }

    public override void Enter()
    {
        for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
        {
            for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
            {
                BoardManager.Instance.GameBoard[i][j].SetHighlight(BoardSituation.None);
            }
        }

        foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
        {
            if (enemy.DebuffDictionary[Debuff.Heal] > 0)
                enemy.EnemyHP += (int) (enemy.EnemyMaxHP * 0.1);
            
            enemy.EnemyUI.ShieldUIUpdate();
            enemy.EnemyUI.HPUIUpdate();
        }

        EnemyManager.Instance.EnemyAttack();
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
        foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
        {
            if(enemy.EnemyActions.Peek().Item1 == EnemyAction.WallsSummon || enemy.EnemyActions.Peek().Item1 == EnemyAction.WallSummon)
                enemy.GetOverLapPosition(enemy.EnemyActions.Peek());
            if(enemy.EnemyShield == 0){
                enemy.EnemyHealShield(enemy.EnemyMaxShield);
            }
        }

        foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
            enemy.setPreviousPos(PlayerManager.Instance.Row, PlayerManager.Instance.Col);
        EnemyManager.Instance.HightLightBoard();
    }
}