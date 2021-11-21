using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using SEUtils;
using UnityObject = UnityEngine.Object;

[CanEditMultipleObjects]
[Serializable]
public abstract class SerializableDictionaryDrawerList<T1,T2> : PropertyDrawer
{
    [SerializeField]
    SerializableDictionaryList<T1,T2> _listDictionary;

    bool _foldout = true;
    bool _dirty = false;
    const float _buttonWidth = 18f;
    const float _itemHeight = 34f;
    const float _spacingBetweenIndexes = 17f;
    const float _spacingBetweenDictionaryKeys = 17f;

    (bool, int) _removingIndex = (false, -1);

    //Used for buttons we don't want to accidently click. 
    //We can use the command data structure next time to store input so that we can reverse our actions or forward them
    const float _dangerousButton1PositionHeader = 220f;
    const float _dangerousButton1Position = 140f;
    const float _dangerousButton2Position = 250f; 



    readonly Dictionary<Type, Func<Rect, object, object>> _Fields =
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

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // return base.GetPropertyHeight(property, label);
        CheckInitialize(property, label);
        if (_foldout && _listDictionary != null)
            return _listDictionary.TotalDictionaryItems * _spacingBetweenDictionaryKeys + (_listDictionary.GetDictionaryList.Count+1) * _itemHeight;

        return 34f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUI.BeginProperty(position, label, property);

        //int arrayBracketStart = property.propertyPath.IndexOf("[");
        //int index = -1;
        //if (arrayBracketStart != -1)
        //{
        //    int arrayBracketEnd = property.propertyPath.IndexOf("]", arrayBracketStart);
        //    index = int.Parse(property.propertyPath.Substring(arrayBracketStart + 1, arrayBracketEnd - arrayBracketStart - 1));
        //    Debug.LogWarning("Index is  " + index);

        //    //if()
        //}

        CheckInitialize(property, label); 
        position.height = 17f;

        var foldoutRect = position;
        foldoutRect.width = _buttonWidth;
        EditorGUI.BeginChangeCheck();

        _foldout = EditorGUI.Foldout(foldoutRect, _foldout, label, true);
        if (EditorGUI.EndChangeCheck())
            EditorPrefs.SetBool(label.text, _foldout);

        var buttonRect = position;
        buttonRect.x = position.width; 
        buttonRect.width = _buttonWidth;
        if(GUI.Button(buttonRect, new GUIContent("+", "Add to list"), EditorStyles.miniButton))
        {
            AddNewIndex();
           // AddNewItem(index);
            _foldout = true;
        }

        buttonRect.x = _dangerousButton1PositionHeader;
        string buttonName = "Clear Entire List";
        buttonRect.width = GetButtonWidthBasedOnStringLength(buttonName);

        if (GUI.Button(buttonRect, new GUIContent(buttonName, "Clear entire list. THIS IS IRREVERSIBLE"), EditorStyles.miniButtonRight))
            ClearEntireList();

        if (!_foldout | _listDictionary == null)
            return;

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
                var newKey = DoField(keyRect, typeof(T1), key);
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
                value = DoField(valueRect, typeof(T2), value);
                if (EditorGUI.EndChangeCheck())
                {
                    _listDictionary.GetDictionaryList[i].SetValue(newKey, value);
                    break;
                }

