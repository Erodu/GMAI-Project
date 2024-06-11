using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI;
using Panda;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BoximonController : MonoBehaviour
{
    #region Variables

    [SerializeField]
    Animator boximonAnim;

    [SerializeField]
    Transform[] RoamSpots;

    NavMeshAgent navAgent;

    [SerializeField]
    PandaBehaviour panda;

    #endregion

    #region Properties

    bool attacked;
    bool canMoveRandomly;
    float newPathTime;
    float pathChangeDelayTime = 5f;
    Transform ChosenRoam;

    #endregion

    #region Roaming/Idle Tree

    [Task]
    public void RoamRandomly()
    {
        if (boximonAnim != null && RoamSpots.Length > 0)
        {
            if (Time.time - newPathTime >= pathChangeDelayTime && canMoveRandomly)
            {
                if (!navAgent.pathPending && !navAgent.hasPath)
                {
                    ChosenRoam = RoamSpots[Random.Range(0, RoamSpots.Length)];

                    navAgent.SetDestination(ChosenRoam.position);
                    newPathTime = Time.time;
                }
            }
        }
        else
        {
            Debug.Log("Check if the animator is referenced, or if the list of roam spots has elements.");
            Task.current.Fail();
        }
        Task.current.Succeed();
    }

    #endregion

    #region Check Player Tree



    #endregion

    #region MonoBehaviour Callbacks
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion
}
