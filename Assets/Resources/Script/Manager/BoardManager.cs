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
    [SerializeField] private GameObject ColorEffect;
    [SerializeField] private GameObject MoveWindEffect;

    private GameObject _playerObject;
    public GameObject PlayerObject => _playerObject;

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
    public void BoardLoading(string dataName)
    {
        Holder holder = BoardData.Instance._load(dataName);
        
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
                Vector3 pos = new Vector3(-size + size * j, size - size * i + 4, 0);
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
                          - new Vector3(0, 0, 0);
        _boardObjects[PlayerManager.Instance.Row][PlayerManager.Instance.Col] = BoardObject.Player;
        Quaternion rotation = Quaternion.Euler(-90, -90, 90);
        _playerObject = Instantiate(PlayerPrefab, initPos, rotation);
        //_playerObject.transform.localScale = new Vector3(12, 12, 12);
    }

    /// <summary>
    /// 
    /// </summary>

    public bool ColoringBoard(int row, int col, BoardColor boardColor)
    {
        if (row >= _boardSize || col >= _boardSize || row < 0 || col < 0)
            return false;
        else
        {
            if(boardColor == BoardColor.Player)
                _gameBoard[row][col].ActivateColorEffect();
            
            _boardColors[row][col] = boardColor;
            _gameBoard[row][col].SetBoardColor(boardColor);
            ResetBingoEffect();
            CheckBingoAndEffect(BoardColor.Player);
            CheckBingoAndEffect(BoardColor.Enemy);
            return true;
        }
    }

    public void PlayColorEffect(int row, int col){
        Debug.Log(_gameBoard[row][col].GetComponent<Transform>().position);
        Vector3 pos = new Vector3(0,0,0);
        pos = _gameBoard[row][col].GetComponent<Transform>().position;
        pos.z = -2;
        GameObject EffectInstance = Instantiate(ColorEffect, pos, Quaternion.identity);
        Destroy(EffectInstance, 2);
        return;
    }
    public void BingoEffect(int row, int col){
        return;
    }

    public bool SummonWalls(int row, int col, int damage)
    {
        bool _isGameOver = false;
        
        if(_boardObjects[row][col] == BoardObject.None)
        {
            _boardObjects[row][col] = BoardObject.Wall;
            ColoringBoard(row, col, BoardColor.None);
            GameObject wall = Instantiate(WallPrefabs, _gameBoard[row][col].transform.position + new Vector3(0, 0, 2.5f), Quaternion.identity);
            _boardAttackables[row][col] = wall.GetComponent<Wall>();
            wall.GetComponent<Wall>().Init(row, col);
            wall.transform.DOMoveZ(-1, 1).SetEase(Ease.OutQuart);
            wall.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        }
        else if(_boardObjects[row][col] == BoardObject.Player)
            _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
        
        return _isGameOver;
    }

    public bool MovePlayer(int row, int col, MoveCardEffect effect)
    {
        if (row >= _boardSize || col >= _boardSize || row < 0 || col < 0)
            return false;
        else
        {
            int angle = -1 * (int)Vector2.Angle(new Vector2(0, 1), 
                new Vector2(row - PlayerManager.Instance.Row, col - PlayerManager.Instance.Col));

            _boardObjects[PlayerManager.Instance.Row][PlayerManager.Instance.Col] = BoardObject.None;
            PlayerManager.Instance.Row = row;
            PlayerManager.Instance.Col = col;
            Vector3 nextPos = _gameBoard[PlayerManager.Instance.Row][PlayerManager.Instance.Col].transform.position;
            _boardObjects[row][col] = BoardObject.Player;
            Vector3 nextRot = PlayerObject.transform.position - nextPos;

            PlayMoveEffect(PlayerObject.transform.position, nextPos);
            switch (effect)
            {
                case MoveCardEffect.Slide:
                    //PlayerObject.transform.DORotate(new Vector3(angle, 90, -90), 0.5f);
                    PlayerObject.transform.DOMove(nextPos, 0.5f, false);
                    return true;
                default:  // TODO: 나중에 effect 만들 시간 있으면 추가하기로 하고, 일단은 Slide로 고정.
                    //PlayerObject.transform.DORotate(new Vector3(angle, 90, -90), 0.5f);
                    PlayerObject.transform.DOMove(nextPos, 0.5f, false);
                    return true;
            }
        }
    }

    public void PlayMoveEffect(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        Quaternion angle = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        Instantiate(MoveWindEffect, from + new Vector3(0, 0, -5), angle);  // 자동으로 destroy된다.
    }

    /// <summary>
    /// 보드판 전체에서 color로 색칠된 빙고의 개수를 리턴한다.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
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
    public int CheckBingo(int row, int col, BoardColor color)
    {
        int ret = 0;
        bool check = true;
        
        if (row >= _boardSize || col >= _boardSize || row < 0 || col < 0)
            return 0;
        else if (row == col)
        { 
            for (int i = 0; i < _boardSize; i++)
                if (_boardColors[i][i] != color)
                    check = false;
            if (check) ret++;
        }

        check = true;
        for (int i = 0; i < _boardSize; i++)
            if (_boardColors[row][i] != color)
                check = false;
        if (check) ret++;
        
        check = true;
        for (int i = 0; i < _boardSize; i++)
            if (_boardColors[i][col] != color)
                check = false;
        if (check) ret++;

        Debug.Log(string.Format("빙고 개수 : {0}", ret));
        
        return ret;
    } 
    public void ResetBingoEffect(){
        for(int i = 0; i<_boardSize; i++){
            for(int j = 0; j<_boardSize; j++){
                GameBoard[i][j].GetComponent<Board>().ActivateBingoEffect(false, BoardColor.None);
            }
        }
    }

    public int CheckBingoAndEffect(BoardColor color)
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

            if (check1){
                Debug.Log("check1 true ret++");
                ret++;
                for(int j = 0; j<_boardSize; j++){
                    GameBoard[i][j].GetComponent<Board>().ActivateBingoEffect(true, color);
                }
            }
            if (check2){
                Debug.Log("check2 true ret++");
                ret++;
                for(int j = 0; j<_boardSize; j++){
                    GameBoard[j][i].GetComponent<Board>().ActivateBingoEffect(true, color);
                }
            }
        }

        check1 = check2 = true;
        for (int i = 0; i < _boardSize; i++)
        {
            if (_boardColors[i][i] != color)
                check1 = false;
            if (_boardColors[i][_boardSize - 1 - i] != color)
                check2 = false;
        }
        
        if (check1){
            Debug.Log("check1_2 true ret++");
            ret++;
            for (int i = 0; i < _boardSize; i++)
                GameBoard[i][i].GetComponent<Board>().ActivateBingoEffect(true, color);
        }
        if (check2){
            Debug.Log("check1_2 true ret++");
            ret++;
            for (int i = 0; i < _boardSize; i++)
                GameBoard[i][_boardSize - 1 - i].GetComponent<Board>().ActivateBingoEffect(true, color);
        }
        Debug.Log("Total Bingo is why not changing: "+ret.ToString());

        return ret;
    }
}
