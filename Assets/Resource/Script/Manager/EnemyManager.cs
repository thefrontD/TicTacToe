using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    private List<Enemy> _enemyList;

    public List<Enemy> EnemyList => _enemyList;

    [SerializeField] private StringGameObjectDictionary EnemyPrefab;

    void Awake()
    {
        _enemyList = new List<Enemy>();
        EnemyLoading("EnemyData.json");
    }

    void  Start()
    {
        for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
        {
            for (int j = 0; j < BoardManager.Instance.BoardSize; j++)
            {
                BoardManager.Instance.GameBoard[i][j].SetHighlight(BoardSituation.None);
            }
        }
        HightLightBoard();
    }

    void Update()
    {
        
    }

    public void HightLightBoard()
    {
        foreach(Enemy enemy in _enemyList){
            switch (enemy.EnemyActions.Peek().Item1)
            {
                case EnemyAction.H1Attack:
                    for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                        BoardManager.Instance.GameBoard[i][PlayerManager.Instance.Col].SetHighlight(BoardSituation.WillAttack);
                    break;
                case EnemyAction.V1Attack:
                    for (int i = 0; i < BoardManager.Instance.BoardSize; i++)
                        BoardManager.Instance.GameBoard[PlayerManager.Instance.Row][i].SetHighlight(BoardSituation.WillAttack);
                    break;
                case EnemyAction.WallSummon:
                case EnemyAction.WallsSummon:
                    enemy.GetOverLapPosition(enemy.EnemyActions.Peek());
                    break;
            }
        }
    }

    /// <summary>
    /// Enemy 생성 함수
    /// Data로 부터 이름을 받아와서 Dictionary에서 서칭한 후 Instantiate
    /// </summary>
    /// <param name="EnemyNameList"></param>
    private void EnemyLoading(string enemyDataName)
    {
        List<EnemyDataHolder> enemyDataHolders = EnemyData.Instance._load(enemyDataName);

        foreach (EnemyDataHolder enemyData in enemyDataHolders)
        {
            GameObject enemyObject = Instantiate(EnemyPrefab[enemyData.EnemyName], new Vector3(0, 20, 0), Quaternion.Euler(30, 0, 0));
            Enemy enemy = enemyObject.GetComponent<Enemy>();
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
        yield return new WaitForSeconds(1.0f);
     
        PlayerManager.Instance.StatesQueue.Enqueue(new NormalState(5, true));
        
        foreach (Enemy enemy in _enemyList)
        {
            enemy.DoEnemyAction();
            yield return new WaitForSeconds(0.5f);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        PlayerManager.Instance.ChangeStates(new NormalState(5, true));
    }
}
