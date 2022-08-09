using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Board의 상태를 조절하는 부분으로 Board와 관련된 부분은 이 Manager에 작성 바람
/// </summary>
public class BoardManager : Singleton<BoardManager>
{
    /// <summary>
    /// BoardState는 장애물 유무를 비롯한 칸의 상태
    /// BoardColor는 현재 칸의 소유주의 상태
    /// 일단은 간편하게 3 by 3 배열 사용중이지만 범용을 위해 List<List<>>로 전환예정
    /// -> Board 기획이 나와서 Json 파일이 나오면 전환 예정
    /// </summary>
    [SerializeField] private GameObject BoardPrefab;
    //Board State Array
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject WallPrefabs;
    private GameObject PlayerObject;

    private List<List<Board>> _gameBoard = new List<List<Board>>();
    public List<List<Board>> GameBoard => _gameBoard;

    private List<List<BoardObject>> _boardObjects = new List<List<BoardObject>>();
    public List<List<BoardObject>> BoardObjects => _boardObjects;
    //Board Color Array
    private List<List<BoardColor>> _boardColors = new List<List<BoardColor>>();
    public List<List<BoardColor>> BoardColors => _boardColors;
    //Actual Board Components in Game
    private int _boardSize = 3;
    public int BoardSize => _boardSize;

    private List<List<IAttackable>> _boardAttackables = new List<List<IAttackable>>();
    public List<List<IAttackable>> BoardAttackables => _boardAttackables;


    void Awake()
    {
        
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Board 기획이 나오면 Json 파일로 저장해서 로딩해서 사용할 예정
    /// </summary>
    public void BoardLoading()
    {
        Holder holder = BoardData.Instance._load("BoardData.json");
        
        _boardSize = holder._boardSize;
        PlayerManager.Instance.Row = holder._playerRow;
        PlayerManager.Instance.Col = holder._playerCol;
        _boardObjects = holder._boardObjects;
        _boardColors = holder._boardColors;
        
        for (int i = 0; i < _boardSize; i++)
        {
            _gameBoard.Add(new List<Board>());
            _boardAttackables.Add(new List<IAttackable>());
            for (int j = 0; j < _boardSize; j++)
            {
                float size = BoardPrefab.transform.localScale.x;
                Vector3 pos = new Vector3(-size + size * j, size - size * i, 0);
                _gameBoard[i].Add(Instantiate(BoardPrefab, pos, Quaternion.identity).GetComponent<Board>());
                _gameBoard[i][j].Init(_boardColors[i][j], i, j);
                _boardAttackables[i].Add(null);
            }
        }

        InitPlayer();
    }

    private void InitPlayer()
    {
        Vector3 initPos = _gameBoard[PlayerManager.Instance.Row][PlayerManager.Instance.Col].transform.position 
                          - new Vector3(0, 0, PlayerPrefab.transform.localScale.z/2);
        _boardObjects[PlayerManager.Instance.Row][PlayerManager.Instance.Col] = BoardObject.Player;
        PlayerObject = Instantiate(PlayerPrefab, initPos, Quaternion.identity);
    }

    /// <summary>
    /// 
    /// </summary>

    public bool ColoringBoard(int x, int y, BoardColor boardColor)
    {
        if (x >= _boardSize || y >= _boardSize || x < 0 || y < 0)
            return false;
        else
        {
            _boardColors[x][y] = boardColor;
            _gameBoard[x][y].SetBoardColor(boardColor);

            return true;
        }
    }

    public bool SummonWalls(int x, int y, int damage)
    {
        bool _isGameOver = false;
        
        if(_boardObjects[x][y] == BoardObject.None)
            _boardObjects[x][y] = BoardObject.Wall;
        else if(_boardObjects[x][y] == BoardObject.Player)
            _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
        
        return _isGameOver;
    }

    public bool MovePlayer(int row, int col)
    {
        if (row >= _boardSize || col >= _boardSize || row < 0 || col < 0)
            return false;
        else
        {
            PlayerManager.Instance.Row = row;
            PlayerManager.Instance.Col = col;
            Vector3 nextPos = _gameBoard[PlayerManager.Instance.Row][PlayerManager.Instance.Col].transform.position - 
                              new Vector3(0, 0, PlayerPrefab.transform.localScale.z/2);
            PlayerObject.transform.DOMove(nextPos, 0.5f, false);
            return true;
        }
    }

    public int CheckBingo(BoardColor color)
    {
        int ret = 0;
        bool check1, check2;
        
        for (int i = 0; i < _boardSize; i++)
        {
            check1 = check2 = true;            
            
            for (int j = 0; j < _boardSize; j++)
            {
                if (_boardColors[i][j] != color)
                    check1 = false;

                if (_boardColors[j][i] != color)
                    check2 = false;
            } 

            if (check1)
                ret++;
            if (check2)
                ret++;
        }

        check1 = check2 = true;
        for (int i = 0; i < _boardSize; i++)
        {
            if (_boardColors[i][i] != color)
                check1 = false;
            if (_boardColors[i][_boardSize - 1 - i] != color)
                check2 = false;
        }
        
        if (check1)
            ret++;
        if (check2)
            ret++;

        return ret;
    }
    
    /// <summary>
    /// 빙고 체킹
    /// </summary>
    public int CheckBingo(int x, int y, BoardColor color)
    {
        int ret = 0;
        bool check = true;
        
        if (x >= _boardSize || y >= _boardSize || x < 0 || y < 0)
            return 0;
        else if (x == y)
        { 
            for (int i = 0; i < _boardSize; i++)
                if (_boardColors[i][i] != color)
                    check = false;
            if (check) ret++;
        }

        check = true;
        for (int i = 0; i < _boardSize; i++)
            if (_boardColors[x][i] != color)
                check = false;
        if (check) ret++;
        
        check = true;
        for (int i = 0; i < _boardSize; i++)
            if (_boardColors[i][y] != color)
                check = false;
        if (check) ret++;

        Debug.Log(string.Format("빙고 개수 : {0}", ret));
        
        return ret;
    } 
}
