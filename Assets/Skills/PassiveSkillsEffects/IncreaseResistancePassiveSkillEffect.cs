using SECharacters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SEUtils;

[CreateAssetMenu(fileName = "New Increase Resistance Passive Skill Effect", menuName = "ScriptableData/Increase Resistance Passive Skill Effect")]
public class IncreaseResistancePassiveSkillEffect : BasePassiveEffect
{
    /* Passive SKill Effect: Increases resistances 
     * Exposed Variables
     *      Permanent resistance increase
     *      Conditional resistance increase
     *      
     * 
     * 
    // */
    [SerializeField]
    public TriggerGroupList _triggerGroup = new TriggerGroupList();

    //[SerializeField]
    //public List<TriggerConditions> _triggerConditions = new List<TriggerConditions>();

    //[SerializeField]
    //public TriggerConditions _triggerCondition;

    //[Header("Skill Specific")]
    //[SerializeField]
    //protected StatModifierDictionaryList _skillSpecificStatModifierList;
    //[Space(5)]
    //[SerializeField]
    //protected StatusEffectModifierDictionaryList _skillSpecificStatusModifierList;

    public IncreaseResistancePassiveSkillEffect(Character self) : base(self)
    {
    }

    public override void ApplyPassiveStatChange(Character self)
    {
       // self.mod

    }

    /*
     * TriggerCondition if none
     *   StatusEffectModifierDictionary _modifiers;
     *   
     *   
     * TriggerCondition if life 50 and below. Only triggers once
     *      +40 stamina recovery
     *      
     * TriggerCondition if life 50 and below. Only triggers once and lasts for 2 turns
     *      +40 stamina recovery
     *   
     * Deals damage back to attacker based on 20% of max health and ignore armor
     * 
     * 
     */
}
