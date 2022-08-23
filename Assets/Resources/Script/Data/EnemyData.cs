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
    private int _enemyPower;
    private Queue<(EnemyAction, int)> _enemyAction;

    public string EnemyName => _enemyName;
    public int EnemyHP => _enemyHP;
    public int EnemyShield => _enemyShield;
    public int EnemyPower => _enemyPower;
    public Queue<(EnemyAction, int)> EnemyAction => _enemyAction;

    public EnemyDataHolder(string enemyName, int enemyHp, int enemyShield, int enemyPower,
    Queue<(EnemyAction, int)> enemyAction)
    {
        this._enemyName = enemyName;
        this._enemyHP = enemyHp;
        this._enemyShield = enemyShield;
        this._enemyPower = enemyPower;
        this._enemyAction = enemyAction;
    }
}

public class EnemyData : Singleton<EnemyData>
{
    public List<EnemyDataHolder> _load(string dataName)
    {
        string path = Application.streamingAssetsPath;
        path += $"/Data/Enemy/{dataName}.json";
        
        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(path);
        List<EnemyDataHolder> enemyDataHolder = JsonConvert.DeserializeObject<List<EnemyDataHolder>>
            (pDataStringLoad, converter);

        return enemyDataHolder;
    }
}