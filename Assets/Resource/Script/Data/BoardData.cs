using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Holder
{
    public int _boardSize, _playerRow, _playerCol;
    public List<List<BoardObject>> _boardObjects = new List<List<BoardObject>>();
    public List<List<BoardColor>> _boardColors = new List<List<BoardColor>>();

    public Holder(int boardSize, int playerRow, int playerCol,
        List<List<BoardObject>> boardObjects, List<List<BoardColor>> boardColors)
    {
        _boardSize = boardSize;
        _playerRow = playerRow;
        _playerCol = playerCol;
        _boardObjects = boardObjects;
        _boardColors = boardColors;
    }
}

public class BoardData : Singleton<BoardData>
{
    /*public bool saveData(Holder holder, string dataName)
    {
        var converter = new StringEnumConverter();
        var pDataStringSave = JsonConvert.SerializeObject(holder, converter);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, dataName), pDataStringSave);
        return true;
    }*/
    
    public Holder _load(string dataName)
    {
        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, dataName));
        Holder EnemyActions = JsonConvert.DeserializeObject<Holder>(pDataStringLoad, converter);

        return EnemyActions;
    }
}