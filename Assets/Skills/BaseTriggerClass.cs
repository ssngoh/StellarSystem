using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SEUtils;
using SECharacters;
using System;

public abstract class BaseTriggerClass : ScriptableObject
{
    //If we add a new trigger type, we need to add an abstract function of that type
    [SerializeField]
    public enum TRIGGER_TYPE
    {
        NONE,
        ON_BASIC_ATTACK,
        ON_SKILL_ATTACK,
        ON_ALL_ATTACK,
        ON_DAMAGE_DEALT,

        ON_RECEIVING_DAMAGE,
        ON_RECEIVED_DAMAGE,

        ON_START_OF_TURN,
        ON_END_OF_TURN,
        ON_START_OF_ROUND,
        ON_END_OF_ROUND,


        //NOT IN USE YET
        ON_COMMAND_USED,
        ON_WAIT_USED,
        ON_WAIT
    }

    [SerializeField]
    public enum TRIGGER_CONDITION_TARGET_TYPE
    { 
        SELF,

        ALLY_ON_RECEIVING_DAMAGE,
        ALLY_ON_RECEIVED_DAMAGE,

        ENEMY_ON_RECEIVING_DAMAGE,
        ENEMY_ON_RECEIVED_DAMAGE,

        ON_START_OF_ENEMY_TURN,
        ON_END_OF_ENEMY_TURN,
    }



    //Attack and defend 
    public abstract void OnBasicAttack(Character self, List<Character> targets); //Maybe need to put in attack modifiers?
    public abstract void OnAttack(Character self, Character target); //Maybe need to put in attack modifiers and attack type?
    public abstract void OnDamageDealt(Character self, Character target, int damageDealt);

    public abstract void OnReceivingDamage(Character self, Character target, int damageReceived);
    public abstract void OnReceivedDamage(Character self, Character target, int damageReceived);

    //Turn sequences
    public abstract void OnStartOfTurn(Character self, List<System.Type> types = null, List<System.Object> objects = null);
    public abstract void OnEndOfTurn(Character self, List<System.Type> types = null, List<System.Object> objects = null);
    public abstract void OnStartOfRound(Character self, List<System.Type> types = null, List<System.Object> objects = null);
    public abstract void OnEndOfRound(Character self, List<System.Type> types = null, List<System.Object> objects = null);


}
