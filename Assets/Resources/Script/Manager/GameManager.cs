using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private bool isPuzzleMode = false;
    public bool IsPuzzleMode => isPuzzleMode;

    public DataTableBase StageTable { get; private set; }
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

    [SerializeField] private int _currentCol;
    public int CurrentCol
    {
        get => _currentCol;
        set => _currentCol = value;
    }

    [SerializeField] private int _currentRow;
    public int CurrentRow
    {
        get => _currentRow;
        set => _currentRow = value;
    }

    [SerializeField] private int _currentLevel;
    public int CurrentLevel
    {
        get => _currentLevel;
        set => _currentLevel = value;
    }
    
    void Awake()
    {
        PlayerDataHolder _holder = PlayerData.Instance._load(string.Format("PlayerData{0}", GameManager.Instance.PlayerNum));
        _currentCol = _holder.Col;
        _currentRow = _holder.Row;
        if (GameManager.Instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(this.gameObject);
        SetDataTable();
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
        foreach (CardUI card in CardManager.Instance.HandCardList)
        {
            card.isHand = false;
        }
        
        PlayerManager.Instance.CardUsable = false;
        
        BoardManager.Instance.PlayerObject.transform.DORotate(new Vector3(-180, 0, 0), 0.6f, RotateMode.Fast)
        .SetEase(Ease.InQuart);

        yield return new WaitForSeconds(1.0f);

        BoardManager.Instance.PlayerObject.transform.GetChild(0).gameObject.SetActive(false);
        BoardManager.Instance.PlayerObject.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        SoundManager.Instance.PlaySE("Death");

        yield return new WaitForSeconds(1.0f);

        gameOverPanelActivation();
    }

    public void GameClear()
    {
        if(isPuzzleMode){
            if(PuzzleManager.Instance.checkClear())
            {
                foreach (CardUI card in CardManager.Instance.HandCardList)
                {
                    card.isHand = false;
                }
                
                PlayerManager.Instance.CardUsable = false;

                gameClearPanelActivation();
            }
        }
        else {
            if (EnemyManager.Instance.EnemyList.Count == 0)
            {
                foreach (CardUI card in CardManager.Instance.HandCardList)
                {
                    card.isHand = false;
                }
                
                PlayerManager.Instance.CardUsable = false;

                // DialogueManager.Instance.dialogueCallBack.DialogueCallBack -= PlayerManager.Instance.Init;
                // DialogueManager.Instance.dialogueCallBack.DialogueCallBack += gameClearPanelActivation;
                
                // if(PlayerManager.Instance.TutorialTrigger)
                // {
                //     DialogueManager.Instance.dialogueCallBack.DialogueCallBack -= PlayerManager.Instance.NextTutorialNum;
                //     TutorialManager.Instance.toNextTutorial(PlayerManager.Instance.TutorialPhase);
                // }
                // else
                // {
                //     DialogueManager.Instance.StartDialogue(string.Format("Enemy{0}_victory", _currentStage % 100));
                // }
                // return;

                gameClearPanelActivation();
            }
        }
    }

    private void gameOverPanelActivation()
    {
        PanelManager.Instance.GameOverPanel.SetActive(true);
    }

    private void gameClearPanelActivation()
    {
        PanelManager.Instance.GameClearPanel.SetActive(true);
        PanelManager.Instance.GameClearPanel.transform.DOLocalMoveY(100, 1.0f);
    }

    private void SetDataTable()
    {
        StageTable = new StageDataTable("StageTable");  // table 이름 초기화, 중요하진 않음
        StageTable.LoadCsv("stage.csv");
    }
}
