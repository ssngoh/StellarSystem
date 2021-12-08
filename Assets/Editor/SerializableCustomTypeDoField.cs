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

    static (bool, int) _removingIndex = (false, -1);


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
        {
            triggerGroup.triggerConditions.Add(new TriggerConditions());
        }

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
            buttonRect.width = _buttonWidth * 10;
            
            if (GUI.Button(buttonRect, new GUIContent("Add stat condition", "Add to stat condition to proc list"), EditorStyles.miniButton))
            {
                triggerGroup.triggerConditions[i].statConditionsToProc.AddNewIndex();
            }

            if(triggerGroup.triggerConditions[i].statConditionsToProc != null)
                SetupDictionary(triggerGroup.triggerConditions[i].statConditionsToProc, ref rect);

            rect.y += 20f;
            buttonRect = rect;
            buttonRect.x = 17;
            buttonRect.width = _buttonWidth * 10;

            if (GUI.Button(buttonRect, new GUIContent("Add Status Effect condition", "Add to status effect condition to proc list"), EditorStyles.miniButton))
            {
                triggerGroup.triggerConditions[i].statusEffectConditionsToProc.AddNewIndex();
            }

            if (triggerGroup.triggerConditions[i].statusEffectConditionsToProc != null)
                SetupDictionary(triggerGroup.triggerConditions[i].statusEffectConditionsToProc, ref rect);
        }

        rect.y += 8f;
        EditorGUI.LabelField(rect, "______________________________________________________________________________________________________");
        return triggerGroup;
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

                var removeRect = valueRect;
                removeRect.x = valueRect.xMax + 2;
                removeRect.width = _buttonWidth;
                //if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight))
                //{
                //    RemoveItem(i, key);
                //    break;
                //}
            }
        }

        if (_removingIndex.Item1)
        {
            _removingIndex.Item1 = false;
            _listDictionary.GetDictionaryList.RemoveAt(_removingIndex.Item2);
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
            _removingIndex = (true, index);
        }
    }


    static float GetButtonWidthBasedOnStringLength(string sentence)
    {
        return sentence.Length * 6.5f;
    }

}
