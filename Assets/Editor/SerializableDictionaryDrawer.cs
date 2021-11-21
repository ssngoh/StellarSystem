using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using SEUtils;
using UnityObject = UnityEngine.Object;

[CanEditMultipleObjects]
[Serializable]
public abstract class SerializableDictionaryDrawer<T1,T2> : PropertyDrawer
{
    [SerializeField]
    SerializableDictionary<T1, T2> _dictionary;

    bool _foldout = true;
    const float _buttonWidth = 18f;
  
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // return base.GetPropertyHeight(property, label);
        CheckInitialize(property, label);
        if (_foldout)
            return (_dictionary.Count + 1) * 17f;
        return 17f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUI.BeginProperty(position, label, property);

        CheckInitialize(property, label); 
        position.height = 17f;

        var foldoutRect = position;
        foldoutRect.width -= 2 * _buttonWidth;
        EditorGUI.BeginChangeCheck();

        _foldout = EditorGUI.Foldout(foldoutRect, _foldout, label, true);
        if (EditorGUI.EndChangeCheck())
            EditorPrefs.SetBool(label.text, _foldout);

        var buttonRect = position;
        buttonRect.x = position.width - _buttonWidth + position.x;
        buttonRect.width = _buttonWidth + 2;
        if(GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButton))
        {
            AddNewItem();
            _foldout = true;
        }

        buttonRect.x -= _buttonWidth * 2;

        if (GUI.Button(buttonRect, new GUIContent("AE", "Fill dictionary with Enums"), EditorStyles.miniButtonRight))
        {
            AddAllEnumValues();
            _foldout = true;
        }

        buttonRect.x -= _buttonWidth;

        if (GUI.Button(buttonRect, new GUIContent("X", "Clear dictionary"), EditorStyles.miniButtonRight))
        {
            ClearDictionary();
        }


        if (!_foldout)
            return;

        foreach(var kvp in _dictionary.GetDictionary)
        {
            var key = kvp.Key;
            var value = kvp.Value;

            position.y += 17f;
            var keyRect = position;
            keyRect.width /= 2;
            keyRect.width -= 14; 
            EditorGUI.BeginChangeCheck();
            var newKey = DoField(keyRect, typeof(T1), key);
            if(EditorGUI.EndChangeCheck())
            {
                try
                {
                    _dictionary.Remove(key);
                    _dictionary.AddNewItem(newKey, value);
                }
                catch(Exception e)
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
            if(EditorGUI.EndChangeCheck())
            {
                _dictionary.SetValue(newKey, value);
                break;
            }
             
            var removeRect = valueRect;
            removeRect.x = valueRect.xMax + 2;
            removeRect.width = _buttonWidth;
            if(GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight))
            {
                RemoveItem(key);
                break;
            }
        }

    }

    static readonly Dictionary<Type, Func<Rect, object, object>> _Fields =
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
        if(_dictionary == null)
        {
            var target = property.serializedObject.targetObject; 
            _dictionary = fieldInfo.GetValue(target) as SerializableDictionary<T1, T2>;
            if(_dictionary == null)
            {
                _dictionary = new SerializableDictionary<T1, T2>();
                fieldInfo.SetValue(target, _dictionary);
            }

            _foldout = EditorPrefs.GetBool(label.text);
        }
    }

    void RemoveItem(T1 key)
    {
        _dictionary.Remove(key);
    }

    void ClearDictionary()
    {
        _dictionary.Clear();
    }

    void AddNewItem()
    {
        try
        {
            T1 key;
            if (typeof(T1) == typeof(string))
                key = (T1)(object)"";
            else
                key = default(T1);

            _dictionary.AddNewItem(key, default(T2));
        }
        catch(Exception e)
        {
            Debug.LogError("Adding to serializable dictionary failed because of " + e.Message);
        }
    }

    void AddAllEnumValues()
    {
        try
        {
            if (typeof(T1).IsEnum)
            {
                foreach (T1 val in Enum.GetValues(typeof(T1)))
                    _dictionary.AddNewItem(val, default(T2));
            }
            else
                Debug.LogError("Type of T1 is not an enum");

        }
        catch (Exception e)
        {
            Debug.LogError("Adding to serializable dictionary failed because of " + e.Message);
        }
    }

}

[CustomPropertyDrawer(typeof(StringIntDictionary))]
public class StringIntDictionaryDrawer : SerializableDictionaryDrawer<string, int> { }
[CustomPropertyDrawer(typeof(StatusEffectIntDictionary))]
public class StatusEffectIntDictionaryDrawer : SerializableDictionaryDrawer<SEUtils.STATUS_EFFECTS, int> { }
[CustomPropertyDrawer(typeof(CharacterActionTypesIntDictionary))]
public class CharacterActionTypesIntDictionaryDrawer : SerializableDictionaryDrawer<SECharacters.CHARACTER_ACTION_TYPES, int> { }
[CustomPropertyDrawer(typeof(CharacterClassesIntDictionary))]
public class CharacterClassesIntDictionaryDrawer : SerializableDictionaryDrawer<SECharacters.CHARACTER_CLASSES, int> { }
[CustomPropertyDrawer(typeof(StatTypeIntDictionary))]
public class StatTypeIntDictionaryDrawer : SerializableDictionaryDrawer<SEUtils.STAT_TYPE, int> { }
[CustomPropertyDrawer(typeof(SkillTypeIntDictionary))]
public class SkillTypeIntDictionaryDrawer : SerializableDictionaryDrawer<SESkills.SkillTypes, int> { }
[CustomPropertyDrawer(typeof(StatusEffectModifierDictionary))]
public class StatusEffectModifierDictionaryDrawer : SerializableDictionaryDrawer<SEUtils.STATUS_EFFECTS, SEUtils.Modifier> { }


