using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
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
}
