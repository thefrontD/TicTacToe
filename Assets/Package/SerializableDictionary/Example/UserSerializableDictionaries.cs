using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class StringStringDictionary : SerializableDictionary<string, string> {}
[Serializable]
public class SituationColorDictionary : SerializableDictionary<BoardSituation, Color> {}

[Serializable]
public class ObjectColorDictionary : SerializableDictionary<UnityEngine.Object, Color> {}

[Serializable]
public class ColorArrayStorage : SerializableDictionary.Storage<Color[]> {}
[Serializable]
public class StringSpriteDictionary : SerializableDictionary<string, Sprite> {}
[Serializable]
public class StringColorDictionary : SerializableDictionary<string, Color> {}
[Serializable]
public class BoardSituationSpriteDictionary : SerializableDictionary<BoardSituation, Sprite> {}
[Serializable]
public class StringSoundDictionary : SerializableDictionary<string, AudioClip> {}

[Serializable]
public class StringColorArrayDictionary : SerializableDictionary<string, Color[], ColorArrayStorage> {}

[Serializable]
public class MyClass
{
    public int i;
    public string str;
}

[Serializable]
public class QuaternionMyClassDictionary : SerializableDictionary<Quaternion, MyClass> {}

[Serializable]
public class StringGameObjectDictionary : SerializableDictionary<string, GameObject> {}
[Serializable]
public class DebuffGameObjectDictionary : SerializableDictionary<Debuff, GameObject> {}