using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class CardData : MonoBehaviour
{

    public bool saveData(List<Card> cardData, string DataName)
    {
        var converter = new StringEnumConverter();
        var pDataStringSave = JsonConvert.SerializeObject(cardData, converter);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, DataName), pDataStringSave);
        return true;
    }

    public List<Card> _load(string DataName)
    {
        List<Card> cardData;
        
        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, DataName));
        cardData = JsonConvert.DeserializeObject<List<Card>>(pDataStringLoad, converter);

        return cardData;
    }

    public bool _loadnew(string DataName)
    {
        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "OriginCardData.json"));
        List<Card> cardData = JsonConvert.DeserializeObject<List<Card>>(pDataStringLoad, converter);
        saveData(cardData, DataName);
        return true;
    }
}