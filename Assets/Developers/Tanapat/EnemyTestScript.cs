using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestScript : MonoBehaviour, ICapturable
{
    [SerializeField] private float forceScale = 5f;
    public void Captured()
    {
        Debug.Log("Enemy Captured");
    }

    public void Thrown(Vector3 direction)
    {
        Debug.Log("Enemy Thrown");
    }

    public GameObject GetRoot()
    {
        return this.gameObject;
    }
    public Rigidbody GetRigidbody()
    {
        return GetComponent<Rigidbody>();
    }
    public void DisableCollision()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
    public void EnableCollision()
    {
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
    }
    public bool CanCapture()
    {
        return true;
    }
}
