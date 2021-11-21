using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SEUtils;
using System;

namespace SECharacters
{
    public class Character
    {
        private BaseCharacter _character;


        public int GetCurrentHealth => _currentStats.GetDictionary[STAT_TYPE.HEALTH];
        public int GetCurrentStamina => _currentStats.GetDictionary[STAT_TYPE.STAMINA];
        public int GetCurrentMinorActionsAvailable => _currentActionsAvailable.GetDictionary[CHARACTER_ACTION_TYPES.MINOR_ACTION];
        public int GetCurrentMajorActionsAvailable => _currentActionsAvailable.GetDictionary[CHARACTER_ACTION_TYPES.MAJOR_ACTION];

        public int GetMaxHealth => _character.GetBaseHealth + _passiveStatModifiers.GetDictionary[STAT_TYPE.HEALTH]; // + active mods
        public int GetMaxStamina => _character.GetBaseStamina + _passiveStatModifiers.GetDictionary[STAT_TYPE.STAMINA]; // + active mods
        public int GetMaxMinorActionsAvailable => _character.GetBaseMinorActions; // + passive + active mods
        public int GetMaxMajorActionsAvailable => _character.GetBaseMajorActions; // + passive  active mods

        public int GetCurrentStaminaRecovery => GetCurrentStats(STAT_TYPE.STAMINA_RECOVERY);
        public int GetCurrentDamage => GetCurrentStats(STAT_TYPE.BASE_DAMAGE);
        public int GetCurrentAttackSkill => GetCurrentStats(STAT_TYPE.ATTACK_SKILL);
        public int GetCurrentDefenseSkill => GetCurrentStats(STAT_TYPE.DEFENSE_SKILL);
        public int GetCurrentSpeed => GetCurrentStats(STAT_TYPE.SPEED);
        public int GetCurrentPositiveVariance => GetCurrentStats(STAT_TYPE.POSITIVE_VARIANCE);
        public int GetCurrentNegativeVariance => GetCurrentStats(STAT_TYPE.NEGATIVE_VARIANCE);
 
        public Dictionary<STATUS_EFFECTS, int> GetAllCurrentStatusInflicited => _currentStatusEffectInflicted.GetDictionary;
        public Dictionary<STATUS_EFFECTS, int> GetAllCurrentResistances => GetAllCurrentResistancesDic();
        public Dictionary<STATUS_EFFECTS, int> GetAllCurrentRecovery => GetAllCurrentRecoveryDic();


        //Keeps track of current status of the character
        private StatTypeIntDictionary _currentStats;
        private StatusEffectIntDictionary _currentBaseResistances;
        private StatusEffectIntDictionary _currentBaseRecovery;
        private StatusEffectIntDictionary _currentStatusEffectInflicted;
        private CharacterActionTypesIntDictionary _currentActionsAvailable;

        //Passive modifiers
        private StatTypeIntDictionary _passiveStatModifiers;
        private StatusEffectIntDictionary _passiveResistanceModifiers;
        private StatusEffectIntDictionary _passiveRecoveryModifiers;
        private List<BasePassiveSkill> _passiveSkillList = new List<BasePassiveSkill>();

        //Active modifers


        public Character(BaseCharacter character)
        {
            _character = character;
        }

        public void ApplyStatModifiers(int amount, SEUtils.STAT_TYPE type)
        {
            switch(type)
            {
                case SEUtils.STAT_TYPE.HEALTH:
                    
                    break;
                default:
                    break;
            }
        }

        private int GetCurrentStats(STAT_TYPE type)
        {
            return _currentStats.GetDictionary[type] + _passiveStatModifiers.GetDictionary[type]; // + active modifiers
        }

        public int GetCurrentResistance(STATUS_EFFECTS effectType)
        {
            return _currentBaseResistances.GetDictionary[effectType] + _passiveResistanceModifiers.GetDictionary[effectType]; // + active modifiers
        }

        public int GetCurrentStatusRecovery(STATUS_EFFECTS effectType)
        {
            return _currentBaseRecovery.GetDictionary[effectType] + _passiveRecoveryModifiers.GetDictionary[effectType]; // + active modifiers
        }

        public int GetCurrentStatusInflicited(STATUS_EFFECTS effectType)
        {
            return _currentStatusEffectInflicted.GetDictionary[effectType]; // + active modifiers
        }

        Dictionary<STATUS_EFFECTS, int> GetAllCurrentResistancesDic()
        {
            Dictionary<STATUS_EFFECTS, int> tempDic = new Dictionary<STATUS_EFFECTS, int>();
            foreach (STATUS_EFFECTS statusEffect in Enum.GetValues(typeof(STATUS_EFFECTS)))
                tempDic.Add(statusEffect, GetCurrentResistance(statusEffect));
            return tempDic;
        }

        Dictionary<STATUS_EFFECTS, int> GetAllCurrentRecoveryDic()
        {
            Dictionary<STATUS_EFFECTS, int> tempDic = new Dictionary<STATUS_EFFECTS, int>();
            foreach (STATUS_EFFECTS statusEffect in Enum.GetValues(typeof(STATUS_EFFECTS)))
                tempDic.Add(statusEffect, GetCurrentStatusRecovery(statusEffect));
            return tempDic;
        }

        //Interaction with skills

        public void ModifyCurrentHealth(int amount, bool canGoAboveMaxHealth = false)
        {
            if (canGoAboveMaxHealth)
                _currentStats.GetDictionary[STAT_TYPE.HEALTH] += amount;
            else
                _currentStats.GetDictionary[STAT_TYPE.HEALTH] = Mathf.Max(GetCurrentHealth + amount, GetMaxHealth);
        }



    }
}