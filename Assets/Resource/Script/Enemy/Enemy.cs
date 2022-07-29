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
            else _enemyHp = value;
        }
    }

    public Queue<EnemyAction> EnemyActions;
    
    void Start()
    {
        
    }

    public void InitEnemyData(EnemyDataHolder enemyDataHolder)
    {
        _enemyName = enemyDataHolder.EnemyName;
        _enemyHp = enemyDataHolder.EnemyHP;
        _enemyShield = enemyDataHolder.EnemyShield;
        EnemyActions = enemyDataHolder.EnemyAction;
    }
    
    public void EnemyAction()
    {
        EnemyAction enemyAction = EnemyActions.Dequeue();
            
        switch ((int)enemyAction / 10)
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

    private void EnemyAttack(EnemyAction enemyAction)
    {
        switch (enemyAction)
        {
            case global::EnemyAction.RowAttack:
                break;
            case global::EnemyAction.ColAttack:
                break;
            case global::EnemyAction.ColorAttack:
                break;
            case global::EnemyAction.UnColorAttack:
                break;
            case global::EnemyAction.AllAttack:
                break;
        }
    }
    
    private void EnemySummon(EnemyAction enemyAction)
    {
        switch (enemyAction)
        {
            case global::EnemyAction.MobSummon:
                break;
            case global::EnemyAction.WallSummon:
                break;
        }
    }
    
    private void EnemyBuff(EnemyAction enemyAction)
    {
        switch (enemyAction)
        {
            case global::EnemyAction.PowerIncrease:
                break;
            case global::EnemyAction.DamageDecrease:
                break;
            case global::EnemyAction.HpHealing:
                break;
            case global::EnemyAction.ArmorHealing:
                break;
        }
    }
    
    private void EnemyDebuff(EnemyAction enemyAction)
    {
        switch (enemyAction)
        {
            case global::EnemyAction.PlayerPowerDecrease:
                break;
            case global::EnemyAction.PlayerDamageIncrease:
                break;
            case global::EnemyAction.DrawCardDecrease:
                break;
            case global::EnemyAction.CardCostIncrease:
                break;
        }
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