using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    private int _enemyShield;
    public int EnemyShield
    {
        get { return _enemyShield; }
        set
        {
            if (value < 0) _enemyShield = 0;
            else _enemyShield = value;
        }
    }
    
    private int _enemyMaxHp;
    public int EnemyMaxHP
    {
        get { return _enemyMaxHp; }
        set
        {
            if (value < 0) _enemyMaxHp = 0;
            else _enemyMaxHp = value;
        }
    }

    private int _enemyHp;
    public int EnemyHP
    {
        get { return _enemyHp; }
        set
        {
            if (value < 0) _enemyHp = 0;
            else if(value > _enemyMaxHp) _enemyHp = _enemyMaxHp;
            else _enemyHp = value;
        }
    }

    private int _enemyPower;
    public int EnemyPower => _enemyPower;

    private int _previousPlayerRow;
    private int _previousPlayerCol;

    private Dictionary<Debuff,  int> _debuffDictionary;
    public Dictionary<Debuff, int> DebuffDictionary => _debuffDictionary;

    private (EnemyAction, EnemyAction) _previousAttack = (EnemyAction.None, EnemyAction.None);
    private List<(int, int)> overlapPoint;

    public Queue<(EnemyAction, int)> EnemyActions;
    public void ReduceHP(int damage)
    {
        EnemyHP -= damage;
    }
    
    void Start()
    {
        
    }

    public void InitEnemyData(EnemyDataHolder enemyDataHolder)
    {
        _previousAttack = (EnemyAction.None, EnemyAction.None);
        _enemyName = enemyDataHolder.EnemyName;
        _enemyHp = enemyDataHolder.EnemyHP;
        _enemyShield = enemyDataHolder.EnemyShield;
        EnemyActions = enemyDataHolder.EnemyAction;
        _debuffDictionary = new Dictionary<Debuff, int>();
        foreach(Debuff debuff in Enum.GetValues(typeof(Debuff)))
            _debuffDictionary[debuff] = 0;
        overlapPoint = new List<(int, int)>();
    }
    
    /// <summary>
    /// �� Enemy�� EnemyActions Queue���� EnemyAction �ϳ��� dequeue�� ��, �ش� Action�� �°� �ൿ�� ��, �ٽ� enqueue�Ѵ�.
    /// </summary>
    public void DoEnemyAction()
    {
        (EnemyAction, int) enemyAction = EnemyActions.Dequeue();
        
        switch ((int)enemyAction.Item1 / 10)
        {
            case 0:
                EnemyAttack(enemyAction);
                break;
            case 1:
                EnemySummon(enemyAction);
                break;
            case 2:
                EnemyBuff(enemyAction);
                break;
            case 3:
                EnemyDebuff(enemyAction);
                break;
        }
        
        EnemyActions.Enqueue(enemyAction);
    }

    private void EnemyAttack((EnemyAction, int) enemyAction)
    {
        int damage = enemyAction.Item2;

        if(_debuffDictionary[Debuff.PowerIncrease] > 0)
            damage = (int)(damage * 1.2);
        if(_debuffDictionary[Debuff.PowerDecrease] > 0)
            damage = (int)(damage * 0.8);

        _previousAttack.Item2 = _previousAttack.Item1;
        _previousAttack.Item1 = enemyAction.Item1;

        Debug.Log(_previousPlayerCol);

        switch (enemyAction.Item1)
        {
            case EnemyAction.H1Attack:
                if(PlayerManager.Instance.Col == _previousPlayerCol){
                    PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.V1Attack:
                if(PlayerManager.Instance.Row == _previousPlayerRow){
                    PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.H2Attack:
                if(PlayerManager.Instance.Col == _previousPlayerCol){
                    PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.V2Attack:
                if(PlayerManager.Instance.Row == _previousPlayerRow){
                    PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.ColoredAttack:
                if(BoardManager.Instance.BoardColors
                [PlayerManager.Instance.Row][PlayerManager.Instance.Col] != BoardColor.None){
                    PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.NoColoredAttack:
                if(BoardManager.Instance.BoardColors
                [PlayerManager.Instance.Row][PlayerManager.Instance.Col] == BoardColor.None){
                    PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.AllAttack:
                PlayerManager.Instance.DamageToPlayer(-damage);
                break;
        }
    }
    
    private void EnemySummon((EnemyAction, int) enemyAction)
    {
        int damage = enemyAction.Item2;

        if(_debuffDictionary[Debuff.PowerIncrease] > 0)
            damage = (int)(damage * 1.2);
        if(_debuffDictionary[Debuff.PowerDecrease] > 0)
            damage = (int)(damage * 0.8);

        foreach((int, int) elems in overlapPoint)
            BoardManager.Instance.SummonWalls(elems.Item1, elems.Item2, damage);
    }

    public void GetOverLapPosition((EnemyAction, int) enemyAction)
    {
        overlapPoint.Clear();

        List<(int, int)> temp = new List<(int, int)>();
        List<(int, int)> result = new List<(int, int)>();

        switch(_previousAttack.Item1){
            case EnemyAction.H1Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                    temp.Add((i, _previousPlayerCol));
                break;
            case EnemyAction.V1Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                    temp.Add((_previousPlayerRow, i));
                break;
            case EnemyAction.H2Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(j == _previousPlayerCol) continue;
                        else temp.Add((i, j));
                    }
                }
                break;
            case EnemyAction.V2Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(j == _previousPlayerRow) continue;
                        temp.Add((j, i));
                    }
                }
                break;
            case EnemyAction.ColoredAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(BoardManager.Instance.BoardColors[i][j] == BoardColor.None) continue;
                        temp.Add((i, j));
                    }
                }
                break;
            case EnemyAction.NoColoredAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(BoardManager.Instance.BoardColors[i][j] != BoardColor.None) continue;
                        temp.Add((i, j));
                    }
                }
                break;
            case EnemyAction.AllAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                        temp.Add((i, j));
                break;
        }

        switch(_previousAttack.Item2){
            case EnemyAction.H1Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    if(temp.Contains((i, _previousPlayerCol)))
                        result.Add((i, _previousPlayerCol));
                }
                break;
            case EnemyAction.V1Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    if(temp.Contains((_previousPlayerRow, i)))
                        result.Add((_previousPlayerRow, i));
                }
                break;
            case EnemyAction.H2Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(j == _previousPlayerCol) continue;
                        else if(temp.Contains((i, j)))
                            result.Add((i, j));
                    }
                }
                break;
            case EnemyAction.V2Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(j == _previousPlayerRow) continue;
                        else if(temp.Contains((j, i)))
                            result.Add((j, i));
                    }
                }
                break;
            case EnemyAction.ColoredAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(BoardManager.Instance.BoardColors[i][j] == BoardColor.None) continue;
                        else if(temp.Contains((i, j)))
                            result.Add((i, j));
                    }
                }
                break;
            case EnemyAction.NoColoredAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(BoardManager.Instance.BoardColors[i][j] != BoardColor.None) continue;
                        else if(temp.Contains((i, j)))
                            result.Add((i, j));
                    }
                }
                break;
            case EnemyAction.AllAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(temp.Contains((i, j))){
                            result.Add((i, j));
                        }
                    }
                }
                break;
        }

        int index;
        switch (enemyAction.Item1)
        {
            case EnemyAction.WallSummon:
                index = UnityEngine.Random.Range(0, overlapPoint.Count-1);
                overlapPoint.Add(result[index]);
                break;
            case EnemyAction.WallsSummon:
                overlapPoint = result;
                break;
        }

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
            case EnemyAction.ArmorHealing:
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

    public bool DamagetoEnemy(int damage)
    {
        if (this.EnemyHP < damage)
        {
            Destroy(gameObject);
            return true;
        }
        else
            return false;
    }
}