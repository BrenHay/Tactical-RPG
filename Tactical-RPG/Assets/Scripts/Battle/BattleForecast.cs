using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleForecast : MonoBehaviour
{
    public TextMeshProUGUI aName, oName, aHealth, oHealth, aDmg, oDmg, aHit, oHit;
    private Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenForecast(GameObject initiator, GameObject enemy, int attackerHealth, int opponentHealth, int attackerDamage, int opponentDamage, int attackerHit, int opponentHit)
    {
        anim.SetBool("OpenMenu", true);
        aName.text = initiator.name;
        oName.text = enemy.name;
        aHealth.text = attackerHealth.ToString();
        oHealth.text = opponentHealth.ToString();
        aDmg.text = attackerDamage.ToString();
        oDmg.text = opponentDamage.ToString();
        aHit.text = attackerHit.ToString();
        oHit.text = opponentHit.ToString();
    }

    public void CloseForecast()
    {
        anim.SetBool("OpenMenu", false);
    }
}
