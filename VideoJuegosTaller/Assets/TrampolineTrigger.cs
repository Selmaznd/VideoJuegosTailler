using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineTrigger : MonoBehaviour
{
    public trampoline parentScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rear"))
        {
            parentScript.Trigger();
        }
    }
}

