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
    private GameObject PlayerObject;
    private List<List<GameObject>> gameBoard = new List<List<GameObject>>();

    private List<List<BoardObject>> boardObjects = new List<List<BoardObject>>();
    //Board Color Array
    private List<List<BoardColor>> boardColors = new List<List<BoardColor>>();
    public List<List<BoardColor>> BoardColors { get => boardColors; }
    //Actual Board Components in Game
    private int BoardSize = 3;


    void Start()
    {
        BoardLoading();
        InitPlayer();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Board 기획이 나오면 Json 파일로 저장해서 로딩해서 사용할 예정
    /// </summary>
    private void BoardLoading()
    {
        Holder holder = BoardData.Instance._load("BoardData.json");
        
        BoardSize = holder._boardSize;
        PlayerManager.Instance.Row = holder._playerRow;
        PlayerManager.Instance.Col = holder._playerCol;
        boardObjects = holder._boardObjects;
        boardColors = holder._boardColors;
        
        for (int i = 0; i < BoardSize; i++)
        {
            gameBoard.Add(new List<GameObject>());
            for (int j = 0; j < BoardSize; j++)
            {
                float size = BoardPrefab.transform.localScale.x;
                Vector3 pos = new Vector3(-size + size * i, size - size * j, 0);
                gameBoard[i].Add(Instantiate(BoardPrefab, pos, Quaternion.identity));
                ColoringBoard(i, j, boardColors[i][j]);
            }
        }
    }

    private void InitPlayer()
    {
        Vector3 initPos = gameBoard[PlayerManager.Instance.Row][PlayerManager.Instance.Col].transform.position 
                          - new Vector3(0, 0, PlayerPrefab.transform.localScale.z/2);
        boardObjects[PlayerManager.Instance.Row][PlayerManager.Instance.Col] = BoardObject.Player;
        PlayerObject = Instantiate(PlayerPrefab, initPos, Quaternion.identity);
    }

    /// <summary>
    /// 
    /// </summary>

    public bool ColoringBoard(int x, int y, BoardColor boardColor)
    {
        if (x >= BoardSize || y >= BoardSize || x < 0 || y < 0)
            return false;
        else
        {
            boardColors[x][y] = boardColor;
            gameBoard[x][y].GetComponent<Board>().SetBoardColor(boardColor);
            return true;
        }
    }

    public bool MovePlayer(int x, int y)
    {
        if (x + PlayerManager.Instance.Row >= BoardSize - 1 || y + PlayerManager.Instance.Col >= BoardSize - 1 || x < 0 || y < 0)
            return false;
        else
        {
            PlayerManager.Instance.Row += x;
            PlayerManager.Instance.Col += y;
            Vector3 nextPos = gameBoard[PlayerManager.Instance.Row][PlayerManager.Instance.Col].transform.position - 
                              new Vector3(0, 0, PlayerPrefab.transform.localScale.z/2);
            PlayerObject.transform.DOMove(nextPos, 0.5f, false);
            return true;
        }
    }

    /// <summary>
    /// 빙고 체킹
    /// </summary>
    public int CheckBingo(int x, int y, BoardColor color)
    {
        int ret = 0;
        bool check = true;
        
        if (x >= BoardSize || y >= BoardSize || x < 0 || y < 0)
            return 0;
        else if (x == y)
        { 
            for (int i = 0; i < BoardSize; i++)
                if (boardColors[i][i] != color)
                    check = false;
            if (check) ret++;
        }

        check = true;
        for (int i = 0; i < BoardSize; i++)
            if (boardColors[x][i] != color)
                check = false;
        if (check) ret++;
        
        check = true;
        for (int i = 0; i < BoardSize; i++)
            if (boardColors[i][y] != color)
                check = false;
        if (check) ret++;

        return ret;
    } 

    //특정 타일의 주변 타일을 받아오는 함수
    public List<Vector2Int> GetAdjacentCoord(Vector2Int coord)
    {
        List<Vector2Int> adjacentCoord = new List<Vector2Int>();
        adjacentCoord.Add(Vector2Int.zero); //주변 타일 추가
        return adjacentCoord;
    }
}
