using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;
using DG.Tweening;

public class Enemy : MonoBehaviour, IAttackable
{
    [SerializeField] private ParticleSystem deathParticle;
    private string _enemyName;
    public string EnemyName => _enemyName;
    
    private int _enemyMaxShield;
    public int EnemyMaxShield
    {
        get { return _enemyMaxShield; }
        set
        {
            if (value < 0) _enemyMaxShield = 0;
            else _enemyMaxShield = value;
        }
    }
    
    [SerializeField] private int _enemyShield;
    public int EnemyShield
    {
        get { return _enemyShield; }
        set { _enemyShield = value; }
    }
    
    private int _enemyMaxHp;
    public int EnemyMaxHP
    {
        get { return _enemyMaxHp; }
        set
        {
            // TODO: enemyMaxHp가 현재 Hp보다 낮아지면 현재 Hp도 깎아야 하지 않을까?
            if (value < 0) _enemyMaxHp = 0;
            else _enemyMaxHp = value;
        }
    }

    [SerializeField] private int _enemyHp;
    public int EnemyHP
    {
        get { return _enemyHp; }
        set { _enemyHp = value; }
    }

    private int _enemyPower;
    public int EnemyPower => _enemyPower;

    private int _previousPlayerRow;
    public int PreviousPlayerRow => _previousPlayerRow;
    private int _previousPlayerCol;
    public int PreviousPlayerCol => _previousPlayerCol;

    private Dictionary<Debuff,  int> _debuffDictionary;
    public Dictionary<Debuff, int> DebuffDictionary => _debuffDictionary;

    private (EnemyAction, EnemyAction) _previousAttack = (EnemyAction.None, EnemyAction.None);

    private List<(int, int)> temp1;
    private List<(int, int)> temp2;
    private List<(int, int)> overlapPoint;

    public Queue<(EnemyAction, int)> EnemyActions;
    
    public void AttackedByPlayer(int damage, int attackCount)
    {
        StartCoroutine(AttackedByPlayerCoroutine(damage, attackCount));
    }

