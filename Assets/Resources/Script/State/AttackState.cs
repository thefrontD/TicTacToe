using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
            case AdditionalEffectCondition.PlayerInColoredSpace: // 현재 플레이어가 있는 칸이 색칠된 칸일 때
            {
                (int r, int c) = (PlayerManager.Instance.Row, PlayerManager.Instance.Col);
                Debug.Log(BoardManager.Instance.BoardColors[r][c]);
                if (BoardManager.Instance.BoardColors[r][c] != BoardColor.None)
                    proceed = true;
                additionalEffectParam = new List<IAttackable>(this.selectedAttackableList);
                break;
            }
            case AdditionalEffectCondition.PlayerInRedColoredSpace: // 현재 플레이어가 있는 칸이 색칠(적)된 칸일 때
            {
                (int r, int c) = (PlayerManager.Instance.Row, PlayerManager.Instance.Col);
                Debug.Log(BoardManager.Instance.BoardColors[r][c]);
                if (BoardManager.Instance.BoardColors[r][c] == BoardColor.Enemy)
                    proceed = true;
                additionalEffectParam = new List<IAttackable>(this.selectedAttackableList);
                break;
            }
            case AdditionalEffectCondition.PlayerInBlueColoredSpace: // 현재 플레이어가 있는 칸이 색칠(적)된 칸일 때
            {
                (int r, int c) = (PlayerManager.Instance.Row, PlayerManager.Instance.Col);
                Debug.Log(BoardManager.Instance.BoardColors[r][c]);
                if (BoardManager.Instance.BoardColors[r][c] == BoardColor.Player)
                    proceed = true;
                additionalEffectParam = new List<IAttackable>(this.selectedAttackableList);
                break;
            }
            case AdditionalEffectCondition.PlayerInNotColoredSpace: // 현재 플레이어가 있는 칸이 색칠되지 않은 칸일 때
            {
                (int r, int c) = (PlayerManager.Instance.Row, PlayerManager.Instance.Col);
                Debug.Log(BoardManager.Instance.BoardColors[r][c]);
                if (BoardManager.Instance.BoardColors[r][c] == BoardColor.None)
                    proceed = true;
                additionalEffectParam = new List<IAttackable>(this.selectedAttackableList);
                break;
            }
            case AdditionalEffectCondition.DeckTopIsAttackCard: // 덱 맨 위의 카드가 공격 카드였을 때
            {
                if (CardManager.Instance.DeckList.Count == 0)
                    break;
                CardUI topCard = CardManager.Instance.DeckList.Peek();
                CardManager.Instance.PeekAndReturn();
                if (topCard.Card is AttackCard)
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
            /*case AdditionalEffectCondition.MonsterWillAttack: // 그 몬스터의 의도가 공격일 때
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
            }*/
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
            /*case AdditionalEffect.Move: // 그 칸으로 이동
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
            }*/
            /*case AdditionalEffect.Color: // 그 칸을 색칠
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
            }*/
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
                        attackable.AttackedByPlayer(0, 1);
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
        }

        PlayerManager.Instance.EndCurrentState();
    }
}