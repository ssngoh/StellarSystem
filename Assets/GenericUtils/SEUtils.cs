using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace SEUtils
{

    [SerializeField]
    public enum STAT_TYPE
    {
        HEALTH,
        STAMINA,
        STAMINA_RECOVERY,
        BASE_DAMAGE,
        ATTACK_SKILL,
        DEFENSE_SKILL,
        SPEED,
        POSITIVE_VARIANCE,
        NEGATIVE_VARIANCE
    }

    [SerializeField]
    public enum STATUS_EFFECTS
    {
        ALL,
        BLINDED,
        POISONED,
        STUNNED,
        WEAKENED,
        LOCKED,
        SLOWED,
        INSOMNIA,
        FATIGUED
    }

    [SerializeField]
    public enum TARGET_TYPE
    {
        SELF,
        ALL,
        ALLIES_ONLY,
        ENEMIES_ONLY,
    }

    [SerializeField]
    public enum MODIFIER_TYPE
    {
        ABSOLUTE,
        PERCENTAGE
    }

    [SerializeField]
    public enum CONDITION_TYPE
    {
        OR,
        AND
    }

    [SerializeField]
    public enum COMPARISION_TYPE
    {
        LESSER,
        LESSER_OR_EQUAL,
        EQUAL,
        GREATER_OR_EQUAL,
        GREATER
    }

    [Serializable]
    public class TriggerConditions
    {

        [SerializeField]
        public StatModifierDictionaryList statConditionsToProc;

        [SerializeField]
        public StatusEffectModifierDictionaryList statusEffectConditionsToProc;

        [SerializeField]
        public List<BaseTriggerClass.TRIGGER_TYPE> triggersToProc;

        [SerializeField]
        public COMPARISION_TYPE comparisonType;

        [SerializeField]
        public CONDITION_TYPE conditionType; 

        //[SerializeField]
        //public int max_procs;

    }


    [Serializable]
    public struct Modifier
    {
        [SerializeField]
        public MODIFIER_TYPE mod_type;
        [SerializeField]
        public float amount;
    }


}