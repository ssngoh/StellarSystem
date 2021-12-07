using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[Serializable] public class StatusEffectModifierDictionaryList : SerializableDictionaryList<SEUtils.STATUS_EFFECTS, SEUtils.Modifier> { }
[Serializable] public class StatModifierDictionaryList : SerializableDictionaryList<SEUtils.STAT_TYPE, SEUtils.Modifier> { }


public class SerializableDictionaryList<T1, T2> : ISerializationCallbackReceiver
{
    public int TotalDictionaryItems => GetTotalDictionaryItems();
    public List<SerializableDictionary<T1, T2>> GetDictionaryList => _dictionaryList;

    [HideInInspector, SerializeField]
    List<SerializableDictionary<T1, T2>> _dictionaryList = new List<SerializableDictionary<T1, T2>>();

    public int GetTotalDictionaryItems()
    {
        int totalDictionaryItems = 0;
        for (int i = 0; i < _dictionaryList.Count; ++i)
        {
            foreach (var inner in _dictionaryList[i].GetDictionary)
            {
                ++totalDictionaryItems;
            }
        }

        return totalDictionaryItems;
    }

    public void Clear()
    {
        _dictionaryList.Clear();
    }

    public void AddNewItem(T1 key, T2 value, int index)
    {
        if(index >= _dictionaryList.Count)
        {
            while(_dictionaryList.Count <= index)
            {
                _dictionaryList.Add(new SerializableDictionary<T1, T2>());
            }
        }

        try
        {
            _dictionaryList[index].AddNewItem(key, value);
        }
        catch (Exception e)
        {
            Debug.LogError("Adding to serializable dictionary failed because of " + e.Message);
        }
    }

    public void AddNewIndex()
    {
        _dictionaryList.Add(new SerializableDictionary<T1, T2>());
    }

    public void SetValue(T1 key, T2 value, int index)
    {
        if (index >= _dictionaryList.Count)
        {
            while (_dictionaryList.Count <= index)
            {
                _dictionaryList.Add(new SerializableDictionary<T1, T2>());
            }
        }

        try
        {
            _dictionaryList[index].SetValue(key, value);
            //_keyList.Add(default(T1));
        }
        catch (Exception e)
        {
            Debug.LogError("Set value to dictionary failed because of " + e.Message);
        }
    }

    public void OnBeforeSerialize()
    {
        //Debug.LogWarning("Before serialize dic list");
    }

    public void OnAfterDeserialize()
    {
        
    }


}
