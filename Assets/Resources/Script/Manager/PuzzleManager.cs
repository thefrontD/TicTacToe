using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : Singleton<PuzzleManager>
{
    private List<List<BoardColor>> _targetColors = new List<List<BoardColor>>();
    private int _bingoCount;

    void Start() {
        
    }

    void Update() {
        
    }

    public void LoadingTarget(string stageID) {
        Holder holder = BoardData.Instance._load(stageID);
        
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