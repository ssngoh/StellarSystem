using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[Serializable] public class StringIntDictionary : SerializableDictionary<string, int> { }
[Serializable] public class StatusEffectIntDictionary : SerializableDictionary<SEUtils.STATUS_EFFECTS, int> { }
[Serializable] public class CharacterActionTypesIntDictionary : SerializableDictionary<SECharacters.CHARACTER_ACTION_TYPES, int> { }
[Serializable] public class CharacterClassesIntDictionary : SerializableDictionary<SECharacters.CHARACTER_CLASSES, int> { }
[Serializable] public class StatTypeIntDictionary : SerializableDictionary<SEUtils.STAT_TYPE, int> { }
[Serializable] public class SkillTypeIntDictionary : SerializableDictionary<SESkills.SkillTypes, int> { }
[Serializable] public class StatusEffectModifierDictionary : SerializableDictionary<SEUtils.STATUS_EFFECTS, SEUtils.Modifier> { }


[Serializable]
public class SerializableDictionary<T1,T2> : ISerializationCallbackReceiver
{
    //This interface is supported on objects that are referenced by SerializeReference.

    public int Count => _dictionary.Count;
    public Dictionary<T1, T2> GetDictionary => _dictionary;

    Dictionary<T1, T2> _dictionary = new Dictionary<T1, T2>();

    [HideInInspector,SerializeField]
    List<T1> _keyList = new List<T1>();

    [HideInInspector,SerializeField]
    List<T2> _valueList = new List<T2>();



    /* The order of callback execution is not guaranteed between such objects. 
     * However it is guaranteed that the main object's OnBeforeSerialize callback 
     * would be invoked before those implemented by the referenced objects. 
     * And OnAfterDeserialize on the main object would be invoked after 
     * it is called on all the referenced objects.
     * 
     * When is OnBeforeSerialize called
     * Called once before a recompile is handled by the Unity Editor
     * Called multiple times when the scene is saved (ie two or three times)
     * Called every frame if the inspector for the object is open in the editor
     * Not called in an actual build (in my simple test)
     * 
     * When is OnAfterDeserialize called:
     * Called once after a recompile
     * Called once after the scene is loaded
     * Called once in an actual build (in my simple test)
     * So, if you have the editor open and an inspector open, 
     *  OnBeforeSerialize is called every frame by the OnInspectorUpdate function
     */

    public void Clear()
    {
        _dictionary.Clear();
    }

    public void Remove(T1 key)
    {
        try
        {
            _dictionary.Remove(key);
        }
        catch (Exception e)
        {
            Debug.LogError("Removing from serializable dictionary failed because of " + e.Message);
        }
    }

    public void AddNewItem(T1 key, T2 value)
    {
        try
        {
            _dictionary.Add(key, value);
        }
        catch (Exception e)
        {
            Debug.LogError("Adding to serializable dictionary failed because of " + e.Message);
        }
    }

    public void SetValue(T1 key, T2 value)
    {
        try
        {
            _dictionary[key] = value;
            //_keyList.Add(default(T1));
        }
        catch (Exception e)
        {
            Debug.LogError("Set value to dictionary failed because of " + e.Message);
        }
    }

    public void OnBeforeSerialize()
    {
        _keyList.Clear();
        _valueList.Clear();

        foreach (var kvp in _dictionary)
        {
            _keyList.Add(kvp.Key);
            _valueList.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        _dictionary = new Dictionary<T1, T2>();
         
        for (int i = 0; i != Math.Min(_keyList.Count, _valueList.Count); i++)
            _dictionary.Add(_keyList[i], _valueList[i]);

    }

}

