using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using SEUtils;


[CanEditMultipleObjects]
[Serializable]
public abstract class SerializableListDrawer<T1> : PropertyDrawer
{
    [SerializeField]
    SerializableList<T1> _serializableList;

    bool _foldout = true;
    const float _buttonWidth = 18;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        CheckInitialize(property, label);
        if (_foldout)
            return (_serializableList.GetTotalItems() + 1) * 34f;

        return 34f;
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
        if (GUI.Button(buttonRect, new GUIContent("+", "Add to list"), EditorStyles.miniButton))
        {
            AddNewIndex();
            // AddNewItem(index);
            _foldout = true;
        }

        buttonRect.x = position.width - (_buttonWidth + 5);
        if (GUI.Button(buttonRect, new GUIContent("-", "Remove from list"), EditorStyles.miniButton))
        {
            RemoveLatestIndex();
            _foldout = true;
        }

        int index = 0;
        foreach(var listValue in _serializableList.GetList)
        {
            position.y += 34f;
            EditorGUI.BeginChangeCheck();
            var newListValue = SerializableCustomTypeDoField.DoField(ref position, typeof(T1), (dynamic)listValue);
            if(EditorGUI.EndChangeCheck())
            {
                try
                {
                    _serializableList.GetList[index] = newListValue;
                }
                catch(Exception e)
                {
                    Debug.LogError("Exception when assigning new ListValue to list at index: " + index + "  " + e.Message);
                }
                break;
            }

            ++index;
        }
    }

    private void CheckInitialize(SerializedProperty property, GUIContent label)
    {
        if(_serializableList == null)
        {
            var target = property.serializedObject.targetObject;
            _serializableList = fieldInfo.GetValue(target) as SerializableList<T1>;
            if(_serializableList == null)
            {
                _serializableList = new SerializableList<T1>();
                fieldInfo.SetValue(target, _serializableList);
            }
        }

        _foldout = EditorPrefs.GetBool(label.text);
    }

    void AddNewIndex()
    {
        _serializableList.AddNewIndex();
    }

    void RemoveLatestIndex()
    {
        _serializableList.RemoveLatestIndex();
    }
}


[CustomPropertyDrawer(typeof(TriggerGroupList))]
public class TriggerGroupListDrawer : SerializableListDrawer<SEUtils.TriggerGroup> { }
