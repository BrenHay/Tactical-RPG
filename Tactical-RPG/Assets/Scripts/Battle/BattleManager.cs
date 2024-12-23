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
        if (damageToOpponent < 0) damageToOpponent = 0;

        return damageToOpponent;
    }

    public (int, int, int, int, int, int) ForecastDamage(GameObject attacker, GameObject opponent)
    {
        Unit attackerStats = attacker.GetComponent<Unit>();
        Unit opponentStats = opponent.GetComponent<Unit>();

        int targetDef = 0;
        int targetAttackerDef = 0;

        if (attackerStats.damageType == "physical")
        {
            targetDef = opponentStats.stats.Def;
        }
        else
        {
            targetDef = opponentStats.stats.Res;
        }

        if(opponentStats.damageType == "physical")
        {
            targetAttackerDef = attackerStats.stats.Def;
        }
        else
        {
            targetAttackerDef = attackerStats.stats.Res;
        }

        int damageToOpponent = Mathf.Clamp(attackerStats.stats.Atk - targetDef, 0, 100);
        int damageToAttacker = Mathf.Clamp(opponentStats.stats.Atk - targetAttackerDef, 0, 1000);

        int attackerHit = Mathf.Clamp(attackerStats.stats.Skill - opponentStats.stats.Eva, 0, 100);
        int opponentHit = Mathf.Clamp(opponentStats.stats.Skill - attackerStats.stats.Eva, 0, 100);


        if (opponentStats.stats.Range < Mathf.CeilToInt(Vector2Int.Distance(attackerStats.GetPostion(), opponentStats.GetPostion())) || 
            Mathf.CeilToInt(Vector2Int.Distance(attackerStats.GetPostion(), opponentStats.GetPostion())) == 1 && opponentStats.attackType == "ranged")
        {
            damageToAttacker = 0;
        }

        return (attackerStats.stats.currentHp, opponentStats.stats.currentHp, damageToOpponent, damageToAttacker, attackerHit, opponentHit);
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
        if (damageToOpponent < 0) damageToOpponent = 0;
        int damageToAttacker = opponentStats.stats.Atk - attackerDef;
        if (damageToAttacker < 0) damageToAttacker = 0;

        attack(attackerStats, opponentStats, damageToOpponent);

        // Checks if the opponent can counterattack
        if (opponentStats.stats.currentHp > 0 && opponentStats.stats.Range == attackerStats.stats.Range)
        {
            attack(opponentStats, attackerStats, damageToAttacker);
        }

        // Checks if attacker can double
        if(opponentStats.stats.currentHp > 0 && attackerStats.stats.Spd - opponentStats.stats.Spd >= 5 && attackerStats.stats.currentHp > 0)
        {
            attack(attackerStats, opponentStats, damageToOpponent);
        }
        // Checks to see if Opponent doubles
        else if(attackerStats.stats.currentHp > 0 && opponentStats.stats.Spd - attackerStats.stats.Spd >= 5 && opponentStats.stats.currentHp > 0 && opponentStats.stats.Range == attackerStats.stats.Range)
        {
            attack(opponentStats, attackerStats, damageToAttacker);
        }

        if (opponentStats.stats.currentHp <= 0)
        {
            if(opponent.tag == "Unit")
            {
                FindObjectOfType<TurnManager>().playerArmy.Remove(opponent);
            }
            else
            {
                opponent.GetComponent<EnemyAI>().Unhighlight();
                FindObjectOfType<TurnManager>().enemyArmy.Remove(opponent);
                FindObjectOfType<TurnManager>().HighlightEnemyRange();
            }
            Debug.Log("destroy");
            Destroy(opponent); 
        }
        if (attackerStats.stats.currentHp <= 0) 
        {
            Debug.Log("destroy attacker");
            if (attacker.tag == "Unit")
            {
                FindObjectOfType<TurnManager>().playerArmy.Remove(opponent);
            }
            else if(attacker.tag == "Enemy")
            {
                attacker.GetComponent<EnemyAI>().DefeatEnemy();
            }
            attacker.SetActive(false);
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
