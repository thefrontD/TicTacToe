using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickOutline;
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
    private int DrawNum;
    private bool isNewPlayerTurn;
    
    public NormalState(int DrawNum = 5, bool isNewPlayerTurn = false)
    {
        if(PlayerManager.Instance.DebuffDictionary[Debuff.DrawCardDecrease] != 0)
            this.DrawNum = DrawNum-1;
        else
            this.DrawNum = DrawNum;
        this.isNewPlayerTurn = isNewPlayerTurn;
    }

    public override void DoAction(States state)
    {
        return;
    }

    public override void Enter()
    {
        if(isNewPlayerTurn){
            PlayerManager.Instance.SetMana(1000);
            if(PlayerManager.Instance.DebuffDictionary[Debuff.Heal] > 0)
                PlayerManager.Instance.DamageToPlayer((int)(PlayerManager.Instance.MaxHp*0.1));
            CardManager.Instance.DrawCard(DrawNum);
            foreach(Debuff debuff in Enum.GetValues(typeof(Debuff)))
                PlayerManager.Instance.SetDebuff(debuff, -1); 
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
        for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
        {
            for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
            {
                BoardManager.Instance.GameBoard[i][j].SetHighlight(BoardSituation.None);
            }
        }

        foreach(Enemy enemy in EnemyManager.Instance.EnemyList){
            if(enemy.DebuffDictionary[Debuff.Heal] > 0)
                enemy.EnemyHP += (int)(enemy.EnemyMaxHP*0.1);
            foreach(Debuff debuff in Enum.GetValues(typeof(Debuff)))
                enemy.SetDebuff(debuff, -1);
            if (enemy.EnemyShield == 0)
                enemy.EnemyShield = enemy.EnemyMaxShield;
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
        foreach(Enemy enemy in EnemyManager.Instance.EnemyList)
            enemy.setPreviousPos(PlayerManager.Instance.Row, PlayerManager.Instance.Col);
        EnemyManager.Instance.HightLightBoard();
    }
}


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
        this.movableSpace = new bool[boardSize, boardSize];  // 모든 항이 false인 2D 배열
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
                        case EnemyAction.AllAttack:
                            for (int i = 0; i < boardSize; i++)
                                for (int j = 0; j < boardSize; j++)
                                    this.movableSpace[i, j] = true;
                            break;
                        case EnemyAction.ColoredAttack:  // TODO: ColorAttack은 Enemy로 색칠된 건지, Player로 색칠된 건지?
                        {
                            List<List<BoardColor>> boardColors = BoardManager.Instance.BoardColors;
                            for (int i = 0; i < boardSize; i++)  // row
                                for (int j = 0; j < boardSize; j++)  // col
                                    if (boardColors[i][j] == BoardColor.Player)
                                        this.movableSpace[i, j] = true;
                            break;
                        }
                        case EnemyAction.NoColoredAttack:
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

        // 카메라 암전 등
        Camera camera = Camera.main;
        this.tempBackgroundColor = camera.backgroundColor;
        camera.backgroundColor = Color.blue;
    }

    public override void Update()
    {
        // UI 상으로 이동 가능한 곳은 O 표시.
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
                    this.moveRow = row;
                    this.moveCol = col;
                    Debug.Log($"Move to R{this.moveRow}, C{this.moveCol}");
                    PlayerManager.Instance.ChangeStates(PlayerManager.Instance.StatesQueue.Dequeue());
                }
            }
        }
    }

    public override void Exit()
    {
        // 이동 모션?
        // 카메라 다시 밝게
        Camera camera = Camera.main;
        camera.backgroundColor = this.tempBackgroundColor;

        PlayerManager.Instance.MovePlayer(this.moveRow, this.moveCol);
        DoAdditionalEffect();
    }

    private void DoAdditionalEffect()
    {
        bool proceed = false;
        switch (this.card.AdditionalEffectCondition)
        {
            case AdditionalEffectCondition.PlayerInColoredSpace:
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

        switch(this.card.AdditionalEffect)  // TODO
        {
            case AdditionalEffect.Re:
            {
                break;
            }
            case AdditionalEffect.Mana1:
            {
                break;
            }
            case AdditionalEffect.Draw1:
            {
                break;
            }
            case AdditionalEffect.PlayerHp30:
            {
                break;
            }
            case AdditionalEffect.DumpMoveCard1:
            {
                break;
            }
        }
    }
}

