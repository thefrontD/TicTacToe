using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{

    private int _playerNum = 1;
    public int PlayerNum
    {
        get => _playerNum;
        set => _playerNum = value;
    }
    [SerializeField] private int _currentStage = 103;

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
        if(Input.GetMouseButtonDown(0))
            SoundManager.Instance.PlaySE("Click", 0.2f);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        PlayerManager.Instance.CardUsable = false;
        DialogueManager.Instance.dialogueCallBack.DialogueCallBack -= PlayerManager.Instance.Init;
        DialogueManager.Instance.dialogueCallBack.DialogueCallBack += gameOverPanelActivation;
        
        BoardManager.Instance.PlayerObject.transform.DORotate(new Vector3(-180, 0, 0), 0.6f, RotateMode.Fast)
        .SetEase(Ease.InQuart);

        yield return new WaitForSeconds(1.0f);

        BoardManager.Instance.PlayerObject.transform.GetChild(0).gameObject.SetActive(false);
        BoardManager.Instance.PlayerObject.transform.GetChild(1).GetComponent<ParticleSystem>().Play();

        yield return new WaitForSeconds(1.0f);

        DialogueManager.Instance.StartDialogue(string.Format("Enemy{0}_defeat", _currentStage%100));
    }

    public void GameClear()
    {
        if (EnemyManager.Instance.EnemyList.Count == 0)
        {
            PlayerManager.Instance.CardUsable = false;

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
        PanelManager.Instance.GameOverPanel.SetActive(true);
    }

    private void gameClearPanelActivation(object sender, EventArgs eventArgs)
    {
        PanelManager.Instance.GameClearPanel.SetActive(true);
        PanelManager.Instance.GameClearPanel.transform.DOLocalMoveY(0, 1f);
    }
}
