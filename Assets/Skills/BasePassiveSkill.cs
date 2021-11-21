using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SEUtils;
using SECharacters;
using System;

[CreateAssetMenu(fileName = "New passive skill", menuName = "ScriptableData/Base Passive Skill")]
public class BasePassiveSkill : ScriptableObject
{
    [SerializeField]
    List<BasePassiveEffect> _passiveEffects;

    [SerializeField]
    BasePassiveEffect _passiveEffect;


    


    //[SerializeField]
    //protected float _overallDamageModifier;
    //[SerializeField]
    //protected StatTypeIntDictionary _statsModifier;
    //[SerializeField]
    //protected StatusEffectIntDictionary _resistanceModifier;
    //[SerializeField]
    //protected StatusEffectIntDictionary _recoveryModifier;

    //[SerializeField]
    //protected StatusEffectIntDictionary _statusEffectReceivedModifier;

    //[SerializeField]
    //protected StatusEffectIntDictionary _statusEffectInflictedModifier;

    //[SerializeField]
    //protected CharacterActionTypesIntDictionary _currentActionsModifier;


    //[SerializeField]
    //protected TriggerCondition _OnAttackCondition; 

    //Add commands (All commands should be in a scriptable Object)

    //Overriding skills
   
}
