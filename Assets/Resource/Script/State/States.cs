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
    private bool isNewPlayerTurn;
    
    public NormalState(int DrawNum = 5, bool isNewPlayerTurn = false)
    {
        this.DrawNum = DrawNum;
        this.isNewPlayerTurn = isNewPlayerTurn;
    }

    public override void DoAction(States state)
    {
        return;
    }

    public override void Enter()
    {
        if(isNewPlayerTurn)
            CardManager.Instance.DrawCard(DrawNum);
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
        return;
    }
}
public class EnemyState : BaseState
{
    public override void DoAction(States state)
    {
        return;
    }

    public override void Enter()
    {
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
        return;
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
                        (PlayerManager.Instance.Row - 1, PlayerManager.Instance.Col),  // 위
                        (PlayerManager.Instance.Row, PlayerManager.Instance.Col + 1),  // 오른쪽
                        (PlayerManager.Instance.Row + 1, PlayerManager.Instance.Col),  // 아래
                        (PlayerManager.Instance.Row, PlayerManager.Instance.Col - 1)   // 왼쪽
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
                        (PlayerManager.Instance.Row - 1, PlayerManager.Instance.Col - 1),  // 왼쪽 위
                        (PlayerManager.Instance.Row - 1, PlayerManager.Instance.Col + 1),  // 오른쪽 위
                        (PlayerManager.Instance.Row + 1, PlayerManager.Instance.Col + 1),  // 오른쪽 아래
                        (PlayerManager.Instance.Row + 1, PlayerManager.Instance.Col - 1)   // 왼쪽 아래
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
                    List<List<BoardColor>> boardColors = BoardManager.Instance.BoardColors;

                    for (int i = 0; i < boardColors.Count; i++)  // row
                    {
                        for (int j = 0; j < boardColors[0].Count; j++)  // col
                        {
                            if (boardColors[i][j] == BoardColor.Player)
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
                    /*
                    List<Enemy> enemyList = new List<Enemy>();

                    foreach (Enemy enemy in enemyList)
                        foreach ((int, int) coord in enemy.WhereToAttack)
                            this.movableSpace[coord.Item1, coord.Item2] = true;
                    */
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

    public override void Exit()
    {
        // 이동 모션?
        // 카메라 다시 밝게
        //BoardManager.Instance.MovePlayer()
    }

    public override void MouseEvent()
    {
        // 이동 가능한 곳을 클릭할 시 진행.
    }

    public override void Update()
    {
        // UI 상으로 이동 가능한 곳은 O 표시.
    }

}

public interface IAttackable    //선택 가능한 오브젝트들이 IAttackable을 갖는다
{
        
}

public class AttackState : BaseState
{
    private AttackCard Card;
    List<IAttackable> attackableList = new List<IAttackable>();
    struct coord {
        public coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int x;
        public int y; 
    }

    public AttackState(AttackCard card)
    {
        this.Card = card;
    }

//취소하면 normal state로 돌아감
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
        int playerRow = PlayerManager.Instance.Row;
        int playerCol = PlayerManager.Instance.Col;
        coord[] coords = {new coord(playerRow-1, playerCol), new coord(playerRow + 1, playerCol), new coord(playerRow, playerCol - 1), new coord(playerRow, playerCol + 1)};
        
        if(isMonster)
        {
            List<Enemy> enemyList = EnemyManager.Instance.EnemyList;
            attackableList.AddRange(enemyList);
        }
        if(isWall)
        {
            foreach (coord c in coords)
            {
                //row col이 0,3 미만이고, 그 좌표에 Wall이 있을 때
                if (c.x >= 0 && c.x < 3 && c.y >= 0 && c.y < 3 && BoardManager.Instance.BoardObjects[c.x][c.y] == BoardObject.Wall)
                {
                    if (BoardManager.Instance.BoardObjects[c.x][c.y] == BoardObject.Wall)
                    {
                        attackableList.Add(BoardManager.Instance.BoardAttackables[c.x][c.y]);
                    }
                }
            }
        }
        if(isMinion)
        {
            foreach (coord c in coords)
            {
                //row col이 0,3 미만이고, 그 좌표에 Minion이 있을 때
                if (c.x >= 0 && c.x < 3 && c.y >= 0 && c.y < 3 && BoardManager.Instance.BoardObjects[c.x][c.y] == BoardObject.Minion)
                {
                    if (BoardManager.Instance.BoardObjects[c.x][c.y] == BoardObject.Minion)
                    {
                        attackableList.Add(BoardManager.Instance.BoardAttackables[c.x][c.y]);
                    }
                }
            }
        }
        //모든 attack 가능한 오브젝트를 attackableList에 담았음
    }
    public override void Exit()
    {
        //attackableList 초기화
        attackableList.Clear();
    }

    public override void MouseEvent()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if(Physics.Raycast(ray, out hitData))
        {
            GameObject hitObject = hitData.transform.gameObject;
            Debug.Log(hitObject);
            IAttackable iAttackable = hitObject.GetComponent<IAttackable>();
            if(iAttackable != null) //레이캐스트에 맞은 오브젝트에 Iattackable 컴포넌트가 있는가?
            {
                if (attackableList.Contains(iAttackable))   //attackableList에 있는가?
                {
                    //처리
                }
            }
        }
    }
    public override void Update()
    {
        // UI 상으로 이동 가능한 곳은 O 표시.
    }
}


public class ColorState : BaseState
{
    
