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

public class MoveState : BaseState
{
    bool[,] movableSpace;
    public MoveState(MoveCard originalCard)
    {
        switch (originalCard.moveDirection)
        {
            case MoveDirection.UDLR:
                // 현재 위치로부터 상하좌우로 한 칸 이동
                {
                    this.movableSpace = new bool[3, 3];  // 모든 항이 false인 2D 배열
                    (int, int)[] coords =
                    {
                        (PlayerManager.Instance.row - 1, PlayerManager.Instance.col),  // 위
                        (PlayerManager.Instance.row, PlayerManager.Instance.col + 1),  // 오른쪽
                        (PlayerManager.Instance.row + 1, PlayerManager.Instance.col),  // 아래
                        (PlayerManager.Instance.row, PlayerManager.Instance.col - 1)   // 왼쪽
                    };
                    foreach ((int, int) coord in coords)
                    {
                        if (coord.Item1 >= 0 && coord.Item1 < 3 && coord.Item2 >= 0 && coord.Item2 < 3)
                        {
                            this.movableSpace[coord.Item1, coord.Item2] = true;
                        }
                    }
                    break;
                }

            case MoveDirection.Diagonal:
                // 현재 위치로부터 대각선으로 한 칸 이동
                {
                    this.movableSpace = new bool[3, 3];  // 모든 항이 false인 2D 배열
                    (int, int)[] coords =
                    {
                        (PlayerManager.Instance.row - 1, PlayerManager.Instance.col - 1),  // 왼쪽 위
                        (PlayerManager.Instance.row - 1, PlayerManager.Instance.col + 1),  // 오른쪽 위
                        (PlayerManager.Instance.row + 1, PlayerManager.Instance.col + 1),  // 오른쪽 아래
                        (PlayerManager.Instance.row + 1, PlayerManager.Instance.col - 1)   // 왼쪽 아래
                    };
                    foreach ((int, int) coord in coords)
                    {
                        if (coord.Item1 >= 0 && coord.Item1 < 3 && coord.Item2 >= 0 && coord.Item2 < 3)
                        {
                            this.movableSpace[coord.Item1, coord.Item2] = true;
                        }
                    }
                    break;
                }

            case MoveDirection.Colored:
                // 내가 색칠해 뒀던 칸으로 이동
                {
                    BoardColor[,] boardColors = BoardManager.Instance.BoardColors;

                    for (int i = 0; i < boardColors.GetLength(0); i++)  // row
                    {
                        for (int j = 0; j < boardColors.GetLength(1); j++)  // col
                        {
                            if (boardColors[i, j] == BoardColor.Player)
                            {
                                this.movableSpace[i, j] = true;
                            }
                        }
                    }
                    break;
                }

            case MoveDirection.Dangerous:
                // 적이 이번 턴에 공격할 칸으로 이동
                {
                    //List<Enemy> enemyList = EnemyManager.EnemyList;
                    List<Enemy> enemyList = new List<Enemy>();

                    foreach (Enemy enemy in enemyList)
                        foreach ((int, int) coord in enemy.WhereToAttack)
                            this.movableSpace[coord.Item1, coord.Item2] = true;
                    break;
                }

            case MoveDirection.All:
                // 원하는 칸으로 이동
                this.movableSpace = new bool[,] {
                    { true, true, true },
                    { true, true, true },
                    { true, true, true }
                };
                break;

            default:
                return;
        }
    }

    public override void DoAction(States state)
    {
        
    }

    public override void Enter()
    {
        // 카메라 암전 등
    }

    public override void MouseEvent()
    {
        // 이동 가능한 곳을 클릭할 시 진행.
    }

    public override void Update()
    {
        // UI 상으로 이동 가능한 곳은 O 표시.
    }

    public override void Exit()
    {
        // 이동 모션?
        // 카메라 다시 밝게
        //BoardManager.Instance.MovePlayer()
    }
}