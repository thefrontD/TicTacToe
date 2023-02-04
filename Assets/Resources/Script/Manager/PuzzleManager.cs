using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : Singleton<PuzzleManager>
{
    [SerializeField] List<GameObject> TargetBoardUIPrefab;
    [SerializeField] GameObject _targetBoard;
    [SerializeField] Sprite DefaultSprite;
    [SerializeField] Sprite EnemySprite;
    [SerializeField] Sprite PlayerSprite;
    [SerializeField] Sprite BlackSprite;

    private List<List<BoardColor>> _targetColors = new List<List<BoardColor>>();
    private List<List<SpriteRenderer>> _targetUI = new List<List<SpriteRenderer>>();
    private int _bingoCount;
    private GameObject _targetBoardUI;
    

    void Start() {
        
    }

    void Update() {
        
    }

    public void LoadingTarget(string stageID) {
        TargetBoardDataHolder holder = TargetBoardData.Instance._load(stageID);
        
        // _boardSize = holder._boardSize;
        // _boardColors = holder._boardColors;

        // MainBoard = Instantiate(BoardPrefabs[_boardSize - 3], BoardPos, Utils.QS);
        
        // for (int i = 0; i < _boardSize; i++)
        // {
        //     _gameBoard.Add(new List<Board>());
        //     _boardAttackables.Add(new List<IAttackable>());
        //     for (int j = 0; j < _boardSize; j++)
        //     {
        //         _gameBoard[i].Add(MainBoard.transform.GetChild(i*_boardSize+j).GetComponent<Board>());
        //         _gameBoard[i][j].Init(_boardColors[i][j], i, j);
        //         _boardAttackables[i].Add(null);
        //     }
        // }

        TargetUIInit();
    }

    private void TargetUIInit() {
        _targetBoard.SetActive(true);
        _targetBoardUI = Instantiate(TargetBoardUIPrefab[BoardManager.Instance.BoardSize-3], _targetBoard.transform);

        for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
        {
            _targetUI.Add(new List<SpriteRenderer>());
            for (int j = 0; j < BoardManager.Instance.BoardSize; j++) 
            {
                _targetUI[i].Add(_targetBoardUI.transform.GetChild(i*BoardManager.Instance.BoardSize+j).GetComponent<SpriteRenderer>());
                switch(_targetColors[i][j]) {
                    case BoardColor.None:
                        _targetUI[i][j].sprite = DefaultSprite;
                    break;
                    case BoardColor.Player:
                        _targetUI[i][j].sprite = PlayerSprite;
                    break;
                    case BoardColor.Enemy:
                        _targetUI[i][j].sprite = EnemySprite;
                    break;
                    case BoardColor.Black:
                        _targetUI[i][j].sprite = BlackSprite;
                    break;
                }
            }
        }
    }
    
    public bool checkClear() {
        bool ret = true;

        if(_bingoCount > 0) {
            if(BoardManager.Instance.CheckBingo(BoardColor.Player) != _bingoCount) {
                ret = false;
            }
        }
        else {
            for (int i = 0; i < BoardManager.Instance.BoardSize; i++) {
                for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                {
                    if(BoardManager.Instance.BoardColors[i][j] != _targetColors[i][j]) {
                        ret = false;
                        break; 
                    }
                }
            }
        }

        return ret;
    }
}