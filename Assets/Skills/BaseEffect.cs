using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SECharacters;

namespace SESkills
{
    [CreateAssetMenu(fileName = "New BaseEffect", menuName = "ScriptableData/Base Effect")]
    public abstract class BaseEffect : ScriptableObject
    {

        //Not sure if target type and max targets should be inside the base Effect or not
        /*eg,
         * Heals 20 to self
         * Deals 20 to all enemies
         * 3 allies get +20 attack skill for 1 turn
         * 
         */

        [SerializeField]
        protected string _effectName;

        [SerializeField]
        public List<Character> _targets;
        [SerializeField]
        protected Character _source;
        [SerializeField]
        protected SEUtils.TARGET_TYPE _targetType;


        public abstract void ApplyEffect(List<Character> targets);
    }
}