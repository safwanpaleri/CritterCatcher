using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IBreakable
{
    [Header("Object Parts")]
    [SerializeField] private List<Rigidbody> parts = new List<Rigidbody>();
    [SerializeField] private GameObject whole;
    [HideInInspector] public bool isBroken = false;
    [SerializeField][Range(1,100)] private int points = 1;
    [SerializeField] private ParticleSystem breakParticleEffect;
    public void BreakObject()
    {
        isBroken = true;
        whole.SetActive(false);
        GetComponent<SphereCollider>().enabled = false;
        foreach (var part in parts)
        {
            part.gameObject.SetActive(true);
            part.useGravity = true;
            part.isKinematic = false;
        }
        ParticleSystem go = Instantiate(breakParticleEffect, this.transform);
        go.Play();
        Destroy(go, 5.0f);
        BreakPoints.AddPoints(points);
        ExtraBreakEffects();
        //Add points
    }
    protected virtual void ExtraBreakEffects()
    {

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
            //Debug.Log("Vase collided with: " + other.gameObject.name);
        }
    }
}
