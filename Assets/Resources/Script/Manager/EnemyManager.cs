using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    private List<Enemy> _enemyList;

    public List<Enemy> EnemyList => _enemyList;

    [SerializeField] private StringGameObjectDictionary EnemyPrefab;
    [SerializeField] public GameObject EnemyAttackEffect;

    void Awake()
    {
        _enemyList = new List<Enemy>();
    }

    void Start()
    {
        
    }


    public void EnemyLoading(string dataName)
    {
        EnemyDataLoading(dataName);
        
        for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
        {
            for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
            {
                BoardManager.Instance.GameBoard[i][j].SetHighlight(BoardSituation.None);
            }
        }

        foreach (Enemy enemy in EnemyList)
            enemy.setPreviousPos(PlayerManager.Instance.Row, PlayerManager.Instance.Col);
        
        HightLightBoard();
    }
    
    public void HightLightBoard()
    {
        foreach(Enemy enemy in _enemyList){
            switch (enemy.EnemyActions.Peek().Item1)
            {
                case EnemyAction.H1Attack:
                    for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                        BoardManager.Instance.GameBoard[enemy.PreviousPlayerRow][i].SetHighlight(BoardSituation.WillAttack);
                    break;
                case EnemyAction.V1Attack:
                    for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                        BoardManager.Instance.GameBoard[i][enemy.PreviousPlayerCol].SetHighlight(BoardSituation.WillAttack);
                    break;
                case EnemyAction.H2Attack:
                    for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                    {
                        if (i == enemy.PreviousPlayerRow) continue;
                        else
                        {
                            for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                                BoardManager.Instance.GameBoard[i][j].SetHighlight(BoardSituation.WillAttack);
                        }
                    }
                    break;
                case EnemyAction.V2Attack:
                    for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                    {
                        if (i == enemy.PreviousPlayerCol) continue;
                        else
                        {
                            for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                            {
                                Debug.Log(string.Format("{0} : {1}", i, j));
                                BoardManager.Instance.GameBoard[j][i].SetHighlight(BoardSituation.WillAttack);
                            }
                        }
                    }
                    break;
                case EnemyAction.ColoredAttack:
                    for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                    {
                        for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                        {
                            if (BoardManager.Instance.GameBoard[i][j].currentBoardColor == BoardColor.Player)
                            {
                                Debug.Log(string.Format("{0} : {1}", i, j));
                                BoardManager.Instance.GameBoard[i][j].SetHighlight(BoardSituation.WillAttack);
                            }
                        }
                    }
                    break;
                case EnemyAction.NoColoredAttack:
                    for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                    {
                        for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                        {
                            if(BoardManager.Instance.GameBoard[i][j].currentBoardColor != BoardColor.Enemy)
                                BoardManager.Instance.GameBoard[i][j].SetHighlight(BoardSituation.WillAttack);
                        }
                    }
                    break;
                case EnemyAction.AllAttack:
                    for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                    {
                        for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
                        {
                            BoardManager.Instance.GameBoard[i][j].SetHighlight(BoardSituation.WillAttack);
                        }
                    }
                    break;
                case EnemyAction.WallSummon:
                case EnemyAction.WallsSummon:
                    enemy.HighlightOverlapPoint();
                    break;
            }
        }
    }

    /// <summary>
    /// Enemy 생성 함수
    /// Data로 부터 이름을 받아와서 Dictionary에서 서칭한 후 Instantiate
    /// </summary>
    /// <param name="EnemyNameList"></param>
    private void EnemyDataLoading(string enemyDataName)
    {
        List<EnemyDataHolder> enemyDataHolders = EnemyData.Instance._load(enemyDataName);

        foreach (EnemyDataHolder enemyData in enemyDataHolders)
        {
            GameObject enemyObject = Instantiate(EnemyPrefab[enemyData.EnemyName], new Vector3(0, 17.5f, 0), Quaternion.Euler(-90, 0, 0));
            Enemy enemy = enemyObject.transform.GetChild(0).GetComponent<Enemy>();
            enemy.InitEnemyData(enemyData);
            EnemyList.Add(enemy);
        }
    }

    public void EnemyAttack()
    {
        StartCoroutine(EnemyAttackCoroutine());
    }

    private IEnumerator EnemyAttackCoroutine()
    {
        bool _isGameOver = false;
        
        yield return new WaitForSeconds(1.0f);

        foreach (Enemy enemy in _enemyList)
        {
            _isGameOver = enemy.DoEnemyAction();
            
            Debug.Log(_isGameOver);
            
            if (_isGameOver) break;
            yield return new WaitForSeconds(0.5f);
        }
        
        yield return new WaitForSeconds(0.5f);
        if (_isGameOver)
            PlayerManager.Instance.ChangeStates(new NormalState(6, false));
        else
            PlayerManager.Instance.ChangeStates(new NormalState(6, true));
    }
}
