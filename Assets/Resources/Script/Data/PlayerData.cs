using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class PlayerDataHolder
{
    private int _currentStage;
    public int CurrentStage => _currentStage;

    private int _maxHp;
    public int MaxHp => _maxHp;
    
    private int _hp;
    public int Hp => _hp;
    
    private int _maxMana;
    public int MaxMana => _maxMana;
    
    private int _mana;
    public int Mana => _mana;

    private int _baseAp;
    public int BaseAp => _baseAp;
    
    private int _col;
    public int Col => _col;
    
    private int _row;
    public int Row => _row;

    private int _level;
    public int Level => _level;


    public PlayerDataHolder(int currentStage, int maxHp, int hp, int maxMana, int mana, int baseAp, int col, int row, int level)
    {
        this._currentStage = currentStage;
        this._maxHp = maxHp;
        this._maxMana = maxMana;
        this._hp = hp;
        this._mana = mana;
        this._baseAp = baseAp;
        this._col = col;
        this._row = row;
        this._level = level;
    }
}

public class PlayerData : Singleton<PlayerData>
{
    public bool saveData(PlayerDataHolder holder, string dataName)
    {
        string path = Application.streamingAssetsPath;
        path += $"/Data/Player/{dataName}.json";
        
        var converter = new StringEnumConverter();
        var pDataStringSave = JsonConvert.SerializeObject(holder, converter);
        File.WriteAllText(path, pDataStringSave);
        return true;
    }
    
    public PlayerDataHolder _load(string dataName)
    {
        string path = Application.streamingAssetsPath;
        path += $"/Data/Player/{dataName}.json";
        
        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(path);
        PlayerDataHolder playerData = JsonConvert.DeserializeObject<PlayerDataHolder>(pDataStringLoad, converter);

        return playerData;
    }
    
    public PlayerDataHolder _loadNew(string dataName)
    {
        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, dataName));
        PlayerDataHolder playerData = JsonConvert.DeserializeObject<PlayerDataHolder>(pDataStringLoad, converter);

        return playerData;
    }
}