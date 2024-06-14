using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI;
using UnityEngine.AI;

public class Paladin : MonoBehaviour
{
    #region Variables

    [HideInInspector]
    public NavMeshAgent paladinAgent;

    [HideInInspector]
    public Animator paladinAnim;

    [SerializeField]
    Transform playerTransform;

    #endregion

    #region Properties

    public float movementRadius = 10f;

    [HideInInspector]
    public bool roaming;

    #endregion

    #region States

    public PaladinStates m_Idle { get; set; } = null;

    public PaladinStates m_Current { get; set; } = null;

    #endregion

    #region Methods

    public Vector3 GetRandomRoamLocation() // Get a random location to roam to within a sphere.
    {
        Vector3 randomDir = Random.insideUnitSphere * movementRadius;
        randomDir += transform.position;
        NavMeshHit hit;
        Vector3 finalPos = Vector3.zero;
        if (NavMesh.SamplePosition(randomDir, out hit, movementRadius, 1))
        {
            finalPos = hit.position;
        }
        return finalPos;
    }

    #endregion

    #region MonoBehaviour Callbacks
    // Start is called before the first frame update
    void Start()
    {
        paladinAgent = GetComponent<NavMeshAgent>();

        m_Idle = new PaladinIdle(this);
        m_Current = m_Idle;
        m_Current.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        m_Current.Execute();

        CheckMovementAnim();
    }

    void CheckMovementAnim()
    {
        if (paladinAnim != null)
        {
            bool isMoving = paladinAgent.velocity.magnitude > 0.1f;
            paladinAnim.SetBool("Running", isMoving);
        }
    }

    #endregion
}