    public IEnumerator AttackedByPlayerCoroutine(int damage, int attackCount)
    {
        for (int i = 0; i < attackCount; i++)
        {
            EnemyShield = EnemyShield > damage ? EnemyShield - damage : 0;

            PlayAttackFromPlayerEffect();

            if(PlayerManager.Instance.GOD)
            {
                EnemyManager.Instance.EnemyList.Remove(this);
                StartCoroutine(EnemyDeathCoroutine());
            }

            if (EnemyShield == 0)
            {
                int bingoCount = BoardManager.Instance.CheckBingo(BoardColor.Player);
                
                if (bingoCount > 0)
                {
                    for (int ii = 0; ii < BoardManager.Instance.BoardSize; ii++)
                    {
                        for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                        {
                            BoardManager.Instance.GameBoard[ii][j].SetBoardColor(BoardColor.None);
                            BoardManager.Instance.GameBoard[ii][j].ActivateBingoEffect(false);
                        }
                    }

                    _enemyHp -= (int) Math.Pow(2, bingoCount - 1);
                    PlayerManager.Instance.BingoAttack = true;
                }
                else
                {
                    if (PlayerManager.Instance.TutorialPhase == 4)
                        PlayerManager.Instance.tutorial4Trigger = true;
                }
                if (EnemyHP <= 0)
                {
                    EnemyManager.Instance.EnemyList.Remove(this);
                    StartCoroutine(EnemyDeathCoroutine());
                }
            }
            
            EnemyUI.ShieldUIUpdate();
            EnemyUI.HPUIUpdate();
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator EnemyDeathCoroutine()
    {
        transform.parent.DORotate(new Vector3(0, 0, 0), 0.6f, RotateMode.Fast).SetEase(Ease.InQuart);

        yield return new WaitForSeconds(1.0f);

        deathParticle.Play();

        yield return new WaitForSeconds(1.0f);

        GameManager.Instance.GameClear();
        Destroy(this.gameObject);
    }

    private void PlayAttackFromPlayerEffect()
    {
        int leftOrRight = Random.Range(0, 2);
        int angle;
        if (leftOrRight == 0)
            angle = Random.Range(10, 70);
        else
            angle = Random.Range(-10, -70);
        print(angle);
        Quaternion rotation = Quaternion.Euler(angle, -90, 90);
        Instantiate(PlayerManager.Instance.AttackEffect, transform.position + new Vector3(0, 0, -3), rotation);  // 자동으로 destroy된다.
    }

    public EnemyUI EnemyUI;

    public void InitEnemyData(EnemyDataHolder enemyDataHolder)
    {
        temp1 = new List<(int, int)>();
        temp2 = new List<(int, int)>();
        _previousAttack = (EnemyAction.None, EnemyAction.None);
        _enemyName = enemyDataHolder.EnemyName;
        _enemyMaxHp = enemyDataHolder.EnemyHP;
        _enemyMaxShield = enemyDataHolder.EnemyShield;
        _enemyHp = enemyDataHolder.EnemyHP;
        _enemyShield = enemyDataHolder.EnemyShield;
        EnemyActions = enemyDataHolder.EnemyAction;
        _debuffDictionary = new Dictionary<Debuff, int>();
        gameObject.GetComponent<Outlinable>().enabled = false;
        foreach(Debuff debuff in Enum.GetValues(typeof(Debuff)))
            _debuffDictionary[debuff] = 0;
        overlapPoint = new List<(int, int)>();
        EnemyUI.InitUI();
    }
    
    /// <summary>
    /// �� Enemy�� EnemyActions Queue���� EnemyAction �ϳ��� dequeue�� ��, �ش� Action�� �°� �ൿ�� ��, �ٽ� enqueue�Ѵ�.
    /// </summary>
    public bool DoEnemyAction()
    {
        (EnemyAction, int) enemyAction = EnemyActions.Dequeue();
        bool _isGameOver = false;
        
        Debug.Log(enemyAction.Item1);

        switch ((int)enemyAction.Item1 / 10)
        {
            case 0:
                _isGameOver = EnemyAttack(enemyAction);
                break;
            case 1:
                _isGameOver = EnemySummon(enemyAction);
                break;
            case 2:
                EnemyBuff(enemyAction);
                break;
            case 3:
                EnemyDebuff(enemyAction);
                break;
        }

        EnemyActions.Enqueue(enemyAction);

        EnemyUI.IntentionUpdate();

        return _isGameOver;
    }

    private bool EnemyAttack((EnemyAction, int) enemyAction)
    {
        int damage = enemyAction.Item2;

        bool _isGameOver = false;

        if(_debuffDictionary[Debuff.PowerIncrease] > 0)
            damage = (int)(damage * (1 + _debuffDictionary[Debuff.PowerIncrease] / 100));
        if(_debuffDictionary[Debuff.PowerDecrease] > 0)
            damage = (int)(damage * (1 - _debuffDictionary[Debuff.PowerIncrease] / 100));

        _previousAttack.Item2 = _previousAttack.Item1;
        _previousAttack.Item1 = enemyAction.Item1;
        
        temp1.Clear();
        
        foreach (var elem in temp2)
            temp1.Add(elem);
            
        temp2.Clear();

        switch (enemyAction.Item1)
        {
            case EnemyAction.H1Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                {
                    temp2.Add((_previousPlayerRow, i));
                    if(BoardManager.Instance.BoardObjects[_previousPlayerRow][i] == BoardObject.Player)
                        _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.V1Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                {
                    temp2.Add((i, _previousPlayerCol));
                    Debug.Log(BoardManager.Instance.BoardObjects[i][_previousPlayerCol]);
                    if(BoardManager.Instance.BoardObjects[i][_previousPlayerCol] == BoardObject.Player)
                        _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.H2Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                {
                    if (i == _previousPlayerRow) continue;
                    else
                    {
                        for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                        {
                            temp2.Add((i, j));
                            if(BoardManager.Instance.BoardObjects[i][j] == BoardObject.Player)
                                _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                        }
                    }
                }
                break;
            case EnemyAction.V2Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                {
                    if (i == _previousPlayerCol) continue;
                    else
                    {
                        for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                        {
                            temp2.Add((j, i));
                            if(BoardManager.Instance.BoardObjects[j][i] == BoardObject.Player)
                                _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                        }
                    }
                }
                break;
            case EnemyAction.ColoredAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(BoardManager.Instance.BoardColors[i][j] == BoardColor.None) continue;
                        else{
                            if(BoardManager.Instance.BoardObjects[i][j] == BoardObject.Player)
                                _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                            temp2.Add((i, j));
                        }
                    }
                }
                break;
            case EnemyAction.NoColoredAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(BoardManager.Instance.BoardColors[i][j] != BoardColor.None) continue;
                        else{
                            if(BoardManager.Instance.BoardObjects[i][j] == BoardObject.Player)
                                _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                            temp2.Add((i, j));
                        }
                    }
                }
                break;
            case EnemyAction.AllAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                        temp2.Add((i, j));
                _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                break;
        }

        StartCoroutine(PlayEnemyAttackEffect(temp2, _isGameOver));

        return _isGameOver;
    }

