using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class EnemyDataHolder
{
    private string _enemyName;
    private int _enemyHP;
    private int _enemyShield;
    private Queue<EnemyAction> _enemyAction;

    public string EnemyName => _enemyName;
    public int EnemyHP => _enemyHP;
    public int EnemyShield => _enemyShield;
    public Queue<EnemyAction> EnemyAction => _enemyAction;

    public EnemyDataHolder(string enemyName, int enemyHp, int enemyShield, Queue<EnemyAction> enemyAction)
    {
        this._enemyName = enemyName;
        this._enemyHP = enemyHp;
        this._enemyShield = enemyShield;
        this._enemyAction = enemyAction;
    }
}

public class EnemyData : Singleton<EnemyData>
{
    public List<EnemyDataHolder> _load(string dataName)
    {
        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, dataName));
        List<EnemyDataHolder> enemyDataHolder = JsonConvert.DeserializeObject<List<EnemyDataHolder>>
            (pDataStringLoad, converter);

        return enemyDataHolder;
    }
}