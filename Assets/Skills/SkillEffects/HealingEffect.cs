using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SECharacters;

namespace SESkills
{
    [CreateAssetMenu(fileName = "New Healing Effect", menuName = "ScriptableData/Healing Effect")]
    public class HealingEffect : BaseEffect
    {

        [SerializeField]
        StatTypeIntDictionary healingEffect;


        public override void ApplyEffect(List<Character> targets)
        {
            //for (int i = 0; i < targets.Count; ++i)
            //    targets[i].ModifyCurrentHealth(amount);
        }
    }
}