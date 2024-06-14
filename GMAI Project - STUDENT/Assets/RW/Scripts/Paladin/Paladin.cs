using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI;

public class Paladin : MonoBehaviour
{
    #region States

    public PaladinStates m_Idle { get; set; } = null;

    public PaladinStates m_Current { get; set; } = null;

    #endregion

    #region MonoBehaviour Callbacks
    // Start is called before the first frame update
    void Start()
    {
        m_Idle = new PaladinIdle(this);
        m_Current = m_Idle;
        m_Current.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        m_Current.Execute();
    }

    #endregion
}
