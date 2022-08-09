using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameClearPanel;
    
    void Awake()
    {
        if (GameManager.Instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        BoardManager.Instance.BoardLoading("BoardData");
        PlayerManager.Instance.PlayerLoading();
        EnemyManager.Instance.EnemyLoading("Enemy");
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
        {
            gameClearPanel.SetActive(true);
            return;
        }
        else
            return;
    }
}
