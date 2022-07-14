using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Board의 상태를 조절하는 부분으로 Board와 관련된 부분은 이 Manager에 작성 바람
/// </summary>
public class BoardManager : Singleton<BoardManager>
{
    /// <summary>
    /// BoardState는 장애물 유무를 비롯한 칸의 상태
    /// BoardColor는 현재 칸의 소유주의 상태
    /// 일단은 간편하게 3 by 3 배열 사용중이지만 범용을 위해 List<List>로 전환예정
    /// -> Board 기획이 나와서 Json 파일이 나오면 전환 예정
    /// </summary>
    [SerializeField] private GameObject BoardPrefab;
    //Board State Array
    [SerializeField] private GameObject PlayerPrefab;
    private GameObject PlayerObject;
    private GameObject[,] gameBoard = new GameObject[3, 3];
    private BoardStates[,] boardStates = new BoardStates[3, 3];
    //Board Color Array
    private BoardColor[,] boardColors = new BoardColor[3, 3];
    //Actual Board Components in Game
    private int BoardSize = 3;
    
    void Start()
    {
        BoardLoading();
        InitPlayer(1, 1);
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Board 기획이 나오면 Json 파일로 저장해서 로딩해서 사용할 예정
    /// </summary>
    private void BoardLoading()
    {
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                float size = BoardPrefab.transform.localScale.x;
                Vector3 pos = new Vector3(-size + size * i, size - size * j, 0);
                gameBoard[i, j] = Instantiate(BoardPrefab, pos, Quaternion.identity);
            }
        }
    }

    private void InitPlayer(int x, int y)
    {
        Vector3 initPos = gameBoard[x, y].transform.position - new Vector3(0, 0, PlayerPrefab.transform.localScale.z/2);
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
            boardColors[x, y] = boardColor;
            gameBoard[x, y].GetComponent<Board>().SetBoardColor(boardColor);
            return true;
        }
    }

    public bool MovePlayer(int x, int y)
    {
        if (x >= BoardSize - 1 || y >= BoardSize - 1 || x < 0 || y < 0)
            return false;
        else
        {
            Vector3 nextPos = gameBoard[x, y].transform.position - new Vector3(0, 0, PlayerPrefab.transform.localScale.z/2);
            StartCoroutine(MovingPlayerCoroutine(nextPos));
            return true;
        }
    }

    private IEnumerator MovingPlayerCoroutine(Vector3 nextPos)
    {
        PlayerObject.transform.position = nextPos;
        return null;
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
                if (boardColors[i, i] != color)
                    check = false;
            if (check) ret++;
        }

        check = true;
        for (int i = 0; i < BoardSize; i++)
            if (boardColors[x, i] != color)
                check = false;
        if (check) ret++;
        
        check = true;
        for (int i = 0; i < BoardSize; i++)
            if (boardColors[i, y] != color)
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
