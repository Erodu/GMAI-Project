using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinDeath : PaladinStates
{
    public PaladinDeath(Paladin m_Paladin)
    {
        paladin = m_Paladin;
    }

    public override void Enter()
    {
        paladin.paladinAgent.isStopped = true;
        paladin.paladinAnim.SetTrigger("Die");
        paladin.StartCoroutine(DeathDelay());
    }

    public override void Execute()
    {
        //Debug.Log("The paladin has fallen...");
    }

    public override void Exit()
    {
        
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(5f);
        this.paladin.gameObject.SetActive(false);
    }
}
