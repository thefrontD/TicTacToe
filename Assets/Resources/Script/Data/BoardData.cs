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
    public Holder _load(string dataName)
    {
        string path = Application.streamingAssetsPath;
        path += $"/Data/Board/{dataName}.json";

        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(path);
        Holder holder = JsonConvert.DeserializeObject<Holder>(pDataStringLoad, converter);

        return holder;
    }
}