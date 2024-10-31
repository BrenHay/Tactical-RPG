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

    void Battle(GameObject attacker, GameObject opponent, Stats attackerStats, Stats opponentStats)
    {
        
    }
}
