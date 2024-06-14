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
        paladin.HitByPlayer = false;
    }

    public override void Execute()
    {
        distanceToPlayer = Vector3.Distance(paladin.transform.position, paladin.playerTransform.position);
        Debug.Log($"DistanceToPlayer: {distanceToPlayer}, AttackRange: {attackRange}");
        Chase();
    }

    public override void Exit()
    {
        // Nothing yet
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
                Attack();
            }
        }
    }

    void Attack()
    {
        paladin.StartCoroutine(PunchDelay());
        Punch();
    }

    void Punch()
    {
        paladin.paladinAnim.SetTrigger("Punch");
        paladin.paladinHitBox.enabled = true;
        paladin.StartCoroutine(PaladinHitBoxDeactivationDelay());
        
    }

    IEnumerator PunchDelay()
    {
        yield return new WaitForSeconds(5f);
    }

    IEnumerator PaladinHitBoxDeactivationDelay()
    {
        yield return new WaitForSeconds(0.25f);
        paladin.paladinHitBox.enabled = false;
    }
}
