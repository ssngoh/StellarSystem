using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using SEUtils;


[CanEditMultipleObjects]
[Serializable]
public abstract class SerializableHashSetDrawer<T1> : PropertyDrawer
{
    [SerializeField]
    SerializableHashSet<T1> _serializableHashset;

    bool _foldout = true;
    const float _buttonWidth = 18;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        CheckInitialize(property, label);
        if (_foldout)
            return (_serializableHashset.GetTotalItems() + 1) * 34f;

        return 17f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
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
        if (GUI.Button(buttonRect, new GUIContent("+", "Add to hashset"), EditorStyles.miniButton))
        {
            AddNewDefaultItem();
            _foldout = true;
        }

        buttonRect.x -= _buttonWidth * 2;

        if (GUI.Button(buttonRect, new GUIContent("AE", "Fill hashset with Enums"), EditorStyles.miniButtonRight))
        {
            AddAllEnumValues();
            _foldout = true;
        }

        buttonRect.x -= _buttonWidth;

        if (GUI.Button(buttonRect, new GUIContent("X", "Clear hashset"), EditorStyles.miniButtonRight))
        {
            ClearHashSet();
        }


        if (!_foldout)
            return;

        foreach(var hashValue in _serializableHashset.GetHashSet)
        {
            position.y += 34f;
            EditorGUI.BeginChangeCheck();
            var newHashValue = SerializableCustomTypeDoField.DoField(ref position, typeof(T1), (dynamic)hashValue);
            if(EditorGUI.EndChangeCheck())
            {
                try
                {
                    _serializableHashset.RemoveKey(hashValue);
                    _serializableHashset.AddNewItem(newHashValue);
                }
                catch(Exception e)
                {
                    Debug.LogError("Exception when assigning new hashValue to hashset: " + e.Message);
                }
                break;
            }

        }
    }

    private void CheckInitialize(SerializedProperty property, GUIContent label)
    {
        if(_serializableHashset == null)
        {
            var target = property.serializedObject.targetObject;
            _serializableHashset = fieldInfo.GetValue(target) as SerializableHashSet<T1>;
            if(_serializableHashset == null)
            {
                _serializableHashset = new SerializableHashSet<T1>();
                fieldInfo.SetValue(target, _serializableHashset);
            }
        }

        _foldout = EditorPrefs.GetBool(label.text);
    }

    void AddNewItem(T1 key)
    {
        _serializableHashset.AddNewItem(key);
    }

    void RemoveKey(T1 key)
    {
        _serializableHashset.RemoveKey(key);
    }

    void AddAllEnumValues()
    {
        try
        {
            if(typeof(T1).IsEnum)
            {
                foreach(T1 val in Enum.GetValues(typeof(T1)))
                {
                    _serializableHashset.AddNewItem(val);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Adding to serializable dictionary failed because of " + e.Message);
        }
    }

    void AddNewDefaultItem()
    {
        try
        {
            T1 key;
            if (typeof(T1) == typeof(string))
                key = (T1)(object)"";
            else
                key = default(T1);

            _serializableHashset.AddNewItem(key);
        }
        catch(Exception e)
        {
            Debug.LogError("Adding to serializable hashset failed because of " + e.Message);

        }
    }

    void ClearHashSet()
    {
        _serializableHashset.Clear();
    }

}

[CustomPropertyDrawer(typeof(SerializableHashSetInt))]
public class SerializableHashSetIntDrawer : SerializableHashSetDrawer<int> { }