    private bool Selectable;
    private ColorCard card;
    List<Tuple<int,int>> colorables = new List<Tuple<int,int>>(); 
    int boardsize;

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

        switch(card.colorTargetPosition)
        {
            case ColorTargetPosition.All:
                for(int i=0; i<boardsize; i++){
                    for(int j = 0; j<boardsize; j++){
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
                if(Row+1 < boardsize)
                    IfColorableAddToList(Row+1, Col);
                if(Row-1 > 0)
                    IfColorableAddToList(Row-1, Col);
                if(Col+1 <boardsize)
                    IfColorableAddToList(Row, Col+1);
                if(Col-1 > 0)
                    IfColorableAddToList(Row, Col-1);
                break;
            case ColorTargetPosition.P5:
                if(Row+1 < boardsize)
                    IfColorableAddToList(Row+1, Col);
                if(Row-1 > 0)
                    IfColorableAddToList(Row-1, Col);
                if(Col+1 <boardsize)
                    IfColorableAddToList(Row, Col+1);
                if(Col-1 > 0)
                    IfColorableAddToList(Row, Col-1);
                IfColorableAddToList(Row,Col);
                break;
            case ColorTargetPosition.Color:
                for(int i=0; i<boardsize; i++){
                    for(int j = 0; j<boardsize; j++){
                        if(BoardManager.Instance.BoardColors[i][j] == BoardColor.Player && isNextToColored(i,j))
                            IfColorableAddToList(i, j);
                    }
                }
                break;
            case ColorTargetPosition.Vertical:
                for(int i = 0; i<boardsize; i++){
                    IfColorableAddToList(i,Col);
                }
                break;
            case ColorTargetPosition.Horizontal:
                for(int i = 0; i<boardsize; i++){
                    IfColorableAddToList(Row,i);
                }
                break;
            case ColorTargetPosition.P3V:
                if(Row+1 < boardsize)
                    IfColorableAddToList(Row+1, Col);
                if(Row-1 > 0)
                    IfColorableAddToList(Row-1, Col);
                IfColorableAddToList(Row, Col);
                break;
            case ColorTargetPosition.P3H:
                if(Col+1 < boardsize)
                    IfColorableAddToList(Row, Col+1);
                if(Col-1 > 0)
                    IfColorableAddToList(Row, Col-1);
                IfColorableAddToList(Row, Col);
                break;
        }
        //Colorable 리스트 colorables 에 튜플로 다 저장됨 

        //color 대상 list
        //각각의 block 에 대해 IsColorable 함수 호출

        foreach(Tuple<int,int> elem in colorables){
            Debug.Log("printing colorable elems");
            Debug.Log(elem.Item1);
            Debug.Log(elem.Item2);
        }

        //color 대상 highlight
    }

    public override void Exit()
    {

    }

    public override void MouseEvent()
    {
        //PlayerManager.Instance.ChangeStates(PlayerManager.Instance.StatesQueue.Dequeue());
        //일단 되는지 테스트
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if(Physics.Raycast(ray, out hitData))
        {
            GameObject hitObject = hitData.transform.gameObject;
            Debug.Log(hitObject);
            IAttackable iAttackable = hitObject.GetComponent<IAttackable>();
            if(iAttackable != null) //레이캐스트에 맞은 오브젝트에 Iattackable 컴포넌트가 있는가?
            {
                //if (attackableList.Contains(iAttackable))   //attackableList에 있는가?
                //{
                    //처리
                //}
            }
        }
    }

    public override void Update()
    {

    }

    private bool isNextToColored(int i, int j){
        bool result = false;
        if(i+1 < boardsize && BoardManager.Instance.BoardColors[i+1][j] == BoardColor.Player)
            result = true;
        if(i-1 > 0 && BoardManager.Instance.BoardColors[i-1][j] == BoardColor.Player)
            result = true;
        if(j+1 < boardsize && BoardManager.Instance.BoardColors[i][j+1] == BoardColor.Player)
            result = true;
        if(j-1 > 0 && BoardManager.Instance.BoardColors[i][j-1] == BoardColor.Player)
            result = true;
        return result;
    }

    private void IfColorableAddToList(int i, int j)
    {
        //컬러가 되어있는 곳은 선택 불가능?
        //벽이 있는 곳은 선택 불가능
        //미니언이 있는 곳은 선택 불가능
        //비어있는 곳은 색칠 가능
        //플레이어가 있는 곳은 색칠 가능
        if(BoardManager.Instance.BoardObjects[i][j] == BoardObject.None){
            Debug.Log("(IfColorableAddToList) this block is None");
            colorables.Add(new Tuple<int,int>(i, j));
            return;
        }
        if(BoardManager.Instance.BoardObjects[i][j] == BoardObject.Player){
            Debug.Log("(IfColorableAddToList) this block is Player");
            colorables.Add(new Tuple<int,int>(i, j));
            return;
        }
        return;
    }
}