                var removeRect = valueRect;
                removeRect.x = valueRect.xMax + 2;
                removeRect.width = _buttonWidth;
                if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight))
                {
                    RemoveItem(i, key);
                    break;
                }
            }
        }

        if(_removingIndex.Item1)
        {
            _removingIndex.Item1 = false;
            _listDictionary.GetDictionaryList.RemoveAt(_removingIndex.Item2);
        }

    }


    T DoField<T>(Rect rect, Type type, T value)
    {
        Func<Rect, object, object> field;
        if (_Fields.TryGetValue(type, out field))
            return (T)field(rect, value);

        if (type.IsEnum)
            return (T)(object)EditorGUI.EnumPopup(rect, (Enum)(object)value);

        if(typeof(UnityObject).IsAssignableFrom(type))
            return (T)(object)EditorGUI.ObjectField(rect, (UnityObject)(object)value, type, true);

        //Find some way to put it in a better place
        if(type == typeof(Modifier))
        {
            Modifier mod = ((Modifier)(object)value);

            var modifierAmount = mod.amount;
            var modType = mod.mod_type;
            
            float halfRect = rect.width / 2;
            rect.width /= 2;

            rect.x += halfRect;
            mod.amount = DoField(rect, modifierAmount.GetType(), modifierAmount);
            rect.x -= halfRect;
            mod.mod_type = DoField(rect, modType.GetType(), modType);

            return (T)(object)mod;

        }

        Debug.LogError("Type is not supported: " + type);
        return value;
    }


    private void CheckInitialize(SerializedProperty property, GUIContent label)
    {
        if(_listDictionary == null)
        {
            var target = property.serializedObject.targetObject;
            try
            {
                var tempObj = fieldInfo.GetValue(target);
                if (tempObj is SerializableDictionaryList<T1, T2>)
                {
                    _listDictionary = fieldInfo.GetValue(target) as SerializableDictionaryList<T1, T2>;
                    if (_listDictionary == null)
                    {
                        _listDictionary = new SerializableDictionaryList<T1, T2>();
                        fieldInfo.SetValue(target, _listDictionary);
                    }
                }
            }
            catch (Exception e)
            {
                //If its a nested class. Only works if its nested with depth 1
                target = property.serializedObject.targetObject;
                var targetObjectClassType = target.GetType();
                var allFields = targetObjectClassType.GetFields();

                foreach (var field in allFields) 
                {
                    //SEUtils.TriggerConditions _triggerCondition -> targetObjectClassType.GetFields()
                    var obj = targetObjectClassType.GetField(field.Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                    var actualObj = obj.GetValue(target);
                    var temp = fieldInfo.GetValue(actualObj);

                    if (temp is SerializableDictionaryList<T1, T2>)
                    {
                        _listDictionary = fieldInfo.GetValue(actualObj) as SerializableDictionaryList<T1, T2>;
                        if (_listDictionary == null)
                        {
                            _listDictionary = new SerializableDictionaryList<T1, T2>();
                            fieldInfo.SetValue(actualObj, _listDictionary);
                        }
                    }
                }

            }

            _foldout = EditorPrefs.GetBool(label.text);
        }
    }

    void SetupDictionaryButtons(Rect referenceButtonRect, SerializableDictionary<T1,T2> _serializableDict, int index)
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
            _foldout = true;
        }

        float fillButtonWidth = _buttonWidth + 20;
        buttonRect.x -= fillButtonWidth;
        buttonRect.width = 34;
        if (GUI.Button(buttonRect, new GUIContent("FILL", "Fill dictionary with Enums"), EditorStyles.miniButtonRight))
        {
            try
            {
                if(typeof(T1).IsEnum)
                {
                    foreach(T1 val in Enum.GetValues(typeof(T1)))
                        _serializableDict.AddNewItem(val, default(T2));
                }

                _foldout = true;
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
            RemoveIndex(index);   
        }
    }


    void RemoveItem(int index, T1 key)
    {
        _listDictionary.GetDictionaryList[index].Remove(key);
    }

    void RemoveIndex(int index)
    {
        _removingIndex = (true, index); 
    }

    void ClearEntireList()
    {
        _listDictionary.Clear();
    }

    void AddNewIndex()
    {
        _listDictionary.AddNewIndex();
    }

    void AddNewItem(int index)
    {
        try
        {
            T1 key;
            if (typeof(T1) == typeof(string))
                key = (T1)(object)"";
            else
                key = default(T1);

            _listDictionary.AddNewItem(key, default(T2), index);
        }
        catch(Exception e)
        {
            Debug.LogError("Adding to serializable dictionary failed because of " + e.Message);
        }
    }

    float GetButtonWidthBasedOnStringLength(string sentence)
    {
        return sentence.Length * 6.5f;
    }


}

[CustomPropertyDrawer(typeof(StatusEffectModifierDictionaryList))]
public class StatusEffectModifierDictionaryDrawerList : SerializableDictionaryDrawerList<SEUtils.STATUS_EFFECTS, SEUtils.Modifier> { }
[CustomPropertyDrawer(typeof(StatModifierDictionaryList))]
public class StatModifierDictionaryDrawerList : SerializableDictionaryDrawerList<SEUtils.STAT_TYPE, SEUtils.Modifier> { }
