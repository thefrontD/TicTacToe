using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EPOOutline;
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
    private int DrawNum;
    private bool isNewPlayerTurn;

    public NormalState(int DrawNum = 6, bool isNewPlayerTurn = false)
    {
        this.isNewPlayerTurn = isNewPlayerTurn;

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
            CardManager.Instance.DrawCard(DrawNum);
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
        foreach (CardUI cardui in CardManager.Instance.HandCardList)
            cardui.gameObject.GetComponent<Outlinable>().enabled = false;
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
        Camera camera = Camera.main;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = camera.farClipPlane;

        Vector3 dir = camera.ScreenToWorldPoint(mousePos);

        if (Physics.Raycast(camera.transform.position, dir, out RaycastHit hit, mousePos.z)) // 맞췄다!
        {
            GameObject gameObject = hit.transform.gameObject;
            Board board = gameObject.GetComponent<Board>();
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

public interface IAttackable //선택 가능한 오브젝트들이 IAttackable을 갖는다
{
    void AttackedByPlayer(int damage, int attackCount);
    GameObject gameObject { get; }
}

public class AttackState : BaseState
{
    private AttackCard card;
    private int targetCountLeft;
    List<IAttackable> attackableList = new List<IAttackable>();
    List<IAttackable> selectedAttackableList = new List<IAttackable>();
    List<int> prevEnemyShield = new List<int>();

    struct coord
    {
        public int row;
        public int col;
        public coord(int r, int c)
        {
            this.row = r;
            this.col = c;
        }
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
        foreach (Enemy enemy in EnemyManager.Instance.EnemyList)
        {
            this.prevEnemyShield.Add(enemy.EnemyShield);
        }

        coord[] coords =
        {
            new coord(playerRow - 1, playerCol), new coord(playerRow + 1, playerCol),
            new coord(playerRow, playerCol - 1), new coord(playerRow, playerCol + 1)
        };

        if (isMonster)
        {
            List<Enemy> enemyList = EnemyManager.Instance.EnemyList;
            attackableList.AddRange(enemyList);
        }

        if (isWall)
        {
            foreach (coord c in coords)
            {
                //row col이 0,3 미만이고, 그 좌표에 Wall이 있을 때
                if (c.row >= 0 && c.row < 3 && c.col >= 0 && c.col < 3 &&
                    BoardManager.Instance.BoardObjects[c.row][c.col] == BoardObject.Wall)
                    attackableList.Add(BoardManager.Instance.BoardAttackables[c.row][c.col]);
            }
        }

        /*
        if (isMinion)
        {
            foreach (coord c in coords)
            {
                //row col이 0,3 미만이고, 그 좌표에 Minion이 있을 때
                if (c.row >= 0 && c.row < 3 && c.col >= 0 && c.col < 3 &&
                    BoardManager.Instance.BoardObjects[c.row][c.col] == BoardObject.Minion)
                    attackableList.Add(BoardManager.Instance.BoardAttackables[c.row][c.col]);
            }
        }
        */
        
        if(attackableList.Count > 1)
            Debug.Log(attackableList[1]);
        
        //모든 attack 가능한 오브젝트를 attackableList에 담았음
        //공격 가능한 대상의 테두리를 밝은 파란 테두리로 표시
        foreach (IAttackable attackable in attackableList)
        {
            if(attackable.gameObject.GetComponent<Enemy>() != null)
            {
                attackable.gameObject.GetComponent<Enemy>().EnemyOutlineEffect();
            }
            attackable.gameObject.GetComponent<Outlinable>().enabled = true;
            attackable.gameObject.GetComponent<Outlinable>().OutlineParameters.Color = Color.blue;
        }

        //targetCount가 1이 아닌 경우, 바로 처리(0: 전체 공격/2 이상: 랜덤 공격)
        if (card.TargetCount == 0)
        {
            //전부 공격
            for (int i = 0; i < attackableList.Count; i++)
            {
                selectedAttackableList.Add(attackableList[i]);
            }

            AttackSelectedAttackableList();
        }

        if (card.TargetCount > 1)
        {
            if (card.TargetCount <= attackableList.Count) //공격 가능한 개체 수가 TargetCount보다 많거나 같은 경우
            {
                //card.TargetCount만큼 랜덤 공격
                for (int i = 0; i < card.TargetCount; i++)
                {
                    int rand = Random.Range(0, attackableList.Count);
                    selectedAttackableList.Add(attackableList[rand]);
                    attackableList.Remove(attackableList[rand]);
                }

                AttackSelectedAttackableList();
            }
            else //공격 가능한 개체 수가 TargetCount보다 적은 경우
            {
                //전부 공격
                for (int i = 0; i < attackableList.Count; i++)
                {
                    selectedAttackableList.Add(attackableList[i]);
                }

                AttackSelectedAttackableList();
            }
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
            IAttackable iAttackable = hitData.transform.GetChild(0).gameObject.GetComponent<IAttackable>();
            if(iAttackable == null)
                iAttackable = hitData.transform.gameObject.GetComponent<IAttackable>();

            if (iAttackable != null) //레이캐스트에 맞은 오브젝트에 Iattackable 컴포넌트가 있는가?
            {
                if (attackableList.Contains(iAttackable)) //attackableList에 있는가?
                {
                    selectedAttackableList.Add(iAttackable); //공격할 오브젝트 리스트에 추가
                    Debug.Log("HIT");
                    AttackSelectedAttackableList();
                }
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("AdditionalEffectCondition: " + card.AdditionalEffectCondition);
        Debug.Log("AdditionalEffect: " + card.AdditionalEffect);
        //attackableList 초기화
        DoAdditionalEffect();
        attackableList.Clear();
        selectedAttackableList.Clear();
    }

    private void DoAdditionalEffect()
    {
        bool proceed = false;
        List<IAttackable> additionalEffectParam = new List<IAttackable>();
        switch (this.card.AdditionalEffectCondition)
        {
            case AdditionalEffectCondition.PlayerHealthUnder50Percent: // 플레이어의 체력이 50% 이하일 때
            {
                if (PlayerManager.Instance.Hp * 2 <= PlayerManager.Instance.MaxHp)
                    proceed = true;
                additionalEffectParam = new List<IAttackable>(this.selectedAttackableList);
                break;
            }
            case AdditionalEffectCondition.PlayerInColoredSpace: // 현재 플레이어가 있는 칸이 색칠(아군)된 칸일 때
            {
                (int r, int c) = (PlayerManager.Instance.Row, PlayerManager.Instance.Col);
                Debug.Log(BoardManager.Instance.BoardColors[r][c]);
                if (BoardManager.Instance.BoardColors[r][c] == BoardColor.Player)
                    proceed = true;
                additionalEffectParam = new List<IAttackable>(this.selectedAttackableList);
                break;
            }
            case AdditionalEffectCondition.DeckTopIsAttackCard: // 덱 맨 위의 카드가 공격 카드였을 때
            {
                if (CardManager.Instance.DeckList.Peek().Card is AttackCard)
                    proceed = true;
                additionalEffectParam = new List<IAttackable>(this.selectedAttackableList);
                break;
            }
            case AdditionalEffectCondition.DestroyWallOrMinion: // 벽이나 하수인을 파괴했을 때
            {
                foreach (IAttackable attackable in this.selectedAttackableList)
                {
                    if (attackable is Wall)
                    {
                        Wall wall = attackable as Wall;
                            Debug.Log("WALL ", wall);
                        if (wall.WallHP <= 0)
                        {
                            proceed = true;
                            additionalEffectParam.Add(wall);
                        }
                    }
                    else if (attackable is Minion)
                    {
                        Minion minion = attackable as Minion;
                        if (minion.MinionHP <= 0)
                        {
                            proceed = true;
                            additionalEffectParam.Add(minion);
                        }
                    }
                }

                break;
            }
            case AdditionalEffectCondition.MonsterWillAttack: // 그 몬스터의 의도가 공격일 때
            {
                foreach (IAttackable attackable in this.selectedAttackableList)
                {
                    if (attackable is Enemy)
                    {
                        Enemy enemy = attackable as Enemy;
                        EnemyAction enemyAction = enemy.EnemyActions.Peek().Item1;
                        switch (enemyAction)
                        {
                            case EnemyAction.AllAttack:
                            case EnemyAction.ColoredAttack:
                            case EnemyAction.NoColoredAttack:
                            case EnemyAction.H1Attack:
                            case EnemyAction.H2Attack:
                            case EnemyAction.V1Attack:
                            case EnemyAction.V2Attack:
                                proceed = true;
                                additionalEffectParam.Add(enemy);
                                break;
                        }
                    }
                }

                break;
            }
            case AdditionalEffectCondition.DestroyShield: // 그 몬스터의 방어도를 방금 파괴했을 때
            {
                for (int i = 0; i < this.prevEnemyShield.Count; i++)
                {
                    if (EnemyManager.Instance.EnemyList[i].EnemyShield <= 0 && this.prevEnemyShield[i] > 0) // 이 공격으로 방어도를 파괴했음
                    {
                        proceed = true;
                        additionalEffectParam.Add(EnemyManager.Instance.EnemyList[i]);
                    }

                }

                break;
            }
            case AdditionalEffectCondition.None:
            {
                additionalEffectParam = new List<IAttackable>(this.selectedAttackableList);
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
            case AdditionalEffect.Draw1: // 카드 한 장 뽑음
            {
                CardManager.Instance.DrawCard(1);
                break;
            }
            case AdditionalEffect.PlayerHpPlus20: // 플레이어 20 회복
            {
                PlayerManager.Instance.SetHp(20);
                break;
            }
            case AdditionalEffect.PlayerHpMinus20: // 플레이어에게 피해 20
            {
                PlayerManager.Instance.SetHp(-20);
                break;
            }
            case AdditionalEffect.BuffPlayer: // 플레이어 강화
            {
                PlayerManager.Instance.SetDebuff(Debuff.PowerIncrease, 1);
                break;
            }
            case AdditionalEffect.Move: // 그 칸으로 이동
            {
                foreach (IAttackable attackable in additionalEffectParam)
                {
                    if (attackable is Wall)
                    {
                        Wall wall = attackable as Wall;
                        PlayerManager.Instance.MovePlayer(wall.Row, wall.Col);
                    }
                    else if (attackable is Minion)
                    {
                        Minion minion = attackable as Minion;
                        PlayerManager.Instance.MovePlayer(minion.Row, minion.Col);
                    }
                }

                break;
            }
            case AdditionalEffect.Color: // 그 칸을 색칠
            {
                foreach (IAttackable attackable in additionalEffectParam)
                {
                    if (attackable is Wall)
                    {
                        Wall wall = attackable as Wall;
                        BoardManager.Instance.ColoringBoard(wall.Row, wall.Col, BoardColor.Player);
                    }
                    else if (attackable is Minion)
                    {
                        Minion minion = attackable as Minion;
                        BoardManager.Instance.ColoringBoard(minion.Row, minion.Col, BoardColor.Player);
                    }
                }

                break;
            }
            case AdditionalEffect.MaxMonsterShieldMinus10: // 그 몬스터의 최대실드 -10
            {
                foreach (IAttackable attackable in additionalEffectParam)
                {
                    if (attackable is Enemy)
                    {
                        Enemy enemy = attackable as Enemy;
                        enemy.EnemyMaxShield -= 10;
                    }
                }

                break;
            }
            case AdditionalEffect.MonsterHpMinus1: // 그 몬스터의 체력 -1
            {
                foreach (IAttackable attackable in additionalEffectParam)
                {
                    if (attackable is Enemy)
                    {
                        Enemy enemy = attackable as Enemy;
                        enemy.EnemyHP -= 1;
                        enemy.EnemyUI.HPUIUpdate();
                    }
                }

                break;
            }
            case AdditionalEffect.BuffMonster: // 그 몬스터를 강화
            {
                foreach (IAttackable attackable in additionalEffectParam)
                {
                    if (attackable is Enemy)
                    {
                        Enemy enemy = attackable as Enemy;
                        enemy.SetDebuff(Debuff.PowerIncrease, 20);
                    }
                }

                break;
            }
            case AdditionalEffect.DebuffMonster: // 그 몬스터를 약화
            {
                foreach (IAttackable attackable in additionalEffectParam)
                {
                    if (attackable is Enemy)
                    {
                        Enemy enemy = attackable as Enemy;
                        enemy.SetDebuff(Debuff.PowerDecrease, 20);
                    }
                }

                break;
            }
            case AdditionalEffect.DMG10: // 그 적에게 추가로 피해 10
            {
                foreach (IAttackable attackable in additionalEffectParam)
                {
                    attackable.AttackedByPlayer(10, 1);
                }

                break;
            }
            case AdditionalEffect.DMG20: // 그 적에게 추가로 피해 20
            {
                foreach (IAttackable attackable in additionalEffectParam)
                {
                    attackable.AttackedByPlayer(20, 1);
                }

                break;
            }
            case AdditionalEffect.DMG30: // 그 적에게 추가로 피해 30
            {
                foreach (IAttackable attackable in additionalEffectParam)
                {
                    attackable.AttackedByPlayer(30, 1);
                }

                break;
            }
        }
    }

    private void AttackSelectedAttackableList()
    {
        int damage = card.Damage;

        if (PlayerManager.Instance.DebuffDictionary[Debuff.PowerIncrease] > 0)
            damage = (int)(damage * (1 + PlayerManager.Instance.DebuffDictionary[Debuff.PowerIncrease] / 100f));
        if (PlayerManager.Instance.DebuffDictionary[Debuff.PowerDecrease] > 0)
            damage = (int)(damage * (1 - PlayerManager.Instance.DebuffDictionary[Debuff.PowerDecrease] / 100f));

        foreach (IAttackable selectedAttackable in selectedAttackableList)
        {
            selectedAttackable.AttackedByPlayer(damage, card.AttackCount); //Damage 줌
        }

        foreach (IAttackable attackable in attackableList)
        {
            if (attackable.gameObject.GetComponent<Enemy>() != null)
            {
                attackable.gameObject.GetComponent<Enemy>().StopEnemyOutlineEffect();
            }
            attackable.gameObject.GetComponent<Outlinable>().enabled = false;
        }

        PlayerManager.Instance.EndCurrentState();
    }
}

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
        prevBingoCount = BoardManager.Instance.CheckBingo(BoardColor.Player);

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
            case ColorTargetPosition.Vertical:
                for (int i = 0; i < boardsize; i++)
                {
                    IfColorableAddToList(i, Col);
                }

                break;
            case ColorTargetPosition.Horizontal:
                for (int i = 0; i < boardsize; i++)
                {
                    IfColorableAddToList(Row, i);
                }

                break;
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
                if (BoardManager.Instance.CheckBingo(BoardColor.Player) > this.prevBingoCount)
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
            case AdditionalEffect.Move: // 그 칸으로 이동
            {
                if (this.clickedCoord.row >= 0 && this.clickedCoord.col >= 0)
                {
                    PlayerManager.Instance.MovePlayer(this.clickedCoord.row, this.clickedCoord.col);
                }

                break;
            }
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
        if(BoardManager.Instance.BoardColors[i][j] == BoardColor.Player){
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

public class DumpState : BaseState
{
    CardType dumpCardType;
    int dumpNum;
    List<CardUI> dumpableCardUIs = new List<CardUI>();

    public DumpState(CardType cardType, int dumpNum)
    {
        this.dumpCardType = cardType;
        this.dumpNum = dumpNum;
    }

    public override void DoAction(States state)
    {

    }

    public override void Enter()
    {
        // 버릴 수 있는 카드들을 리스트에 저장, 카드 Outline 설정
        foreach (CardUI cardui in CardManager.Instance.HandCardList)
        {
            if (cardui.Card.CardType == dumpCardType)
            {
                dumpableCardUIs.Add(cardui);
                cardui.GetComponent<Outlinable>().enabled = true;
            }
        }
    }

    public override void Update()
    {
    }

    public override void MouseEvent()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData))
        {
            CardUI cardui = hitData.transform.gameObject.GetComponent<CardUI>();
            if (cardui != null && dumpableCardUIs.Contains(cardui))
            {
                // 카드 클릭 시 카드 버리기
                cardui.GetComponent<Outlinable>().enabled = false;
                CardManager.Instance.HandtoGrave(CardManager.Instance.HandCardList.IndexOf(cardui));
                PlayerManager.Instance.EndCurrentState();
            }
        }
    }

    public override void Exit()
    {
        // 카드 Outline 해제
        foreach (CardUI cardui in CardManager.Instance.HandCardList)
            cardui.GetComponent<Outlinable>().enabled = false;
        return;
    }
}
