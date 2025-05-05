using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositPoint : MonoBehaviour
{
    private Collider collider;
    [SerializeField] private CritterCaught updateCritterScript;
    [SerializeField] private string critterTag;

    private void Start()
    {
        collider = GetComponent<Collider>();
    }

    private void CaptureCritter(GameObject critter)
    {
        if (critter != null)
        {
            if (FindFirstObjectByType<ActionsController>().GetGrabbedObject() != critter.GetComponent<ICapturable>())
            {
                Destroy(critter); //TODO: Doesn't seeem to destroy the critter
                if (updateCritterScript != null)
                {
                    updateCritterScript.decrementCounter();
                }
            }

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == critterTag || other.tag == "Critter2")
        {
            CaptureCritter(other.gameObject);
        }
    }
}
