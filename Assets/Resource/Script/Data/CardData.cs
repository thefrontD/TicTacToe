using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

public class cardData {
    public List<Card> CardList = new List<Card>();

    public cardData(int equipCard, List<Card> CardList){
        this.CardList = CardList;
    }
}

public class CardData : MonoBehaviour, IData
{
    private cardData cardData;

    public bool saveData()
    {
        var converter = new StringEnumConverter();
        var pDataStringSave = JsonConvert.SerializeObject(cardData, converter);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "CardData.json"), pDataStringSave);
        return true;
    }

    public bool _load()
    {
        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "CardData.json"));
        cardData = JsonConvert.DeserializeObject<cardData>(pDataStringLoad, converter);

        foreach (var Card in cardData.CardList)
        {
            Debug.Log(Card.CardName);
        }

        return true;
    }

    public bool _loadnew()
    {
        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "CardData.json"));
        cardData = JsonConvert.DeserializeObject<cardData>(pDataStringLoad, converter);
        saveData();
        return true;
    }
}