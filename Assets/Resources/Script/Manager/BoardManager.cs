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
    [SerializeField] private List<GameObject> BoardPrefabs;
    //Board State Array
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject WallPrefabs;
    [SerializeField] private GameObject ColorEffect;
    [SerializeField] private GameObject MoveWindEffect;
    [SerializeField] private Vector3 bias;
    [SerializeField] private Vector3 BoardPos;

    private GameObject MainBoard;
    
    private GameObject _playerObject;
    public GameObject PlayerObject => _playerObject;

    private List<List<Board>> _gameBoard = new List<List<Board>>();
    public List<List<Board>> GameBoard => _gameBoard;

    [SerializeField] private List<List<BoardObject>> _boardObjects = new List<List<BoardObject>>();
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
    public void BoardLoading(string stageID)
    {
        BoardDataHolder boardDataBoardDataHolder = BoardData.Instance._load(stageID);
        
        _boardSize = boardDataBoardDataHolder._boardSize;
        PlayerManager.Instance.Row = boardDataBoardDataHolder._playerRow;
        PlayerManager.Instance.Col = boardDataBoardDataHolder._playerCol;
        _boardObjects = boardDataBoardDataHolder._boardObjects;
        _boardColors = boardDataBoardDataHolder._boardColors;

        MainBoard = Instantiate(BoardPrefabs[_boardSize - 3], BoardPos, Utils.QS);
        
        for (int i = 0; i < _boardSize; i++)
        {
            _gameBoard.Add(new List<Board>());
            _boardAttackables.Add(new List<IAttackable>());
            for (int j = 0; j < _boardSize; j++)
            {
                //_gameBoard[i].Add(Instantiate(BoardPrefab, pos, Quaternion.identity).GetComponent<Board>());
                _gameBoard[i].Add(MainBoard.transform.GetChild(i*_boardSize+j).GetComponent<Board>());
                _gameBoard[i][j].Init(_boardColors[i][j], i, j);
                _boardAttackables[i].Add(null);

                if(_boardObjects[i][j] == BoardObject.Wall) SummonWalls(i, j); 
            }
        }

        InitPlayer();
    }

    private void InitPlayer()
    {
        Vector3 initPos = _gameBoard[PlayerManager.Instance.Row][PlayerManager.Instance.Col].transform.position  + bias;
        _boardObjects[PlayerManager.Instance.Row][PlayerManager.Instance.Col] = BoardObject.Player;
        _playerObject = Instantiate(PlayerPrefab, initPos, Utils.QI);
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
            if(boardColor == BoardColor.Player){
                _gameBoard[row][col].ActivateColorEffect();
                SoundManager.Instance.PlaySE("Coloring");
            }
            _boardColors[row][col] = boardColor;
            _gameBoard[row][col].SetBoardColor(boardColor);
            ResetBingoEffect();
            CheckBingoAndEffect(BoardColor.Player);
            CheckBingoAndEffect(BoardColor.Enemy);
            return true;
        }
    }

    // public void PlayColorEffect(int row, int col){
    //     Debug.Log(_gameBoard[row][col].GetComponent<Transform>().position);
    //     Vector3 pos = new Vector3(0,0,0);
    //     pos = _gameBoard[row][col].GetComponent<Transform>().position;
    //     pos.z = -2;
    //     GameObject EffectInstance = Instantiate(ColorEffect, pos, Quaternion.identity);
    //     Destroy(EffectInstance, 2);
    //     return;
    // }
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
            GameObject wall = Instantiate(WallPrefabs, _gameBoard[row][col].transform.position, Quaternion.Euler(-90, 0, 0));
            _boardAttackables[row][col] = wall.GetComponent<Wall>();
            wall.GetComponent<Wall>().Init(row, col);
            wall.transform.DOMoveZ(-4, 1).SetEase(Ease.OutQuart);
            wall.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        }
        else if(_boardObjects[row][col] == BoardObject.Player)
            _isGameOver = PlayerManager.Instance.DamageToPlayer(-damage);
        
        return _isGameOver;
    }

    public void SummonWalls(int row, int col)
    {
        
        _boardObjects[row][col] = BoardObject.Wall;
        ColoringBoard(row, col, BoardColor.None);
        GameObject wall = Instantiate(WallPrefabs, _gameBoard[row][col].transform.position, Quaternion.Euler(-90, 0, 0));
        wall.GetComponent<Wall>().Init(row, col);
        wall.transform.DOMoveZ(-4, 1).SetEase(Ease.OutQuart);
        wall.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
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
            Vector3 nextPos = _gameBoard[PlayerManager.Instance.Row][PlayerManager.Instance.Col].transform.position + bias;
            _boardObjects[row][col] = BoardObject.Player;
            Vector3 nextRot = PlayerObject.transform.position - nextPos;

            PlayMoveEffect(PlayerObject.transform.position, nextPos);
            SoundManager.Instance.PlaySE("MovePlayer");
            
            switch (effect)
            {
                case MoveCardEffect.Slide:
                    //PlayerObject.transform.DORotate(new Vector3(angle, 90, -90), 0.5f);
                    PlayerObject.transform.DOMove(nextPos, 0.5f, false);
                    return true;
                default:  // 띄우고, 옮기고, 내림
                    Vector3 currPos = PlayerObject.transform.position;
                    //PlayerObject.transform.DORotate(new Vector3(angle, 90, -90), 0.5f);
                    Sequence moveSequence = DOTween.Sequence()
                        .Append(PlayerObject.transform.DOMove(currPos + new Vector3(0, 0, -9), 0.3f, false))
                        .Append(PlayerObject.transform.DOMove(nextPos + new Vector3(0, 0, -9), 0.5f, false))
                        .Append(PlayerObject.transform.DOMove(nextPos, 0.3f, false));
                    return true;
            }
        }
    }

    public void PlayMoveEffect(Vector3 from, Vector3 to)
    {
        // Vector3 direction = to - from;
        // float scale = direction.magnitude / 7f;
        // print("Scale: " + scale);
        // Quaternion angle = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        // GameObject moveWindEffectObject = Instantiate(MoveWindEffect, from + new Vector3(0, 0, -5), angle);  // 자동으로 destroy된다.
        // moveWindEffectObject.transform.localScale = new Vector3(scale, 3f, 3f);
    }

    /// <summary>
    /// 보드판 전체에서 color로 색칠된 빙고의 개수를 리턴하고, 빙고 색칠을 지운다.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public int CheckBingo(BoardColor color)
    {
        bool[] horizontalBingo = new bool[_boardSize];
        bool[] verticalBingo = new bool[_boardSize];
        bool[] diagonalbingo = new bool[2];
        int ret = 0;

        //빙고 개수 세기, 빙고 위치 저장
        for (int i = 0; i < _boardSize; i++)
        {
            horizontalBingo[i] = verticalBingo[i] = true;            
            for (int j = 0; j < _boardSize; j++)
            {
                if (_boardColors[i][j] != color)
                    horizontalBingo[i] = false;

                if (_boardColors[j][i] != color)
                    verticalBingo[i] = false;
            } 
            if (horizontalBingo[i])
                ret ++;
            if (verticalBingo[i])
                ret++;
        }

        diagonalbingo[0] = diagonalbingo[1] = true;
        for (int i = 0; i < _boardSize; i++)
        {
            if (_boardColors[i][i] != color)
                diagonalbingo[0] = false;
            if (_boardColors[i][_boardSize - 1 - i] != color)
                diagonalbingo[1] = false;
        }
        if (diagonalbingo[0])
            ret++;
        if (diagonalbingo[1])
            ret++;

        //보드판 색칠 지우기
        for (int i = 0; i < _boardSize; i++)
        {
            if(horizontalBingo[i])
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    _boardColors[i][j] = BoardColor.None;
                }
            }
            if(verticalBingo[i])
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    _boardColors[j][i] = BoardColor.None;
                }
            }
            if(diagonalbingo[0])
            {
                _boardColors[i][i] = BoardColor.None;
            }
            if(diagonalbingo[1])
            {
                _boardColors[i][_boardSize - 1 - i] = BoardColor.None;
            }
        }

        
        return ret;
    }

    /// <summary>
    /// 보드판 전체에서 color로 색칠된 빙고의 개수를 리턴한다.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public int CountBingo(BoardColor color)
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

        return ret;
    }

    public void SetPlayerDebuffEffect(Debuff debuff){
        if(debuff == Debuff.PowerIncrease || debuff == Debuff.DamageIncrease){
            PlayerObject.transform.Find("BuffEffect").gameObject.SetActive(true);
            SoundManager.Instance.PlaySE("Buff", 0.5f);
            Debug.Log("BuffSE should be played");
        }else if(debuff == Debuff.Heal){
            PlayerObject.transform.Find("HealEffect").gameObject.SetActive(true);
            SoundManager.Instance.PlaySE("HealHP", 0.5f);
            Debug.Log("HealHP should be played");
        }else{
            PlayerObject.transform.Find("DebuffEffect").gameObject.SetActive(true);
            SoundManager.Instance.PlaySE("Debuff", 1.0f);
            Debug.Log("DebuffSE should be played");
        }
    }
}