public interface IAttackable    //선택 가능한 오브젝트들이 IAttackable을 갖는다
{
    void AttackedByPlayer(int damage);
    GameObject GetGameObject();
}

public class AttackState : BaseState
{
    private AttackCard card;
    private int targetCountLeft;
    List<IAttackable> attackableList = new List<IAttackable>();
    List<IAttackable> selectedAttackableList = new List<IAttackable>();
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
        this.card = card;
    }

//취소하면 normal state로 돌아감
    public override void DoAction(States state)
    {
        
    }
    public override void Enter()
    {
        //공격 가능한 대상 개수 가져옴
        targetCountLeft = card.TargetCount;
        //공격 가능한 대상 종류 확인
        int targetType = card.TargetType;
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
        //공격 가능한 대상의 테두리를 밝은 파란 테두리로 표시
        foreach(IAttackable attackable in attackableList)
        {
            attackable.GetGameObject().GetComponent<Outline>().enabled = true;
            attackable.GetGameObject().GetComponent<Outline>().color = 3;
        }
        //targetCount가 1이 아닌 경우, 바로 처리(0: 전체 공격/2 이상: 랜덤 공격)
        if (card.TargetCount == 0)
        {
            //전부 공격
            for (int i = 0; i < attackableList.Count; i++)
            {
                selectedAttackableList.Add(attackableList[i]);
            }
            GiveDamageToSelectedAttackableList();
            PlayerManager.Instance.ChangeStates(PlayerManager.Instance.StatesQueue.Dequeue());
        }
        if (card.TargetCount > 1)
        {
            //card.TargetCount만큼 랜덤 공격
            for (int i = 0; i < card.TargetCount; i++)
            {
                int rand = Random.Range(0, attackableList.Count);
                selectedAttackableList.Add(attackableList[rand]);
                attackableList[rand].GetGameObject().GetComponent<Outline>().enabled = false;   //아웃라인 끔
                attackableList.Remove(attackableList[rand]);
            }
            GiveDamageToSelectedAttackableList();
            PlayerManager.Instance.ChangeStates(PlayerManager.Instance.StatesQueue.Dequeue());
        }
    }
    public override void Update()
    {
        /* 마우스오버 했을 때 오브젝트 커지게
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData))
        {
            IAttackable iAttackable = hitData.transform.gameObject.GetComponent<IAttackable>();
            if (iAttackable != null) //레이캐스트에 맞은 오브젝트에 Iattackable 컴포넌트가 있는가?
            {
                if (attackableList.Contains(iAttackable))   //attackableList에 있는가?
                {
                    Debug.Log("raycast hit");
                }
            }
        }
        */
    }
    public override void MouseEvent()
    {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitData;
            if (Physics.Raycast(ray, out hitData))
            {
                IAttackable iAttackable = hitData.transform.gameObject.GetComponent<IAttackable>();
                if (iAttackable != null) //레이캐스트에 맞은 오브젝트에 Iattackable 컴포넌트가 있는가?
                {
                    if (attackableList.Contains(iAttackable))   //attackableList에 있는가?
                    {
                        selectedAttackableList.Add(iAttackable);    //공격할 오브젝트 리스트에 추가
                        GiveDamageToSelectedAttackableList();
                        PlayerManager.Instance.ChangeStates(PlayerManager.Instance.StatesQueue.Dequeue());
                    }
                }
            }
    }
    public override void Exit()
    {
        //아웃라인 끄기
        for (int i = 0; i < attackableList.Count; i++)
        {
            attackableList[i].GetGameObject().GetComponent<Outline>().enabled = false;
        }
        //attackableList 초기화
        attackableList.Clear();
        selectedAttackableList.Clear();
        DoAdditionalEffect();
    }

    private void DoAdditionalEffect()
    {
        bool proceed = false;
        switch (this.card.AdditionalEffectCondition)
        {
            case AdditionalEffectCondition.PlayerInColoredSpace:
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

        switch (this.card.AdditionalEffect)  // TODO
        {
            case AdditionalEffect.Re:
            {
                break;
            }
            case AdditionalEffect.Mana1:
            {
                break;
            }
            case AdditionalEffect.Draw1:
            {
                break;
            }
            case AdditionalEffect.PlayerHp30:
            {
                break;
            }
            case AdditionalEffect.DumpMoveCard1:
            {
                break;
            }
        }
    }

    private void GiveDamageToSelectedAttackableList()
    {
        int damage = card.Damage;

                if(PlayerManager.Instance.DebuffDictionary[Debuff.PowerIncrease] != 0)
                    damage = (int) (1.2 * damage);

                if(PlayerManager.Instance.DebuffDictionary[Debuff.PowerDecrease] != 0)
                    damage = (int) (0.8 * damage);

                foreach (IAttackable selectedAttackable in selectedAttackableList)
                {
                    for (int i = card.AttackCount; i > 0; i--)  //AttackCount번 공격
                    {
                        selectedAttackable.AttackedByPlayer(card.Damage);   //Damage 줌
                    }
                }
                foreach (IAttackable attackable in attackableList)
                {
                    attackable.GetGameObject().GetComponent<Outline>().enabled = false;
                }
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
                if(Col+1 < boardsize)
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
            case ColorTargetPosition.Horizontal:
                for(int i = 0; i < boardsize; i++){
                    IfColorableAddToList(i,Col);
                }
                break;
            case ColorTargetPosition.Vertical:
                for(int i = 0; i<boardsize; i++){
                    IfColorableAddToList(Row,i);
                }
                break;
            case ColorTargetPosition.P3H:
                if(Row+1 < boardsize)
                    IfColorableAddToList(Row+1, Col);
                if(Row-1 > 0)
                    IfColorableAddToList(Row-1, Col);
                IfColorableAddToList(Row, Col);
                break;
            case ColorTargetPosition.P3V:
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
        /*foreach(Tuple<int,int> elem in colorables){
            Debug.Log("printing colorable elems");
            Debug.Log(elem.Item1);
            Debug.Log(elem.Item2);
        }*/

        //선택할 필요가 없는 경우 바로 시전
        if(card.colorTargetNum != ColorTargetNum.One){
            Debug.Log("Target is unselectable");
            //todo
            foreach(Tuple<int,int> pos in colorables){
                Debug.Log("(MouseEvent)" + pos.Item1.ToString() + pos.Item2.ToString());
                BoardManager.Instance.ColoringBoard(pos.Item1, pos.Item2, BoardColor.Player);
            }
            
            PlayerManager.Instance.ChangeStates(PlayerManager.Instance.StatesQueue.Dequeue());
            return;
        }
        else{//선택할 필요가 있는 경우 highlight enable
            foreach(Tuple<int,int> coord in colorables){
                BoardManager.Instance.GameBoard[coord.Item2][coord.Item1].GetComponent<Outline>().enabled = true;
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
        if(true)
        {
            foreach(RaycastHit Data in hitData){
                GameObject hitObject = Data.transform.gameObject;
                if(hitObject.GetComponent<Board>()){
                    //Debug.Log("it is board");
                    BoardManager.Instance.ColoringBoard(hitObject.GetComponent<Board>().Row,
                        hitObject.GetComponent<Board>().Col, BoardColor.Player);
                    //highlight disable
                    foreach(Tuple<int,int> coord in colorables){
                        BoardManager.Instance.GameBoard[coord.Item2][coord.Item1].GetComponent<Outline>().enabled = false;
                    }
                    EnemyManager.Instance.HightLightBoard();
                    PlayerManager.Instance.ChangeStates(PlayerManager.Instance.StatesQueue.Dequeue());
                }
                else{
                    //Debug.Log("it is nonboard Object");
                }
            }
        }
    }

    public override void Exit()
    {
        DoAdditionalEffect();
    }

    private void DoAdditionalEffect()
    {
        bool proceed = false;
        switch (this.card.AdditionalEffectCondition)
        {
            case AdditionalEffectCondition.PlayerInColoredSpace:
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

        switch (this.card.AdditionalEffect)  // TODO
        {
            case AdditionalEffect.Re:
            {
                break;
            }
            case AdditionalEffect.Mana1:
            {
                break;
            }
            case AdditionalEffect.Draw1:
            {
                break;
            }
            case AdditionalEffect.PlayerHp30:
            {
                break;
            }
            case AdditionalEffect.DumpMoveCard1:
            {
                break;
            }
        }
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
            Debug.Log("(IfColorableAddToList) "+ i.ToString() + j.ToString() + " block is None");
            colorables.Add(new Tuple<int,int>(i, j));
            return;
        }
        if(BoardManager.Instance.BoardObjects[i][j] == BoardObject.Player){
            Debug.Log("(IfColorableAddToList) "+ i.ToString() + j.ToString() + " block is Player");
            colorables.Add(new Tuple<int,int>(i, j));
            return;
        }
        return;
    }
}

