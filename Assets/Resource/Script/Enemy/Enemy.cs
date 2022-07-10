using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
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

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
