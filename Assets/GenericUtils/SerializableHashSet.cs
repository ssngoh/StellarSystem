using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] public class SerializableHashSetInt : SerializableHashSet<int> { }

[Serializable]
public class SerializableHashSet<T1> : ISerializationCallbackReceiver
{
    public HashSet<T1> GetHashSet => _serializableHashSet;

    [HideInInspector, SerializeField]
    HashSet<T1> _serializableHashSet = new HashSet<T1>();

    List<T1> _save = new List<T1>();


    public virtual int GetTotalItems()
    {
        return _serializableHashSet.Count;
    }

    public void AddNewItem(T1 key)
    {
        try
        {
            _serializableHashSet.Add(key);
        }
        catch (Exception e)
        {
            Debug.LogError("Adding to serializable hashset failed because of " + e.Message);
        } 
    }

    public void RemoveKey(T1 key)
    {
        _serializableHashSet.Remove(key);
    }

    public void Clear()
    {
        _serializableHashSet.Clear();
    }

    public void OnBeforeSerialize()
    {
        _save.Clear();

        foreach (var kvp in _serializableHashSet)
            _save.Add(kvp);
        //throw new System.NotImplementedException();
    }

    public void OnAfterDeserialize()
    { 
        _serializableHashSet = new HashSet<T1>();
         
        foreach (var kvp in _save)
            _serializableHashSet.Add(kvp);
        //throw new System.NotImplementedException();
    }


}
