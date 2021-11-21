using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SESkills;
using SECharacters;
using System;
using SEUtils;

[CreateAssetMenu(fileName = "Base PassiveEffect", menuName = "ScriptableData/Base Passive Effect")]
[Serializable]
public class BasePassiveEffect : BaseTriggerClass
{
    //[SerializeField]
    //protected string _passiveEffectName;

    [Header("Permanent")]
    [SerializeField]
    protected StatModifierDictionaryList _permStatModifierList;
    [Space(5)]
    [SerializeField]
    protected StatusEffectModifierDictionaryList _permStatusModifierList;

    Character _self;

    public BasePassiveEffect(Character self)
    {
        _self = self;
    }
 

    public virtual void ApplyPassiveStatChange(Character self)
    {

    }

    public override void OnAttack(Character self, Character target)
    {

    }

    public override void OnBasicAttack(Character self, List<Character> targets)
    {

    }

    public override void OnDamageDealt(Character self, Character target, int damageDealt)
    {

    }

    public override void OnEndOfRound(Character self, List<Type> types = null, List<System.Object> objects = null)
    {

    }

    public override void OnEndOfTurn(Character self, List<Type> types = null, List<System.Object> objects = null)
    {

    }

    public override void OnReceivedDamage(Character self, Character target, int damageReceived)
    {

    }

    public override void OnReceivingDamage(Character self, Character target, int damageReceived)
    {

    }

    public override void OnStartOfRound(Character self, List<Type> types = null, List<System.Object> objects = null)
    {

    }

    public override void OnStartOfTurn(Character self, List<Type> types = null, List<System.Object> objects = null)
    {

    }
}
