using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI;
using Panda;
using UnityEngine.AI;
using JetBrains.Annotations;

[RequireComponent(typeof(NavMeshAgent))]
public class BoximonController : MonoBehaviour
{
    #region Variables

    [SerializeField]
    Animator boximonAnim;

    //[SerializeField]
    //Transform[] RoamSpots;

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

    public float hitPoints = 15;

    bool attacked;
    bool canMoveRandomly;
    float newPathTime;
    float pathChangeDelayTime = 5f;
    Transform ChosenRoam;
    float roamRadius = 8f;

    float proximity = 6f;
    bool playerDetected;

    float attackRange = 1.5f;
    float distanceToPlayer;

    [HideInInspector]
    public bool isDead;

    #endregion

    #region Roaming/Idle Tree

    [Task]
    public void RoamRandomly()
    {
        if (boximonAnim != null)
        {
            if (Time.time - newPathTime >= pathChangeDelayTime && canMoveRandomly)
            {
                if (!navAgent.pathPending && !navAgent.hasPath)
                {
                    Vector3 chosenPos = GetRandomRoamSpot();
                    navAgent.SetDestination(chosenPos);
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

    Vector3 GetRandomRoamSpot()
    {
        Vector3 randomDir = Random.insideUnitSphere * roamRadius;
        randomDir += transform.position;
        NavMeshHit hit;
        Vector3 finalPos = Vector3.zero;
        if (NavMesh.SamplePosition(randomDir, out hit, roamRadius, 1))
        {
            finalPos = hit.position;
        }
        return finalPos;
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

    public void AttackedByPlayer(float damage) // This function is to be called in HitBox.cs.
    {
        hitPoints -= damage;
        attacked = true;
        //Debug.Log($"You hit me! HP Left: {hitPoints}");
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
        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackRange)
        {
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
        boximonAnim.SetTrigger("Attack 01");
        hitBox.enabled = true;
        StartCoroutine(DelayHitBoxDeactivation());
    }

    IEnumerator DelayHitBoxDeactivation()
    {
        yield return new WaitForSeconds(0.25f);
        hitBox.enabled = false;
    }

    #endregion

    #region Check Death Tree

    [Task]
    public void CheckHealthState()
    {
        if (hitPoints <= 0)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
    }

    [Task]
    public void Death()
    {
        boximonAnim.SetTrigger("Die");
        navAgent.isStopped = true;
        isDead = true;
        StartCoroutine(DelayDespawn());
    }

    IEnumerator DelayDespawn()
    {
        yield return new WaitForSeconds(5);
        this.gameObject.SetActive(false);
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
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        hitColliders = Physics.OverlapSphere(transform.position, proximity);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform == playerTransform && !attacked)
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
