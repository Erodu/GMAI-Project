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

    [SerializeField]
    BoxCollider hitBox;

    #endregion

    #region Properties

    bool attacked;
    bool canMoveRandomly;
    float newPathTime;
    float pathChangeDelayTime = 5f;
    Transform ChosenRoam;

    float proximity = 6f;
    bool playerDetected;

    float attackRange = 1.5f;
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
        if (playerTransform != null && attacked)
        {
            navAgent.stoppingDistance = attackRange;
            //Debug.Log($"Distance to Player: {distanceToPlayer}, Attack Range: {attackRange}");
            if (distanceToPlayer > attackRange)
            {
                navAgent.isStopped = false;
                navAgent.SetDestination(playerTransform.position);
                Task.current.Succeed();
            }
            else
            {
                navAgent.isStopped = true;
                Task.current.Fail();
            }
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void AttackPlayer()
    {
        if (distanceToPlayer <= attackRange)
        {
            Debug.Log("Attacking");
            Attack();
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    void Attack()
    {
        boximonAnim.SetTrigger("Attack 02");
        hitBox.enabled = true;
        StartCoroutine(DelayHitBoxDeactivation());
    }

    IEnumerator DelayHitBoxDeactivation()
    {
        yield return new WaitForSeconds(0.25f);
        boximonAnim.ResetTrigger("Attack 02");
        hitBox.enabled = false;
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
        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
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
