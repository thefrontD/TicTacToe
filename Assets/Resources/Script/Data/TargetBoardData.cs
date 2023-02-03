using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class TargetBoardDataHolder
{
    public int _targetBingo;
    public List<List<BoardColor>> _targetColors = new List<List<BoardColor>>();

    public TargetBoardDataHolder(int targetBingo, List<List<BoardColor>> targetColors)
    {
        _targetBingo = targetBingo;
        _targetColors = targetColors;
    }
}

public class TargetBoardData : Singleton<BoardData>
{
    public TargetBoardDataHolder _load(string dataName)
    {
        string path = Application.streamingAssetsPath;
        path += $"/Data/TargetBoard/{dataName}.json";

        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(path);
        TargetBoardDataHolder boardDataHolder = JsonConvert.DeserializeObject<TargetBoardDataHolder>(pDataStringLoad, converter);

        return boardDataHolder;
    }
}