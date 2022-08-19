using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(StringStringDictionary))]
[CustomPropertyDrawer(typeof(ObjectColorDictionary))]
[CustomPropertyDrawer(typeof(StringColorArrayDictionary))]
[CustomPropertyDrawer(typeof(StringGameObjectDictionary))]
[CustomPropertyDrawer(typeof(SituationColorDictionary))]
[CustomPropertyDrawer(typeof(StringSpriteDictionary))]
[CustomPropertyDrawer(typeof(StringColorDictionary))]
[CustomPropertyDrawer(typeof(BoardSituationSpriteDictionary))]
[CustomPropertyDrawer(typeof(StringSoundDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}

[CustomPropertyDrawer(typeof(ColorArrayStorage))]
public class AnySerializableDictionaryStoragePropertyDrawer: SerializableDictionaryStoragePropertyDrawer {}
