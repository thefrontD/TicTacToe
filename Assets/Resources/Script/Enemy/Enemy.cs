using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour, IAttackable
{
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
    
    public void AttackedByPlayer(int damage)
    {
        EnemyShield = EnemyShield > damage ? EnemyShield - damage : 0;

        if (EnemyShield == 0)
        {
            _enemyHp -= (int)Math.Pow(2, BoardManager.Instance.CheckBingo(BoardColor.Player)-1);

            PlayerManager.Instance.BingoAttack = true;

            if (EnemyHP <= 0)
            {
                EnemyManager.Instance.EnemyList.Remove(this);
                GameManager.Instance.GameClear();
                Destroy(this.gameObject);
            }
        }
        EnemyUI.ShieldUIUpdate();
        EnemyUI.HPUIUpdate();
        //Debug.Log("Buff" + DebuffDictionary[Debuff.PowerIncrease]);
        //Debug.Log("Debuff" + DebuffDictionary[Debuff.PowerDecrease]);
        //EnemyUI.BuffDebuffUpdate();
    }
    public GameObject GetGameObject()
    {
        return gameObject;
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
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    temp2.Add((_previousPlayerRow, i));
                    if(BoardManager.Instance.BoardObjects[_previousPlayerRow][i] == BoardObject.Player)
                        _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.V1Attack:
            for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    temp2.Add((i, _previousPlayerCol));
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

        return _isGameOver;
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
    }

    public void SetDebuff(Debuff debuff, int value)
    {
        if(_debuffDictionary[debuff] + value < 0)
            _debuffDictionary[debuff] = 0;
        else
            _debuffDictionary[debuff] += value;
    }

    public void setPreviousPos(int row, int col){
        _previousPlayerRow = row;
        _previousPlayerCol = col;
    }
}