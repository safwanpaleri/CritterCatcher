using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakableshatters : MonoBehaviour
{
   
    [SerializeField] private List<Rigidbody> parts = new List<Rigidbody>();
    [SerializeField] private GameObject whole;
    [HideInInspector] public bool isBroken = false;
    [SerializeField] ParticleSystem glasseffect = null;

    public void BreakObject()
    {
        isBroken = true;
        whole.SetActive(false);
        GetComponent<SphereCollider>().enabled = false;
        glasseffect.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        BreakObject();
        Debug.Log("Vase collided with: " + collision.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Critter")
        {
            BreakObject();
            Debug.Log("Vase collided with: " + other.gameObject.name);
        }
    }
}

