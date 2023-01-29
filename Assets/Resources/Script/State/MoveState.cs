using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveState : BaseState
{
    Color tempBackgroundColor;
    MoveCard card;
    bool[,] movableSpace;
    int moveRow, moveCol;

    public MoveState(MoveCard originalCard)
    {
        this.card = originalCard;
    }

    public override void DoAction(States state)
    {
    }

    public override void Enter()
    {
        int boardSize = BoardManager.Instance.BoardSize;
        this.movableSpace = new bool[boardSize, boardSize]; // 모든 항이 false인 2D 배열
        switch (this.card.moveDirection)
        {
            case MoveDirection.All:
                // 원하는 칸으로 이동
            {
                for (int i = 0; i < boardSize; i++)
                    for (int j = 0; j < boardSize; j++)
                        this.movableSpace[i, j] = true;
                break;
            }

            case MoveDirection.UDLR:
                // 현재 위치로부터 상하좌우로 한 칸 이동
            {
                if(!PlayerManager.Instance.TutorialTrigger){
                    (int, int)[] coords =
                    {
                        (PlayerManager.Instance.Row - 1, PlayerManager.Instance.Col), // 위
                        (PlayerManager.Instance.Row, PlayerManager.Instance.Col + 1), // 오른쪽
                        (PlayerManager.Instance.Row + 1, PlayerManager.Instance.Col), // 아래
                        (PlayerManager.Instance.Row, PlayerManager.Instance.Col - 1) // 왼쪽
                    };
                    foreach ((int, int) coord in coords)
                    {
                        if (coord.Item1 >= 0 && coord.Item1 < boardSize && coord.Item2 >= 0 && coord.Item2 < boardSize)
                        {
                            this.movableSpace[coord.Item1, coord.Item2] = true;
                        }
                    }
                }
                else if (PlayerManager.Instance.TutorialPhase == 9)
                {
                    (int, int)[] coords =
                    {
                        (PlayerManager.Instance.Row, PlayerManager.Instance.Col - 1) // 왼쪽
                    };
                    foreach ((int, int) coord in coords)
                    {
                        if (coord.Item1 >= 0 && coord.Item1 < boardSize && coord.Item2 >= 0 && coord.Item2 < boardSize)
                        {
                            this.movableSpace[coord.Item1, coord.Item2] = true;
                        }
                    }
                }
                else if (PlayerManager.Instance.TutorialPhase == 12)
                {
                    (int, int)[] coords =
                    {
                        (PlayerManager.Instance.Row - 1, PlayerManager.Instance.Col) // 아래
                    };
                    foreach ((int, int) coord in coords)
                    {
                        if (coord.Item1 >= 0 && coord.Item1 < boardSize && coord.Item2 >= 0 && coord.Item2 < boardSize)
                        {
                            this.movableSpace[coord.Item1, coord.Item2] = true;
                        }
                    }
                }
                else if (PlayerManager.Instance.TutorialPhase == 14)
                {
                    switch (PlayerManager.Instance.TutorialSubPhase)
                    {
                        case 0:
                        case 1:
                            (int, int)[] coord1s =
                            {
                                (PlayerManager.Instance.Row, PlayerManager.Instance.Col + 1) // 오른쪽
                            };
                            foreach ((int, int) coord in coord1s)
                            {
                                if (coord.Item1 >= 0 && coord.Item1 < boardSize && coord.Item2 >= 0 && coord.Item2 < boardSize)
                                {
                                    this.movableSpace[coord.Item1, coord.Item2] = true;
                                }
                            }
                            break;
                        case 2:
                            (int, int)[] coord2s =
                            {
                                (PlayerManager.Instance.Row + 1, PlayerManager.Instance.Col) // 아래
                            };
                            foreach ((int, int) coord in coord2s)
                            {
                                if (coord.Item1 >= 0 && coord.Item1 < boardSize && coord.Item2 >= 0 && coord.Item2 < boardSize)
                                {
                                    this.movableSpace[coord.Item1, coord.Item2] = true;
                                }
                            }
                            break;
                    }

                    PlayerManager.Instance.TutorialSubPhase++;
                }
                else if (PlayerManager.Instance.TutorialPhase == 19)
                {
                    (int, int)[] coords =
                    {
                        (PlayerManager.Instance.Row, PlayerManager.Instance.Col - 1)
                    };
                    foreach ((int, int) coord in coords)
                    {
                        if (coord.Item1 >= 0 && coord.Item1 < boardSize && coord.Item2 >= 0 && coord.Item2 < boardSize)
                        {
                            this.movableSpace[coord.Item1, coord.Item2] = true;
                        }
                    }
                }

                break;
            }

            case MoveDirection.Diagonal:
                // 현재 위치로부터 대각선으로 한 칸 이동
            {
                (int, int)[] coords =
                {
                    (PlayerManager.Instance.Row - 1, PlayerManager.Instance.Col - 1), // 왼쪽 위
                    (PlayerManager.Instance.Row - 1, PlayerManager.Instance.Col + 1), // 오른쪽 위
                    (PlayerManager.Instance.Row + 1, PlayerManager.Instance.Col + 1), // 오른쪽 아래
                    (PlayerManager.Instance.Row + 1, PlayerManager.Instance.Col - 1) // 왼쪽 아래
                };
                foreach ((int, int) coord in coords)
                {
                    if (coord.Item1 >= 0 && coord.Item1 < boardSize && coord.Item2 >= 0 && coord.Item2 < boardSize)
                    {
                        this.movableSpace[coord.Item1, coord.Item2] = true;
                    }
                }

                break;
            }

            case MoveDirection.Colored:
                // 내가 색칠해 뒀던 칸으로 이동
            {
                List<List<BoardColor>> boardColors = BoardManager.Instance.BoardColors;
                for (int i = 0; i < boardSize; i++) // row
                    for (int j = 0; j < boardSize; j++) // col
                        if (boardColors[i][j] == BoardColor.Player)
                            this.movableSpace[i, j] = true;
                break;
            }

            /*case MoveDirection.Dangerous:
                // 적이 이번 턴에 공격할 칸으로 이동
            {
                List<Enemy> enemyList = EnemyManager.Instance.EnemyList;
                int playerRow = PlayerManager.Instance.Row;
                int playerCol = PlayerManager.Instance.Col;

                foreach (Enemy enemy in enemyList)
                {
                    EnemyAction enemyAction = enemy.EnemyActions.Peek().Item1;
                    switch (enemyAction)
                    {
                        case EnemyAction.H1Attack:
                            for (int j = 0; j < boardSize; j++)
                                this.movableSpace[playerRow, j] = true;
                            break;
                        case EnemyAction.V1Attack:
                            for (int i = 0; i < boardSize; i++)
                                this.movableSpace[i, playerCol] = true;
                            break;
                        case EnemyAction.H2Attack:
                            for (int j = 0; j < boardSize; j++)
                            {
                                for (int i = 0; i < boardSize; i++)
                                {
                                    if (i == playerRow)
                                        continue;
                                    this.movableSpace[i, j] = true;
                                }
                            }
                            break;
                        case EnemyAction.V2Attack:
                            for (int i = 0; i < boardSize; i++)
                            {
                                for (int j = 0; j < boardSize; j++)
                                {
                                    if (j == playerCol)
                                        continue;
                                    this.movableSpace[i, j] = true;
                                }
                            }
                            break;
                        case EnemyAction.AllAttack:
                            for (int i = 0; i < boardSize; i++)
                                for (int j = 0; j < boardSize; j++)
                                    this.movableSpace[i, j] = true;
                            break;
                        case EnemyAction.ColoredAttack:
                        {
                            List<List<BoardColor>> boardColors = BoardManager.Instance.BoardColors;
                            for (int i = 0; i < boardSize; i++) // row
                                for (int j = 0; j < boardSize; j++) // col
                                    if (boardColors[i][j] == BoardColor.Player)
                                        this.movableSpace[i, j] = true;
                            break;
                        }
                        case EnemyAction.NoColoredAttack:
                        {
                            List<List<BoardColor>> boardColors = BoardManager.Instance.BoardColors;
                            for (int i = 0; i < boardSize; i++) // row
                                for (int j = 0; j < boardSize; j++) // col
                                    if (boardColors[i][j] != BoardColor.Player)
                                        this.movableSpace[i, j] = true;
                            break;
                        }
                    }
                }

                break;
            }*/
        }

        for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
        {
            for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
            {
                BoardManager.Instance.GameBoard[i][j].SetHighlight(BoardSituation.None); 

                if(this.movableSpace[i, j])
                    BoardManager.Instance.GameBoard[i][j].SetHighlight(BoardSituation.WillMove); 
            }
        }
    }

    public override void Update()
    {
        // UI 상으로 이동 가능한 곳은 O 표시.
    }

    public override void MouseEvent()
    {
        // 이동 가능한 곳을 클릭할 시 진행.
        LayerMask mask = LayerMask.GetMask("Board");
        
        Debug.Log(Utils.MousePos);
        
        if (Physics.Raycast(Utils.MousePos, Vector3.forward, out RaycastHit hit,50f, mask)) // 맞췄다!
        {
            Debug.Log("Laycast Work");
            Board board =  hit.collider.gameObject.GetComponent<Board>();
            if (board != null) // 클릭한 물체가 Board 칸 중 하나일 경우
            {
                int row = board.Row;
                int col = board.Col;
                //Debug.Log($"{row}, {col}");
                if (this.movableSpace[row, col] && BoardManager.Instance.BoardObjects[row][col] == BoardObject.None)
                {
                    this.moveRow = row;
                    this.moveCol = col;
                    //Debug.Log($"Move to R{this.moveRow}, C{this.moveCol}");
                    PlayerManager.Instance.EndCurrentState();
                }
                else if (this.movableSpace[row, col] && BoardManager.Instance.BoardObjects[row][col] == BoardObject.Player && card.triggerCondition == TriggerCondition.ColoredSpaceExists)
                {
                    // TODO: 이건 "이동 마법진"을 위한 임시조치. 나중에 이 코드는 없애고, 아예 전용 triggerCondition(ex. ColoredSpaceExceptPlayerExists)을 만들어야 할 듯.
                    this.moveRow = row;
                    this.moveCol = col;
                    PlayerManager.Instance.EndCurrentState();
                }
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("AdditionalEffectCondition: " + card.AdditionalEffectCondition);
        Debug.Log("AdditionalEffect: " + card.AdditionalEffect);

        int boardSize = BoardManager.Instance.BoardSize;

        // Outline 끄기
        for (int i = 0; i < boardSize; i++)
            for (int j = 0; j < boardSize; j++)
                BoardManager.Instance.GameBoard[i][j].SetHighlight(BoardSituation.None);

        EnemyManager.Instance.HightLightBoard();
        
        // 이동 모션
        PlayerManager.Instance.MovePlayer(this.moveRow, this.moveCol, this.card.MoveCardEffect);
        DoAdditionalEffect();
    }

    private void DoAdditionalEffect()
    {
        bool proceed = false;
        switch (this.card.AdditionalEffectCondition)
        {
            case AdditionalEffectCondition.PlayerInColoredSpace: // 플레이어 색의 칸으로 이동했을 때
            {
                (int r, int c) = (PlayerManager.Instance.Row, PlayerManager.Instance.Col);
                if (BoardManager.Instance.BoardColors[r][c] == BoardColor.Player)
                    proceed = true;
                break;
            }
            case AdditionalEffectCondition.None:
            {
                proceed = true;
                break;
            }
        }

        if (!proceed) return;

        switch (this.card.AdditionalEffect)
        {
            case AdditionalEffect.Mana1: // 마나 1 회복
            {
                PlayerManager.Instance.SetMana(1, ignoreDebuff: true);
                break;
            }
            case AdditionalEffect.Draw1: // 카드 1장 뽑음
            {
                CardManager.Instance.DrawCard(1);
                break;
            }
            case AdditionalEffect.Mana1Draw1: // 마나 1 회복, 카드 1장 뽑음
            {
                PlayerManager.Instance.SetMana(1, ignoreDebuff: true);
                CardManager.Instance.DrawCard(1);
                break;
            }
            case AdditionalEffect.PlayerHpPlus20: // 플레이어 20 회복
            {
                PlayerManager.Instance.SetHp(20);
                break;
            }
            case AdditionalEffect.PlayerHpMinus30: // 플레이어에게 피해 30
            {
                PlayerManager.Instance.SetHp(-30);
                break;
            }
            case AdditionalEffect.DumpMoveCard1: // 이동 카드 1장 버림
            {
                foreach (BaseState state in PlayerManager.Instance.StatesQueue)
                {
                    Debug.Log(state.ToString());
                }
                // DumpState를 state queue의 head에 Enqueue시킴
                List<BaseState> states = new List<BaseState> { new DumpState(CardType.Move, 1) };
                states.AddRange(PlayerManager.Instance.StatesQueue);

                // 되도록이면 StatesQueue를 교체하지 않고 in-place로 내부값만 바꾸고 싶었음
                PlayerManager.Instance.StatesQueue.Clear();
                foreach (BaseState state in states)
                    PlayerManager.Instance.StatesQueue.Enqueue(state);
                break;
            }
            case AdditionalEffect.Re: // 그 카드를 패로 되돌림
            {
                CardManager.Instance.DrawFromGrave();
                break;
            }
        }
    }
}