using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinIdle : PaladinStates
{
    float newPathTime;
    float pathChangeDelayTime = 5f;
    public PaladinIdle(Paladin m_Paladin)
    {
        paladin = m_Paladin;
    }

    public override void Enter()
    {
        paladin.roaming = true;
    }

    public override void Execute()
    {
        Roam();
        if (paladin.playerDetected)
        {
            // Transition to Attack State
        }
    }

    public override void Exit()
    {
        paladin.roaming = false;
    }

    void Roam()
    {
        if (paladin.roaming && paladin.paladinAnim != null)
        {
            if (Time.time - newPathTime >= pathChangeDelayTime)
            {
                if (!paladin.paladinAgent.pathPending && !paladin.paladinAgent.hasPath)
                {
                    Vector3 chosenPos = paladin.GetRandomRoamLocation();
                    paladin.paladinAgent.SetDestination(chosenPos);
                    newPathTime = Time.time;
                }
            }
        }
    }
}
