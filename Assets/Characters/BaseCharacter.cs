using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SEUtils;

namespace SECharacters
{
    [CreateAssetMenu(fileName = "New BaseCharacterData", menuName = "ScriptableData/Base Character Data")]
    public class BaseCharacter : ScriptableObject
    {
        public int GetBaseHealth => GetBaseStats(STAT_TYPE.HEALTH);
        public int GetBaseStamina => GetBaseStats(STAT_TYPE.STAMINA);
        public int GetBaseStaminaRecovery => GetBaseStats(STAT_TYPE.STAMINA_RECOVERY);
        public int GetBaseDamage => GetBaseStats(STAT_TYPE.BASE_DAMAGE);
        public int GetBaseAttackSkill => GetBaseStats(STAT_TYPE.ATTACK_SKILL);
        public int GetBaseDefenseSkill => GetBaseStats(STAT_TYPE.DEFENSE_SKILL);
        public int GetBaseSpeed => GetBaseStats(STAT_TYPE.SPEED);
        public int GetBasePositiveVariance => GetBaseStats(STAT_TYPE.POSITIVE_VARIANCE);
        public int GetBaseNegativeVariance => GetBaseStats(STAT_TYPE.NEGATIVE_VARIANCE);
        public int GetBaseMinorActions => _baseActionsAvailable.GetDictionary[CHARACTER_ACTION_TYPES.MINOR_ACTION];
        public int GetBaseMajorActions => _baseActionsAvailable.GetDictionary[CHARACTER_ACTION_TYPES.MAJOR_ACTION];
        public BaseCharacterClass GetBaseCharacterClass => _baseCharacterClass;

        [SerializeField]
        private StatTypeIntDictionary _baseStats;
        [SerializeField]
        private StatusEffectIntDictionary _baseResistances;
        [SerializeField]
        private StatusEffectIntDictionary _baseRecovery;
        [SerializeField]
        private CharacterActionTypesIntDictionary _baseActionsAvailable;
        [SerializeField]
        private BaseCharacterClass _baseCharacterClass;


        private int GetBaseStats(STAT_TYPE type)
        {
            return _baseStats.GetDictionary[type];
        }


    }
}