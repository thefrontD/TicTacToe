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
    private GameObject[,] GameBoard = new GameObject[3, 3];
    private BoardStates[,] boardStates = new BoardStates[3, 3];
    //Board Color Array
    private BoardColor[,] boardColors = new BoardColor[3, 3];
    //Actual Board Components in Game
    private int BoardSize = 3;
    
    void Start()
    {
        BoardLoading();
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                Vector3 pos = new Vector3(-2 + 2 * i, 2 - 2 * j, 0);
                GameBoard[i, j] = Instantiate(BoardPrefab, pos, Quaternion.identity);
            }
        }
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Board 기획이 나오면 Json 파일로 저장해서 로딩해서 사용할 예정
    /// </summary>
    private void BoardLoading()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    public bool ColoringBoard(int x, int y, BoardColor boardColor)
    {
        if (x >= BoardSize - 1 || y >= BoardSize - 1 || x < 0 || y < 0)
            return false;
        else
        {
            boardColors[x, y] = boardColor;
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
        
        if (x >= BoardSize - 1 || y >= BoardSize - 1 || x < 0 || y < 0)
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
}
