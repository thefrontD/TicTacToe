using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColorState : BaseState
{
    private bool Selectable;
    private ColorCard card;
    List<(int row, int col)> colorables = new List<(int row, int col)>();
    int boardsize;
    int prevBingoCount;
    (int row, int col) clickedCoord = (-1, -1);

    public ColorState(ColorCard card)
    {
        this.card = card;
    }

    public override void DoAction(States state)
    {
        //각 state에서 필요한 함수 작성(필수아님)
    }

    public override void Enter()
    {
        boardsize = BoardManager.Instance.BoardSize;
        int Row = PlayerManager.Instance.Row;
        int Col = PlayerManager.Instance.Col;
        prevBingoCount = BoardManager.Instance.CountBingo(BoardColor.Player);

        switch (card.colorTargetPosition)
        {
            case ColorTargetPosition.All:
                for (int i = 0; i < boardsize; i++)
                {
                    for (int j = 0; j < boardsize; j++)
                    {
                        IfColorableAddToList(i, j);
                    }
                }

                //Debug.Log(BoardManager.Instance.BoardObjects[0][0]);
                //if(BoardManager.BoardObjects[i][j])
                break;
            case ColorTargetPosition.P1:
                IfColorableAddToList(Row, Col);
                break;
            case ColorTargetPosition.P4:
                if (Row + 1 < boardsize)
                    IfColorableAddToList(Row + 1, Col);
                if (Row - 1 >= 0)
                    IfColorableAddToList(Row - 1, Col);
                if (Col + 1 < boardsize)
                    IfColorableAddToList(Row, Col + 1);
                if (Col - 1 >= 0)
                    IfColorableAddToList(Row, Col - 1);
                break;
            case ColorTargetPosition.P5:
                if (Row + 1 < boardsize)
                    IfColorableAddToList(Row + 1, Col);
                if (Row - 1 >= 0)
                    IfColorableAddToList(Row - 1, Col);
                if (Col + 1 < boardsize)
                    IfColorableAddToList(Row, Col + 1);
                if (Col - 1 >= 0)
                    IfColorableAddToList(Row, Col - 1);
                IfColorableAddToList(Row, Col);
                break;
            case ColorTargetPosition.Color:
                for (int i = 0; i < boardsize; i++)
                {
                    for (int j = 0; j < boardsize; j++)
                    {
                        if (BoardManager.Instance.BoardColors[i][j] == BoardColor.Player && isNextToColored(i, j))
                            IfColorableAddToList(i, j);
                    }
                }
                break;
            /*case ColorTargetPosition.Vertical:
                for (int i = 0; i < boardsize; i++)
                {
                    IfColorableAddToList(i, Col);
                }

                break;*/
            /*case ColorTargetPosition.Horizontal:
                for (int i = 0; i < boardsize; i++)
                {
                    IfColorableAddToList(Row, i);
                }

                break;*/
            case ColorTargetPosition.P3V:
                if (Row + 1 < boardsize)
                    IfColorableAddToList(Row + 1, Col);
                if (Row - 1 >= 0)
                    IfColorableAddToList(Row - 1, Col);
                IfColorableAddToList(Row, Col);
                break;
            case ColorTargetPosition.P3H:
                if (Col + 1 < boardsize)
                    IfColorableAddToList(Row, Col + 1);
                if (Col - 1 >= 0)
                    IfColorableAddToList(Row, Col - 1);
                IfColorableAddToList(Row, Col);
                break;
        }
        //Colorable 리스트 colorables 에 튜플로 다 저장됨 

        //color 대상 list
        //각각의 block 에 대해 IsColorable 함수 호출
        /*foreach(Tuple<int,int> elem in colorables){
            Debug.Log("printing colorable elems");
            Debug.Log(elem.Item1);
            Debug.Log(elem.Item2);
        }*/

        //선택할 필요가 없는 경우 바로 시전
        if (card.colorTargetNum != ColorTargetNum.One || card.colorTargetPosition == ColorTargetPosition.P1)
        {
            //Debug.Log("Target is unselectable");
            //todo
            foreach ((int row, int col) in colorables)
            {
                ///Debug.Log("(MouseEvent)" + row.ToString() + col.ToString());
                BoardManager.Instance.ColoringBoard(row, col, BoardColor.Player);
            }

            PlayerManager.Instance.EndCurrentState();
            return;
        }
        else
        {
            //선택할 필요가 있는 경우 highlight enable
            foreach ((int row, int col) in colorables)
            {
                BoardManager.Instance.GameBoard[row][col].SetHighlight(BoardSituation.WillColored);
            }
        }
    }

    public override void Update()
    {
    }

    public override void MouseEvent()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hitData;
        hitData = Physics.RaycastAll(ray);
        if (true)
        {
            foreach (RaycastHit Data in hitData)
            {
                GameObject hitObject = Data.transform.gameObject;
                Board hitBoard = hitObject.GetComponent<Board>();
                if (hitBoard)
                {
                    this.clickedCoord = (hitBoard.Row, hitBoard.Col);
                    if (colorables.Contains(this.clickedCoord))
                    {
                        //Debug.Log("it is board");
                        BoardManager.Instance.ColoringBoard(hitBoard.Row, hitBoard.Col, BoardColor.Player);
                        //highlight disable
                        foreach ((int row, int col) in colorables)
                        {
                            BoardManager.Instance.GameBoard[row][col].SetHighlight(BoardSituation.None);
                        }

                        EnemyManager.Instance.HightLightBoard();
                        PlayerManager.Instance.EndCurrentState();
                    }
                    else
                    {
                        this.clickedCoord = (-1, -1);
                    }
                }
            }
        }
    }

    public override void Exit()
    {
        //빙고 확인 후 Ap 증가
        PlayerManager.Instance.GainAp(PlayerManager.Instance.GetAdditionalApByBingo(BoardManager.Instance.CheckBingo(BoardColor.Player)));
        Debug.Log("AdditionalEffectCondition: " + card.AdditionalEffectCondition);
        Debug.Log("AdditionalEffect: " + card.AdditionalEffect);
        EnemyManager.Instance.HightLightBoard();
        DoAdditionalEffect();
    }

    private void DoAdditionalEffect()
    {
        bool proceed = false;

        switch (this.card.AdditionalEffectCondition)
        {
            case AdditionalEffectCondition.MakeBingo: // 빙고를 완성했을 때. 이 카드를 냈을 때 BingoCount가 더 커지면 됨.
            {
                if (BoardManager.Instance.CountBingo(BoardColor.Player) > this.prevBingoCount)
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
            case AdditionalEffect.PlayerHpMinus10: // 플레이어에게 피해 10
            {
                PlayerManager.Instance.SetHp(-10);
                break;
            }
            case AdditionalEffect.DumpColorCard1: // 이동 카드 1장 버림
            {
                // DumpState를 state queue의 head에 Enqueue시킴
                List<BaseState> states = new List<BaseState> { new DumpState(CardType.Color, 1) };
                states.AddRange(PlayerManager.Instance.StatesQueue);

                // 되도록이면 StatesQueue를 교체하지 않고 in-place로 내부값만 바꾸고 싶었음
                PlayerManager.Instance.StatesQueue.Clear();
                foreach (BaseState state in states)
                    PlayerManager.Instance.StatesQueue.Enqueue(state);
                break;
            }
            /*case AdditionalEffect.Move: // 그 칸으로 이동
            {
                if (this.clickedCoord.row >= 0 && this.clickedCoord.col >= 0)
                {
                    PlayerManager.Instance.MovePlayer(this.clickedCoord.row, this.clickedCoord.col);
                }

                break;
            }*/
            case AdditionalEffect.MonsterShieldMinus20: // 그 몬스터의 실드 -20
            {
                foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
                {
                    enemy.AttackedByPlayer(20, 1);
                }

                break;
            }
            case AdditionalEffect.MonsterShieldMinus1000: // 그 몬스터의 실드 -1000
            {
                foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
                {
                    enemy.AttackedByPlayer(1000, 1);  // 사실상 전부 파괴
                }

                break;
            }
            case AdditionalEffect.Re: // 그 카드를 패로 되돌림
            {
                CardManager.Instance.DrawFromGrave();
                break;
            }
        }
    }

    private bool isNextToColored(int i, int j)
    {
        bool result = false;
        if (i + 1 < boardsize && BoardManager.Instance.BoardColors[i + 1][j] == BoardColor.Player)
            result = true;
        if (i - 1 > 0 && BoardManager.Instance.BoardColors[i - 1][j] == BoardColor.Player)
            result = true;
        if (j + 1 < boardsize && BoardManager.Instance.BoardColors[i][j + 1] == BoardColor.Player)
            result = true;
        if (j - 1 > 0 && BoardManager.Instance.BoardColors[i][j - 1] == BoardColor.Player)
            result = true;
        return result;
    }

    private void IfColorableAddToList(int i, int j)
    {
        //컬러가 되어있는 곳은 선택 불가능!
        //벽이 있는 곳은 선택 불가능
        //미니언이 있는 곳은 선택 불가능
        //비어있는 곳은 색칠 가능
        //플레이어가 있는 곳은 색칠 가능
        if(BoardManager.Instance.BoardColors[i][j] == BoardColor.Player && BoardManager.Instance.BoardColors[i][j] == BoardColor.Black){
            //Debug.Log("(IfColorableAddToList) " + i.ToString() + j.ToString() + " block is already colored");
            return;
        }

        if (BoardManager.Instance.BoardObjects[i][j] == BoardObject.None)
        {
            //Debug.Log("(IfColorableAddToList) " + i.ToString() + j.ToString() + " block is None");
            colorables.Add((i, j));
            return;
        }

        if (BoardManager.Instance.BoardObjects[i][j] == BoardObject.Player)
        {
            //Debug.Log("(IfColorableAddToList) " + i.ToString() + j.ToString() + " block is Player");
            colorables.Add((i, j));
            return;
        }

        return;
    }
}