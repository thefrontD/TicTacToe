using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    private List<GameObject> _enemyObjectList;
    private List<Enemy> _enemyList;

    public List<GameObject> EnemyObjectList => _enemyObjectList;
    public List<Enemy> EnemyList => _enemyList;

    [SerializeField] private StringGameObjectDictionary EnemyPrefab;

    void Start()
    {
        EnemyLoading("EnemyData.json");
        _enemyObjectList = new List<GameObject>();
        _enemyList = new List<Enemy>();
        //EnemyLoading();
    }

    void Update()
    {
        
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
            _enemyObjectList.Add(Instantiate(EnemyPrefab[enemyData.EnemyName],
                new Vector3(0, 20, 0), Quaternion.Euler(30, 0 ,0)));
            EnemyList.Add(_enemyObjectList[_enemyObjectList.Count-1].GetComponent<Enemy>());
            EnemyList[EnemyList.Count-1].InitEnemyData(enemyData);
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
            enemy.EnemyAction();
            yield return new WaitForSeconds(0.5f);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        PlayerManager.Instance.ChangeStates(new NormalState(5, true));
    }
}
