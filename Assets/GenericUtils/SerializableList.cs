using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable] public class TriggerGroupList : SerializableList<SEUtils.TriggerGroup> 
{
    public override int GetTotalItems()
    {
        int totalItems = 0;

        totalItems += GetList.Count;

        for(int i = 0; i < GetList.Count; ++i)
        {
            if (GetList[i].triggerConditions != null)
            {
                for (int j = 0; j < GetList[i].triggerConditions.Count; ++j)
                {
                    totalItems += GetList[i].triggerConditions[j].statConditionsToProc != null ? GetList[i].triggerConditions[j].statConditionsToProc.GetTotalDictionaryItems() : 0;
                    totalItems += GetList[i].triggerConditions[j].statusEffectConditionsToProc != null ? GetList[i].triggerConditions[j].statusEffectConditionsToProc.GetTotalDictionaryItems() : 0;
                    totalItems += GetList[i].triggerConditions[j].triggersToProc != null ? GetList[i].triggerConditions[j].triggersToProc.Count : 0;
                    totalItems += 1; //comparisonType
                    totalItems += 1; //conditionType
                }
            }
        }

        return totalItems;
    }
}

[Serializable]
public class SerializableList<T1> : ISerializationCallbackReceiver
{
    public List<T1> GetList => _serializableList;

    [HideInInspector, SerializeField]
    List<T1> _serializableList = new List<T1>();


    public virtual int GetTotalItems()
    {
        return _serializableList.Count;
    }

    public void AddNewIndex()
    {
        _serializableList.Add(default(T1));
    }

    public void RemoveLatestIndex()
    {
        _serializableList.RemoveAt(_serializableList.Count - 1);
    }

    public void Clear()
    {
        _serializableList.Clear();
    }

    public void OnAfterDeserialize()
    {

        //throw new System.NotImplementedException();
    }

    public void OnBeforeSerialize()
    {

        //throw new System.NotImplementedException();
    }
}
