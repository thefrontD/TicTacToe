using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class EnemyData : Singleton<EnemyData>
{
    public Queue<EnemyAction> _load(string dataName)
    {
        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, dataName));
        Queue<EnemyAction> EnemyActions = JsonConvert.DeserializeObject<Queue<EnemyAction>>(pDataStringLoad, converter);

        return EnemyActions;
    }
}