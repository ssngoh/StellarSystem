using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SECharacters
{
    [CreateAssetMenu(fileName = "New BaseCharacterClass", menuName = "ScriptableData/Base Character Class")]
    public class BaseCharacterClass : ScriptableObject
    {
        public CHARACTER_CLASSES GetCharacterClass => _mainClass;
        public Dictionary<CHARACTER_CLASSES, int> GetSubClasses => _subClasses.GetDictionary;

        [SerializeField]
        private CHARACTER_CLASSES _mainClass;
        [SerializeField]
        private CharacterClassesIntDictionary _subClasses; //Key is which class, value is treated as penalty/bonus

    }
}