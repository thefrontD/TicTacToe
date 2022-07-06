using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class HolderHolder
{
    public List<Card> Objects { get; set; }
}

public class CardData : Singleton<CardData>
{
    public bool saveData(List<Card> cardData, string dataName)
    {
        var converter = new StringEnumConverter();
        var pDataStringSave = JsonConvert.SerializeObject(cardData, converter);
        Debug.Log(pDataStringSave);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, dataName), pDataStringSave);
        return true;
    }

    public List<Card> _load(string dataName)
    {
        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, dataName));
        List<Card> cardData = JsonConvert.DeserializeObject<List<Card>>(pDataStringLoad, converter);

        return cardData;
    }

    public bool _loadnew(string dataName)
    {
        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "OriginCardData.json"));
        List<Card> cardData = JsonConvert.DeserializeObject<List<Card>>(pDataStringLoad, converter);
        saveData(cardData, dataName);
        return true;
    }
}