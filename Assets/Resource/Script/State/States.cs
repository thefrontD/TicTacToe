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
        int boardSize = BoardManager.Instance.BoardSize;
        this.movableSpace = new bool[boardSize, boardSize];  // 모든 항이 false인 2D 배열
        switch (originalCard.moveDirection)
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
                    (int, int)[] coords =
                    {
                        (PlayerManager.Instance.Row - 1, PlayerManager.Instance.Col),  // 위
                        (PlayerManager.Instance.Row, PlayerManager.Instance.Col + 1),  // 오른쪽
                        (PlayerManager.Instance.Row + 1, PlayerManager.Instance.Col),  // 아래
                        (PlayerManager.Instance.Row, PlayerManager.Instance.Col - 1)   // 왼쪽
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

            case MoveDirection.Diagonal:
                // 현재 위치로부터 대각선으로 한 칸 이동
                {
                    (int, int)[] coords =
                    {
                        (PlayerManager.Instance.Row - 1, PlayerManager.Instance.Col - 1),  // 왼쪽 위
                        (PlayerManager.Instance.Row - 1, PlayerManager.Instance.Col + 1),  // 오른쪽 위
                        (PlayerManager.Instance.Row + 1, PlayerManager.Instance.Col + 1),  // 오른쪽 아래
                        (PlayerManager.Instance.Row + 1, PlayerManager.Instance.Col - 1)   // 왼쪽 아래
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
                    for (int i = 0; i < boardSize; i++)  // row
                        for (int j = 0; j < boardSize; j++)  // col
                            if (boardColors[i][j] == BoardColor.Player)
                                this.movableSpace[i, j] = true;
                    break;
                }

            case MoveDirection.Dangerous:
                // 적이 이번 턴에 공격할 칸으로 이동
                {
                    List<Enemy> enemyList = EnemyManager.Instance.EnemyList;
                    int playerRow = PlayerManager.Instance.Row;
                    int playerCol = PlayerManager.Instance.Col;

                    foreach (Enemy enemy in enemyList)
                    {
                        EnemyAction enemyAction = enemy.EnemyActions.Peek();
                        switch (enemyAction)
                        {
                            case EnemyAction.RowAttack:
                                for (int j = 0; j < boardSize; j++)
                                    this.movableSpace[playerRow, j] = true;
                                break;
                            case EnemyAction.ColAttack:
                                for (int i = 0; i < boardSize; i++)
                                    this.movableSpace[i, playerCol] = true;
                                break;
                            case EnemyAction.AllAttack:
                                for (int i = 0; i < boardSize; i++)
                                    for (int j = 0; j < boardSize; j++)
                                        this.movableSpace[i, j] = true;
                                break;
                            case EnemyAction.ColorAttack:  // TODO: ColorAttack은 Enemy로 색칠된 건지, Player로 색칠된 건지?
                                {
                                    List<List<BoardColor>> boardColors = BoardManager.Instance.BoardColors;
                                    for (int i = 0; i < boardSize; i++)  // row
                                        for (int j = 0; j < boardSize; j++)  // col
                                            if (boardColors[i][j] == BoardColor.Player)
                                                this.movableSpace[i, j] = true;
                                    break;
                                }
                            case EnemyAction.UnColorAttack:
                                {
                                    List<List<BoardColor>> boardColors = BoardManager.Instance.BoardColors;
                                    for (int i = 0; i < boardSize; i++)  // row
                                        for (int j = 0; j < boardSize; j++)  // col
                                            if (boardColors[i][j] != BoardColor.Player)
                                                this.movableSpace[i, j] = true;
                                    break;
                                }
                        }
                    }
                    break;
                }
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
        // 코드 출처: https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=stizms&logNo=220226873659
        Camera camera = Camera.main;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = camera.farClipPlane;

        Vector3 dir = camera.ScreenToWorldPoint(mousePos);

        if (Physics.Raycast(camera.transform.position, dir, out RaycastHit hit, mousePos.z))  // 맞췄다!
        {
            GameObject gameObject = hit.transform.gameObject;
            Board board = gameObject.GetComponent<Board>();
            if (board != null)  // 클릭한 물체가 Board 칸 중 하나일 경우
            {
                int row = board.Row;
                int col = board.Col;
                Debug.Log($"{row}, {col}");
                if (this.movableSpace[row, col])
                {
                    Debug.Log("Move!");
                }
            }
        }
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
    private ColorTargetPosition Target;
    private int Cost;
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

    public override void Exit()
    {

    }

    public override void MouseEvent()
    {
        PlayerManager.Instance.ChangeStates(PlayerManager.Instance.StatesQueue.Dequeue());
    }

    public override void Update()
    {

    }
}

