using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GameOver()
    {
        
    }

    public void GameClear()
    {
        if (EnemyManager.Instance.EnemyList.Count == 0)
            return;
        else
            return;
    }
}
