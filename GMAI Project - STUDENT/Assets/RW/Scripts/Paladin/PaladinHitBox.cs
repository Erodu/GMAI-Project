using System.Collections;
using RayWenderlich.Unity.StatePatternInUnity;
using System.Collections.Generic;
using UnityEngine;

public class PaladinHitBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Character character = other.GetComponent<Character>();
            if (character != null)
            {
                character.PlayHurtAnim();
            }
        }
    }
}