    private IEnumerator PlayEnemyAttackEffect(List<(int, int)> attackedSpaces, bool isGameOver)
    {
        int boardSize = BoardManager.Instance.BoardSize;
        bool[,] attacked = new bool[boardSize, boardSize];  // 모두 false로 초기화
        foreach ((int, int) coord in attackedSpaces)
            attacked[coord.Item1, coord.Item2] = true;

        // 위부터 아래 방향으로 순서대로 쾅쾅쾅 터뜨리는 코드
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                if (attacked[r, c])
                {
                    Vector3 position = BoardManager.Instance.GameBoard[r][c].transform.position;
                    Instantiate(EnemyManager.Instance.EnemyAttackEffect, position + new Vector3(0, 0, -6), Quaternion.identity);  // 자동으로 destroy된다.
                }
            }
            yield return new WaitForSeconds(0.3f);
        }

        if(isGameOver)
            GameManager.Instance.GameOver();
    }

    private bool EnemySummon((EnemyAction, int) enemyAction)
    {
        bool _isGameOver = false;
        
        int damage = enemyAction.Item2;

        if(_debuffDictionary[Debuff.PowerIncrease] > 0)
            damage = (int)(damage * (1 + _debuffDictionary[Debuff.PowerIncrease] / 100));
        if(_debuffDictionary[Debuff.PowerDecrease] > 0)
            damage = (int)(damage * (1 - _debuffDictionary[Debuff.PowerIncrease] / 100));

        foreach((int, int) elems in overlapPoint)
            _isGameOver = BoardManager.Instance.SummonWalls(elems.Item1, elems.Item2, damage);

        return _isGameOver;
    }

    public void GetOverLapPosition((EnemyAction, int) enemyAction)
    {
        overlapPoint.Clear();

        if(temp1.Count == 0 || temp2.Count == 0) return;

        foreach ((int, int) item in temp1)
            if(temp2.Contains(item)) overlapPoint.Add(item);

        if (enemyAction.Item1 == EnemyAction.WallSummon)
        {
            overlapPoint.Shuffle();
            overlapPoint = new List<(int, int)>() {overlapPoint[0]};
        }
    }

    public void HighlightOverlapPoint()
    {
        foreach ((int, int) p in overlapPoint)
            BoardManager.Instance.GameBoard[p.Item1][p.Item2].SetHighlight(BoardSituation.WillSummon);
    }
    
    private void EnemyBuff((EnemyAction, int) enemyAction)
    {
        switch (enemyAction.Item1)
        {
            case EnemyAction.PowerIncrease:
                SetDebuff(Debuff.PowerIncrease, enemyAction.Item2);
                break;
            case EnemyAction.DamageDecrease:
                break;
            case EnemyAction.HpHealing:
                SetDebuff(Debuff.Heal, enemyAction.Item2);
                break;
            case EnemyAction.ShieldHealing:
                break;
        }
        EnemyUI.BuffDebuffUpdate();
    }
    
    private void EnemyDebuff((EnemyAction, int) enemyAction)
    {
        switch (enemyAction.Item1)
        {
            case EnemyAction.PlayerPowerDecrease:
                PlayerManager.Instance.SetDebuff(Debuff.PowerDecrease, enemyAction.Item2);
                break;
            case EnemyAction.PlayerDamageIncrease:
                PlayerManager.Instance.SetDebuff(Debuff.DamageIncrease, enemyAction.Item2);
                break;
            case EnemyAction.DrawCardDecrease:
                PlayerManager.Instance.SetDebuff(Debuff.DrawCardDecrease, enemyAction.Item2);
                break;
            case EnemyAction.CardCostIncrease:
                PlayerManager.Instance.SetDebuff(Debuff.CardCostIncrease, enemyAction.Item2);
                break;
        }
        EnemyUI.BuffDebuffUpdate();
    }

    public void SetDebuff(Debuff debuff, int value)
    {
        Debug.Log("SetDebuff");
        if(_debuffDictionary[debuff] + value < 0)
            _debuffDictionary[debuff] = 0;
        else
            _debuffDictionary[debuff] += value;
        if(value >0){
            if(debuff == Debuff.PowerDecrease )
                transform.Find("DebuffEffect").gameObject.SetActive(true);
            else if(debuff == Debuff.Heal)
                transform.Find("HealEffect").gameObject.SetActive(true);
            else
                transform.Find("BuffEffect").gameObject.SetActive(true);
        }
        EnemyUI.BuffDebuffUpdate();
    }

    public void setPreviousPos(int row, int col){
        _previousPlayerRow = row;
        _previousPlayerCol = col;
    }

    public void EnemyHealShield(int num)
    {
        Debug.Log("EnemyHPHeal!");
        _enemyShield = _enemyShield + num > _enemyMaxShield ? _enemyMaxShield : _enemyShield + num;
    }
}