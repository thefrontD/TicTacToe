using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    private List<GameObject> EnemyObjectsList;
    private List<Enemy> EnemyList;

    [SerializeField] private StringGameObjectDictionary EnemyPrefab;

    void Start()
    {
        EnemyObjectsList = new List<GameObject>();
        EnemyList = new List<Enemy>();
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
    private void EnemyLoading(List<string> EnemyNameList)
    {
        foreach (string name in EnemyNameList)
        {
            //EnemyObjectsList.Add(Instantiate(EnemyPrefab[name], Vector3));
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
        
        foreach (Enemy enemy in EnemyList)
        {
            enemy.EnemyAction();
            yield return new WaitForSeconds(0.5f);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        PlayerManager.Instance.ChangeStates(PlayerManager.Instance.StatesQueue.Dequeue());
    }

    public List<Enemy> GetEnemyList()
    {
        return EnemyList;
    }
}
