using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Paladin paladin = GetComponent<Paladin>();
            if (paladin != null)
            {
                paladin.playerDetected = true;
                paladin.detectionZone.gameObject.SetActive(false);
            }
        }
    }
}
