using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinAttack : PaladinStates
{
    float attackRange = 1.5f;
    float distanceToPlayer;
    public PaladinAttack(Paladin m_Paladin)
    {
        paladin = m_Paladin;
    }

    public override void Enter()
    {
        paladin.paladinAgent.stoppingDistance = attackRange;
        paladin.HitByPlayer = false;
    }

    public override void Execute()
    {
        distanceToPlayer = Vector3.Distance(paladin.transform.position, paladin.playerTransform.position);
        Chase();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    void Chase()
    {
        if (paladin.playerTransform != null && !paladin.roaming)
        {
            if (distanceToPlayer > attackRange)
            {
                paladin.paladinAgent.isStopped = false;
                paladin.paladinAgent.SetDestination(paladin.playerTransform.position);
            }
            else
            {
                paladin.paladinAgent.isStopped = true;
            }
        }
    }

    void Attack()
    {
        if (distanceToPlayer <= attackRange)
        {
            paladin.StartCoroutine(PunchDelay());
            Punch();
        }
    }

    void Punch()
    {
        paladin.paladinAnim.SetTrigger("Punch");
    }

    void BlockHit()
    {
        paladin.paladinAnim.SetTrigger("Block");
    }

    void TakeHit()
    {
        paladin.paladinAnim.SetTrigger("Hit");
    }

    IEnumerator PunchDelay()
    {
        yield return new WaitForSeconds(7f);
    }
}
