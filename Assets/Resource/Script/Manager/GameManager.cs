using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameClearPanel;
    private int _currentStage = 101;
    public int CurrentStage
    {
        get => _currentStage;
        set => _currentStage = value;
    }
    
    void Awake()
    {
        if (GameManager.Instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GameOver()
    {
        if (PlayerManager.Instance.Hp <= 0)
        {
            gameOverPanel.SetActive(true);
            return;
        }
        else
            return;
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
