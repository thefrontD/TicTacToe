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

    private int _playerNum = 0;
    public int PlayerNum
    {
        get => _playerNum;
        set => _playerNum = value;
    }
    private int _currentStage = 102;
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
        DialogueManager.Instance.dialogueCallBack.DialogueCallBack -= PlayerManager.Instance.Init;
        DialogueManager.Instance.dialogueCallBack.DialogueCallBack += gameOverPanelActivation;

        DialogueManager.Instance.StartDialogue(string.Format("Enemy{0}_defeat", _currentStage%100));
    }

    public void GameClear()
    {
        if (EnemyManager.Instance.EnemyList.Count == 0)
        {
            DialogueManager.Instance.dialogueCallBack.DialogueCallBack -= PlayerManager.Instance.Init;
            DialogueManager.Instance.dialogueCallBack.DialogueCallBack += gameClearPanelActivation;
            
            if(PlayerManager.Instance.TutorialTrigger)
            {
                DialogueManager.Instance.dialogueCallBack.DialogueCallBack -= PlayerManager.Instance.NextTutorialNum;
                TutorialManager.Instance.toNextTutorial(PlayerManager.Instance.TutorialPhase);
            }
            else
            {
                DialogueManager.Instance.StartDialogue(string.Format("Enemy{0}_victory", _currentStage % 100));
            }
            return;
        }
        else
            return;
    }

    private void gameOverPanelActivation(object sender, EventArgs eventArgs)
    {
        gameOverPanel.SetActive(true);
    }

    private void gameClearPanelActivation(object sender, EventArgs eventArgs)
    {
        gameClearPanel.SetActive(true);
    }
}
