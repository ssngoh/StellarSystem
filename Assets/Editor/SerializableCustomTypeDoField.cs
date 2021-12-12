using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using SEUtils;
using UnityObject = UnityEngine.Object;

public static class SerializableCustomTypeDoField 
{
    const float _spacingBetweenIndexes = 17f;
    const float _spacingBetweenDictionaryKeys = 17f;
    const float _buttonWidth = 18f;
    const float _dangerousButton1PositionHeader = 220f;
    const float _dangerousButton1Position = 140f;
    const float _dangerousButton2Position = 250f;

    static (bool, int) _removingDictionaryIndex = (false, -1);


    public static readonly Dictionary<Type, Func<Rect, object, object>> _Fields =
    new Dictionary<Type, Func<Rect, object, object>>()
    {
            { typeof(int), (rect, value) => EditorGUI.IntField(rect, (int)value) },
            { typeof(float), (rect, value) => EditorGUI.FloatField(rect, (float)value) },
            { typeof(string), (rect, value) => EditorGUI.TextField(rect, (string)value) },
            { typeof(bool), (rect, value) => EditorGUI.Toggle(rect, (bool)value) },
            { typeof(Vector2), (rect, value) => EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value) },
            { typeof(Vector3), (rect, value) => EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value) },
            { typeof(Bounds), (rect, value) => EditorGUI.BoundsField(rect, (Bounds)value) },
            { typeof(Rect), (rect, value) => EditorGUI.RectField(rect, (Rect)value) },
    };


    public static T DoField<T>(ref Rect rect, Type type, T value) 
    {
        Func<Rect, object, object> field;
        if (_Fields.TryGetValue(type, out field))
            return (T)field(rect, value);

        if (type.IsEnum)
            return (T)(object)EditorGUI.EnumPopup(rect, (Enum)(object)value);

        if (typeof(UnityObject).IsAssignableFrom(type))
            return (T)(object)EditorGUI.ObjectField(rect, (UnityObject)(object)value, type, true);

        Debug.LogError("Type is not supported: " + type);
        return value;
    }

    public static Modifier DoField(ref Rect rect, Type type, Modifier modifier)
    {
        var modifierAmount = modifier.amount;
        var modType = modifier.mod_type;

        float halfRect = rect.width / 2;
        rect.width /= 2;

        rect.x += halfRect;
        modifier.amount = DoField(ref rect, modifierAmount.GetType(), modifierAmount);
        rect.x -= halfRect;
        modifier.mod_type = DoField(ref rect, modType.GetType(), modType);

        return modifier;
    }


    public static TriggerGroup DoField(ref Rect rect, Type type, TriggerGroup triggerGroup)
    {
        //Rect width is now the entire width of the inspector
        var triggerGroupRect = rect;
        triggerGroupRect.width = rect.width / 6;

        triggerGroup.mainConditionType = DoField(ref triggerGroupRect, triggerGroup.mainConditionType.GetType(), triggerGroup.mainConditionType);

        var addToListButtonRect = rect;
        addToListButtonRect.x = rect.width / 4;
        addToListButtonRect.width = _buttonWidth;

        if (triggerGroup.triggerConditions == null)
            triggerGroup.triggerConditions = new List<TriggerConditions>();
        

        if (GUI.Button(addToListButtonRect, new GUIContent("+", "Add to list"), EditorStyles.miniButton))
            triggerGroup.triggerConditions.Add(new TriggerConditions());
        

        addToListButtonRect.x = rect.width /4 - 23f;
        if (GUI.Button(addToListButtonRect, new GUIContent("-", "Remove from list"), EditorStyles.miniButton))
        {
            if(triggerGroup.triggerConditions.Count > 0)
                triggerGroup.triggerConditions.RemoveAt(triggerGroup.triggerConditions.Count - 1);
        }

        for (int i = 0; i < triggerGroup.triggerConditions.Count; ++i)
        {
            rect.y += 30f; //Base decrement

            var buttonRect = rect;
            buttonRect.x = 17;
            buttonRect.width = GetButtonWidthBasedOnStringLength("Add stat condition");
            
            if (GUI.Button(buttonRect, new GUIContent("Add stat condition", "Add to stat condition to proc list"), EditorStyles.miniButton))
                triggerGroup.triggerConditions[i].statConditionsToProc.AddNewIndex();

            if (triggerGroup.triggerConditions[i].statConditionsToProc != null)
                SetupDictionary(triggerGroup.triggerConditions[i].statConditionsToProc, ref rect);

            AddSemiBreakLine(ref rect);

            rect.y += 20f;
            buttonRect = rect;
            buttonRect.x = 17;
            buttonRect.width = GetButtonWidthBasedOnStringLength("Add Status Effect condition");

            if (GUI.Button(buttonRect, new GUIContent("Add Status Effect condition", "Add to status effect condition to proc list"), EditorStyles.miniButton))
            {
                triggerGroup.triggerConditions[i].statusEffectConditionsToProc.AddNewIndex();
            }

            if (triggerGroup.triggerConditions[i].statusEffectConditionsToProc != null)
                SetupDictionary(triggerGroup.triggerConditions[i].statusEffectConditionsToProc, ref rect);

            AddSemiBreakLine(ref rect);

            rect.y += 20f;
            var textRect = rect;
            
            EditorGUI.LabelField(textRect, "Triggers to proc", GetTextGUIStyle());
            if (triggerGroup.triggerConditions[i].triggersToProc != null)
                SetupHashSet(triggerGroup.triggerConditions[i].triggersToProc, ref rect);

        }

        AddBreakLine(ref rect);
        return triggerGroup;
    }


    static void SetupList<T1>(List<T1> _list, ref Rect position)
    {
        var buttonRect = position;
        buttonRect.x = position.width;
        buttonRect.width = _buttonWidth;
        if (GUI.Button(buttonRect, new GUIContent("+", "Add to list"), EditorStyles.miniButton))
        {
            _list.Add(default);
        }

        buttonRect.x = position.width - (_buttonWidth + 5);
        if (GUI.Button(buttonRect, new GUIContent("-", "Remove from list"), EditorStyles.miniButton))
        {
            _list.RemoveAt(_list.Count - 1);
        }

        int index = 0;
        foreach (var listValue in _list)
        {
            position.y += 34f;
            EditorGUI.BeginChangeCheck();
            var newListValue = SerializableCustomTypeDoField.DoField(ref position, typeof(T1), (dynamic)listValue);
            if (EditorGUI.EndChangeCheck())
            {
                try
                {
                    _list[index] = newListValue;
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception when assigning new ListValue to list at index: " + index + "  " + e.Message);
                }
                break;
            }

            ++index;
        }
    }


    static void SetupList<T1>(SerializableList<T1> _serializableList, ref Rect position)
    {
        var buttonRect = position;
        buttonRect.x = position.width;
        buttonRect.width = _buttonWidth;
        if (GUI.Button(buttonRect, new GUIContent("+", "Add to list"), EditorStyles.miniButton))
        {
            _serializableList.AddNewIndex();
        }

        buttonRect.x = position.width - (_buttonWidth + 5);
        if (GUI.Button(buttonRect, new GUIContent("-", "Remove from list"), EditorStyles.miniButton))
        {
            _serializableList.RemoveLatestIndex();
        }

        int index = 0;
        foreach (var listValue in _serializableList.GetList)
        {
            position.y += 34f;
            EditorGUI.BeginChangeCheck();
            var newListValue = SerializableCustomTypeDoField.DoField(ref position, typeof(T1), (dynamic)listValue);
            if (EditorGUI.EndChangeCheck())
            {
                try
                {
                    _serializableList.GetList[index] = newListValue;
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception when assigning new ListValue to list at index: " + index + "  " + e.Message);
                }
                break;
            }

            ++index;
        }
    }

    static void SetupHashSet<T1>(HashSet<T1> _hashSet, ref Rect position)
    {
        SetupHashSetButtons(position, _hashSet);
        foreach (var hashValue in _hashSet)
        {
            position.y += _spacingBetweenDictionaryKeys;
            var keyRect = position;
            keyRect.width /= 2;
            keyRect.width -= 14;
            EditorGUI.BeginChangeCheck();
            var newHashValue = DoField(ref keyRect, typeof(T1), (dynamic)hashValue);
            if (EditorGUI.EndChangeCheck())
            {
                try
                {
                    _hashSet.Remove(hashValue);
                    _hashSet.Add(newHashValue);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception when adding key:" + e.Message);
                }
                break;
            }
     
            var removeRect = position;
            removeRect.x = position.width;
            removeRect.width = _buttonWidth;

            if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButton))
            {
                _hashSet.Remove(hashValue);
                break;
            }

        }
    }

    static void SetupHashSetButtons<T1>(Rect referenceButtonRect, HashSet<T1> _serializableHashSet)
    {
        var buttonRect = referenceButtonRect;
        buttonRect.x = referenceButtonRect.width;
        buttonRect.width = _buttonWidth;

        if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButton))
        {
            try
            {
                T1 key;
                if (typeof(T1) == typeof(string))
                    key = (T1)(object)"";
                else
                    key = default(T1);

                _serializableHashSet.Add(key);
            }
            catch (Exception e)
            {
                Debug.LogError("Adding to hash set failed because of " + e.Message);
            }
        }

        float fillButtonWidth = _buttonWidth + 20;
        buttonRect.x -= fillButtonWidth;
        buttonRect.width = 34;
        if (GUI.Button(buttonRect, new GUIContent("FILL", "Fill hashset with Enums"), EditorStyles.miniButtonRight))
        {
            try
            {
                if (typeof(T1).IsEnum)
                {
                    foreach (T1 val in Enum.GetValues(typeof(T1)))
                        _serializableHashSet.Add(val);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Adding to hash set failed because of " + e.Message);
            }
        }

        string buttonName = "Clear Hash set";
        buttonRect.x = _dangerousButton2Position;
        buttonRect.width = GetButtonWidthBasedOnStringLength(buttonName);

        if (GUI.Button(buttonRect, new GUIContent(buttonName, "Clear Hash set. THIS IS IRREVERSIBLE"), EditorStyles.miniButtonRight))
        {
            _serializableHashSet.Clear();
        }
    }

    static void SetupDictionary<T1,T2>(SerializableDictionaryList<T1, T2> _listDictionary, ref Rect position)
    {
        for (int i = 0; i < _listDictionary.GetDictionaryList.Count; ++i)
        {
            position.y += _spacingBetweenIndexes;
            SetupDictionaryButtons(position, _listDictionary.GetDictionaryList[i], i);

            foreach (var kvp in _listDictionary.GetDictionaryList[i].GetDictionary)
            {
                var key = kvp.Key;
                var value = kvp.Value;

                position.y += _spacingBetweenDictionaryKeys;
                var keyRect = position;
                keyRect.width /= 2;
                keyRect.width -= 14;
                EditorGUI.BeginChangeCheck();
                var newKey = DoField(ref keyRect, typeof(T1), (dynamic)key);
                if (EditorGUI.EndChangeCheck())
                {
                    try
                    {
                        _listDictionary.GetDictionaryList[i].Remove(key);
                        _listDictionary.GetDictionaryList[i].AddNewItem(newKey, value);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Exception when adding key:" + e.Message);
                    }
                    break;
                }

                var valueRect = position;
                valueRect.x = position.width / 2 + 15;
                valueRect.width = keyRect.width - _buttonWidth;
                EditorGUI.BeginChangeCheck();
                value = DoField(ref valueRect, typeof(T2), (dynamic)value);
                if (EditorGUI.EndChangeCheck())
                {
                    _listDictionary.GetDictionaryList[i].SetValue(newKey, value);
                    break;
                }

                var removeRect = position;
                removeRect.x = position.width;
                removeRect.width = _buttonWidth;
     
                if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButton))
                {
                    _listDictionary.GetDictionaryList[i].Remove(key);
                    break;
                }
            }
        }

        if (_removingDictionaryIndex.Item1)
        {
            _removingDictionaryIndex.Item1 = false;
            _listDictionary.GetDictionaryList.RemoveAt(_removingDictionaryIndex.Item2);
        }
    }


    static void SetupDictionaryButtons<T1,T2>(Rect referenceButtonRect, SerializableDictionary<T1, T2> _serializableDict, int index)
    {
        var buttonRect = referenceButtonRect;
        buttonRect.x = referenceButtonRect.width;
        buttonRect.width = _buttonWidth;

        if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButton))
        {
            try
            {
                T1 key;
                if (typeof(T1) == typeof(string))
                    key = (T1)(object)"";
                else
                    key = default(T1);

                _serializableDict.AddNewItem(key, default(T2));
            }
            catch (Exception e)
            {
                Debug.LogError("Adding to serializable dictionary failed because of " + e.Message);
            }

            // _serializableDict.AddNewItem()
           // _foldout = true;
        }

        float fillButtonWidth = _buttonWidth + 20;
        buttonRect.x -= fillButtonWidth;
        buttonRect.width = 34;
        if (GUI.Button(buttonRect, new GUIContent("FILL", "Fill dictionary with Enums"), EditorStyles.miniButtonRight))
        {
            try
            {
                if (typeof(T1).IsEnum)
                {
                    foreach (T1 val in Enum.GetValues(typeof(T1)))
                        _serializableDict.AddNewItem(val, default(T2));
                }

               // _foldout = true;
            }
            catch (Exception e)
            {
                Debug.LogError("Adding to serializable dictionary failed because of " + e.Message);
            }
        }

        string buttonName = "Clear Dictionary";
        buttonRect.x = _dangerousButton2Position;
        buttonRect.width = GetButtonWidthBasedOnStringLength(buttonName);

        if (GUI.Button(buttonRect, new GUIContent(buttonName, "Clear dictionary. THIS IS IRREVERSIBLE"), EditorStyles.miniButtonRight))
        {
            _serializableDict.Clear();
        }

        buttonName = "Delete Index";
        buttonRect.x = _dangerousButton1Position;
        buttonRect.width = GetButtonWidthBasedOnStringLength(buttonName);

        if (GUI.Button(buttonRect, new GUIContent(buttonName, "Delete this index. THIS IS IRREVERSIBLE"), EditorStyles.miniButtonRight))
        {
            _removingDictionaryIndex = (true, index);
        }
    }


    static float GetButtonWidthBasedOnStringLength(string sentence)
    {
        return sentence.Length * 6.5f;
    }

    static void AddBreakLine(ref Rect position)
    {
        position.y += 8f;
        EditorGUI.LabelField(position, "______________________________________________________________________________________________________________________________________");
       
    }

    static void AddSemiBreakLine(ref Rect position)
    {
        position.y += 8f;
        EditorGUI.LabelField(position, "______________________________________");
      
    }

    static GUIStyle GetTextGUIStyle()
    {
        GUIStyle textStyle = new GUIStyle(); //We can shift this to a global util function that only initializes once
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.normal.textColor = Color.white;

        return textStyle;
    }

}
