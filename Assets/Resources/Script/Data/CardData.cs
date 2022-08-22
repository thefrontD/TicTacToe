using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class CardData : Singleton<CardData>
{
    public bool saveData(List<Card> cardData, string dataName)
    {
        string path = Application.dataPath;
        path += $"/Data/Card/{dataName}.json";

        var converter = new StringEnumConverter();
        var pDataStringSave = JsonConvert.SerializeObject(cardData, converter);

        Debug.Log(pDataStringSave);

        File.WriteAllText(path, pDataStringSave);
        return true;
    }

    public List<Card> _load(string dataName)
    {
        string path = Application.dataPath;
        path += $"/Data/Card/{dataName}.json";
        print(path);
        
        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(path);

        Debug.Log(pDataStringLoad);

        List<Card> cardData = JsonConvert.DeserializeObject<List<Card>>(pDataStringLoad, converter);

        return cardData;
    }

    public async Task _loadnew(string dataName)
    {
        List<Card> cardData = _load("PlayerCard");
        saveData(cardData, dataName);
    }
}