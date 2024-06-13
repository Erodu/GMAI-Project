using RayWenderlich.Unity.StatePatternInUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoximonHitBox : MonoBehaviour
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
