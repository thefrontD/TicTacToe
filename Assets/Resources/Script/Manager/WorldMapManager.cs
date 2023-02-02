using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.UI;
using TMPro;

public class WorldMapManager : Singleton<WorldMapManager>
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private List<GameObject> _worldMapBoardPrefab;
    [SerializeField] private Vector3 bias;
    [SerializeField] private float animationDuration;
    private GameObject _player;
    private List<List<WorldMapBoard>> _boards = new List<List<WorldMapBoard>>();
    private int _boardSize = 3;
    private bool isMoving = false;
    
    public void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        GameObject worldMapBoard = Instantiate(_worldMapBoardPrefab[GameManager.Instance.CurrentLevel], Vector3.forward * 2, Utils.QI);
        _boardSize = GameManager.Instance.CurrentLevel + 3;

        for (int i = 0; i < _boardSize; i++)
        {
            _boards.Add(new List<WorldMapBoard>());

            for (int j = 0; j < _boardSize; j++)
            {
                _boards[i].Add(worldMapBoard.transform.GetChild(i*_boardSize+j).GetComponent<WorldMapBoard>());
                _boards[i][j].init(j, i);
            }
        }

        Vector3 initPos = _boards[GameManager.Instance.CurrentRow][GameManager.Instance.CurrentCol].transform.position + bias; 
        _player = Instantiate(_playerPrefab, initPos, Utils.QI);
    }


    void Update()
    {
        
    }

    public void MovePlayer(int col, int row) {
        int dCol = Math.Abs(GameManager.Instance.CurrentCol - col); 
        int dRow = Math.Abs(GameManager.Instance.CurrentRow - row); 

        if((dCol + dRow == 1) && !isMoving){
            GameManager.Instance.CurrentRow = row;
            GameManager.Instance.CurrentCol = col;
            isMoving = true;

            Vector3 movePos = _boards[GameManager.Instance.CurrentRow][GameManager.Instance.CurrentCol].transform.position  + bias;    

            _player.transform.DOMove(movePos, animationDuration, false).OnComplete(
                () => { StartCoroutine(toBattleScene()); }
            );
        }

        return;
    }

    private IEnumerator toBattleScene() {
        yield return new WaitForSeconds(0.5f);

        isMoving = false;
        //LoadingManager.Instance.LoadBattleScene();
    }
}
