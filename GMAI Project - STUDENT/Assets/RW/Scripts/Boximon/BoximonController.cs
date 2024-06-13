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

    [SerializeField]
    Transform playerTransform;

    NavMeshAgent navAgent;

    [SerializeField]
    PandaBehaviour panda;

    Collider[] hitColliders;

    #endregion

    #region Properties

    bool attacked;
    bool canMoveRandomly;
    float newPathTime;
    float pathChangeDelayTime = 5f;
    Transform ChosenRoam;

    float proximity = 6f;
    bool playerDetected;

    float attackRange = 2f;
    bool isChasing;
    float distanceToPlayer;

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

    [Task]
    public void CheckForPlayerDetection()
    {
        if (playerDetected)
        {
            // Look towards player when detected.
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void CheckPlayerAttack()
    {
        if (attacked)
        {
            attacked = false;
            canMoveRandomly = false;
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    public void AttackedByPlayer() // This function is to be called in HitBox.cs.
    {
        attacked = true;
        //Debug.Log("You hit me!");
    }

    #endregion

    #region Attacking Tree

    [Task]
    public void ChasePlayer()
    {
        if (playerTransform != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > attackRange)
            {
                navAgent.SetDestination(playerTransform.position);
                Task.current.Succeed();
            }
            else
            {
                Task.current.Fail();
            }
        }
        else
        {
            Task.current.Fail();
        }
    }

    #endregion

    #region MonoBehaviour Callbacks
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();

        playerDetected = false;
        attacked = false;
        canMoveRandomly = true;
        isChasing = false;
    }

    // Update is called once per frame
    void Update()
    {
        hitColliders = Physics.OverlapSphere(transform.position, proximity);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform == playerTransform)
            {
                navAgent.isStopped = true;
                playerDetected = true;
            }
            else
            {
                navAgent.isStopped = false;
                playerDetected = false;
            }
        }
        CheckMovementAnim();
    }

    void CheckMovementAnim()
    {
        if (boximonAnim != null)
        {
            bool isMoving = navAgent.velocity.magnitude > 0.1f;
            boximonAnim.SetBool("Run Forward", isMoving);
        }
    }

    #endregion
}
