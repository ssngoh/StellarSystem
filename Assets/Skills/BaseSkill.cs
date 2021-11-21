using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SESkills;
using SECharacters;

[CreateAssetMenu(fileName = "New BaseSkill", menuName = "ScriptableData/Base Skill")]
public class BaseSkill : ScriptableObject 
{
    //A skill is basically a chain of skill effects instead of the effects coded directly into the skill itself
    [SerializeField]
    List<BaseEffect> _effects;
    [SerializeField]
    List<BaseStatusEffect> _statusEffects;

    [SerializeField]
    SkillTypeIntDictionary _skillTypes;









}
