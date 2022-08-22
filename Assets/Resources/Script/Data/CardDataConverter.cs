using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
{
    protected override JsonConverter ResolveContractConverter(Type objectType)
    {
        if (typeof(Card).IsAssignableFrom(objectType) && !objectType.IsAbstract)
            return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
        return base.ResolveContractConverter(objectType);
    }
}

public class BaseConverter : JsonConverter
{
    static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(Card));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);

        string name = jo["cardName"].Value<string>();

        Debug.Log(name);

        string type = jo["CardType"].Value<string>();
        if(type == null) type = jo["cardType"].Value<string>();

        switch (type)
        {
            case "Attack":
                return JsonConvert.DeserializeObject<AttackCard>(jo.ToString(), SpecifiedSubclassConversion);
            case "Move":
                return JsonConvert.DeserializeObject<MoveCard>(jo.ToString(), SpecifiedSubclassConversion);
            case "Color":
                return JsonConvert.DeserializeObject<ColorCard>(jo.ToString(), SpecifiedSubclassConversion);
            default:
                throw new Exception();
        }
        throw new NotImplementedException();
    }

    public override bool CanWrite
    {
        get { return false; }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException(); // won't be called because CanWrite returns false
    }
}