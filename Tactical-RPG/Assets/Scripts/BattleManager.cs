using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //public GameObject attacker;
    //public GameObject opponent;

    // Update is called once per frame
    void Update()
    {
        
    }

    void CalculateDamage(GameObject attacker, GameObject opponent)
    {
        Unit attackerStats = attacker.GetComponent<Unit>();
        Unit opponentStats = opponent.GetComponent<Unit>();

        int damageToOpponent = attackerStats.stats.Atk - opponentStats.stats.Def;
        int damageToAttacker = opponentStats.stats.Atk - attackerStats.stats.Def;
    }

    public int SimulateDamage(GameObject attacker, GameObject opponent)
    {
        Unit attackerStats = attacker.GetComponent<Unit>();
        Unit opponentStats = opponent.GetComponent<Unit>();

        int targetDef = 0;

        if (attackerStats.damageType == "physical")
        {
            targetDef = opponentStats.stats.Def;
        }
        else
        {
            targetDef = opponentStats.stats.Res;
        }

        int damageToOpponent = attackerStats.stats.Atk - targetDef;

        return damageToOpponent;
    }

    public void Battle(GameObject attacker, GameObject opponent)
    {
        Unit attackerStats = attacker.GetComponent<Unit>();
        Unit opponentStats = opponent.GetComponent<Unit>();

        int targetDef = 0;
        int attackerDef = 0;

        if(attackerStats.damageType == "physical")
        {
            targetDef = opponentStats.stats.Def;
        }
        else
        {
            targetDef = opponentStats.stats.Res;
        }

        if(opponentStats.damageType == "physical")
        {
            attackerDef = attackerStats.stats.Def;
        }
        else
        {
            attackerDef = attackerStats.stats.Res;
        }

        int damageToOpponent = attackerStats.stats.Atk - targetDef;
        int damageToAttacker = opponentStats.stats.Atk - attackerDef;

        attack(attackerStats, opponentStats, damageToOpponent);

        if (opponentStats.stats.currentHp > 0 && opponentStats.stats.Range == attackerStats.stats.Range)
        {
            attack(opponentStats, attackerStats, damageToAttacker);
        }
        if(attackerStats.stats.Spd - opponentStats.stats.Spd >= 5 && attackerStats.stats.currentHp > 0)
        {
            attack(attackerStats, opponentStats, damageToOpponent);
        }
        // Checks to see if Opponent doubles
        else if(opponentStats.stats.Spd - attackerStats.stats.Spd >= 5 && opponentStats.stats.currentHp > 0)
        {
            attack(opponentStats, attackerStats, damageToAttacker);
        }
    }

    void attack(Unit attacker, Unit recipient, int damage)
    {
        if(Random.Range(1, 101) > recipient.stats.Eva - attacker.stats.Skill)
        {
            recipient.stats.currentHp -= damage;
            Debug.Log("Hit!");
        }
        else
        {
            Debug.Log("Miss!");
        }
    }
}
