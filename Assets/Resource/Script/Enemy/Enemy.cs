using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IAttackable
{
    private int enemyShield;
    public int EnemyShield
    {
        get { return enemyShield; }
        set
        {
            if (value < 0) enemyShield = 0;
            else enemyShield = value;
        }
    }

    private int enemyHP;
    public int EnemyHP
    {
        get { return enemyHP; }
        set
        {
            if (value < 0) enemyHP = 0;
            else enemyHP = value;
        }
    }

    public Queue<EnemyAction> EnemyActions;
    
    void Start()
    {
        EnemyActions = EnemyData.Instance._load("EnemyData.json");
    }

    public void EnemyAction()
    {
        EnemyAction enemyAction = EnemyActions.Dequeue();
            
        switch ((int)enemyAction % 10)
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
            case global::EnemyAction.DamageIncrease:
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
            case global::EnemyAction.PlayerDamageDecrease:
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