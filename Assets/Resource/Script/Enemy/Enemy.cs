using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QuickOutline;
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
            int bingoCount = BoardManager.Instance.CheckBingo(BoardColor.Player);

            Debug.Log(bingoCount);
            
            _enemyHp -= bingoCount;

            if (EnemyHP <= 0)
            {
                EnemyManager.Instance.EnemyList.Remove(this);
                GameManager.Instance.GameClear();
            }
        }
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public GameObject HP_Container;
    public GameObject ShieldUI;
    public GameObject HPUI;
    
    void Update()
    {
        ShieldUI.GetComponent<Slider>().value = -_enemyShield / _enemyMaxShield;
        ShieldUI.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = _enemyShield.ToString() + "/" + _enemyMaxShield.ToString();
        Debug.Log(_enemyShield.ToString() + " / " + _enemyMaxShield.ToString());
    }

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
        gameObject.GetComponent<QuickOutline.Outline>().enabled = false;
        foreach(Debuff debuff in Enum.GetValues(typeof(Debuff)))
            _debuffDictionary[debuff] = 0;
        overlapPoint = new List<(int, int)>();
    }
    
    /// <summary>
    /// �� Enemy�� EnemyActions Queue���� EnemyAction �ϳ��� dequeue�� ��, �ش� Action�� �°� �ൿ�� ��, �ٽ� enqueue�Ѵ�.
    /// </summary>
    public bool DoEnemyAction()
    {
        (EnemyAction, int) enemyAction = EnemyActions.Dequeue();
        bool _isGameOver = false;
        
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
        return _isGameOver;
    }

    private bool EnemyAttack((EnemyAction, int) enemyAction)
    {
        int damage = enemyAction.Item2;

        bool _isGameOver = false;

        if(_debuffDictionary[Debuff.PowerIncrease] > 0)
            damage = (int)(damage * 1.2);
        if(_debuffDictionary[Debuff.PowerDecrease] > 0)
            damage = (int)(damage * 0.8);

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
                    temp2.Add((i, _previousPlayerCol));
                if(PlayerManager.Instance.Col == _previousPlayerCol){
                    _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.V1Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                    temp2.Add((_previousPlayerRow, i));
                if(PlayerManager.Instance.Row == _previousPlayerRow){
                    _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.H2Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(j == _previousPlayerCol) continue;
                        else temp2.Add((i, j));
                    }
                }
                if(PlayerManager.Instance.Col != _previousPlayerCol){
                    _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.V2Attack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(j == _previousPlayerRow) continue;
                        temp2.Add((j, i));
                    }
                }
                if(PlayerManager.Instance.Row != _previousPlayerRow){
                    _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.ColoredAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(BoardManager.Instance.BoardColors[i][j] == BoardColor.None) continue;
                        temp2.Add((i, j));
                    }
                }
                if(BoardManager.Instance.BoardColors
                [PlayerManager.Instance.Row][PlayerManager.Instance.Col] != BoardColor.None){
                    _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
                }
                break;
            case EnemyAction.NoColoredAttack:
                for (int i = 0; i < BoardManager.Instance.BoardSize; i++){
                    for (int j = 0; j < BoardManager.Instance.BoardSize; j++){
                        if(BoardManager.Instance.BoardColors[i][j] != BoardColor.None) continue;
                        temp2.Add((i, j));
                    }
                }
                if(BoardManager.Instance.BoardColors
                [PlayerManager.Instance.Row][PlayerManager.Instance.Col] == BoardColor.None){
                    _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
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
        bool _isGameOver = false;;
        
        int damage = enemyAction.Item2;

        if(_debuffDictionary[Debuff.PowerIncrease] > 0)
            damage = (int)(damage * 1.2);
        if(_debuffDictionary[Debuff.PowerDecrease] > 0)
            damage = (int)(damage * 0.8);

        foreach((int, int) elems in overlapPoint)
            _isGameOver = BoardManager.Instance.SummonWalls(elems.Item1, elems.Item2, damage);

        return _isGameOver;
    }

    public void GetOverLapPosition((EnemyAction, int) enemyAction)
    {
        overlapPoint.Clear();

        foreach ((int, int) item in temp1)
            if(temp2.Contains(item)) overlapPoint.Add(item);

        if (enemyAction.Item1 == EnemyAction.WallSummon)
        {
            overlapPoint.Shuffle();
            overlapPoint = new List<(int, int)>() {overlapPoint[0]};
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
}