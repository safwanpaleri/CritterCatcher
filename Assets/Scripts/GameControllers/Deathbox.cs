using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Deathbox : MonoBehaviour
{
    [SerializeField] private GameObject spawnPoint;
    private void OnCollisionEnter(Collision collision)
    {
        var rigidbody = collision.gameObject.GetComponent<Rigidbody>();
        var navmeshagent = collision.gameObject.GetComponent<NavMeshAgent>();
        var creatureAI = collision.gameObject.GetComponent<CreatureAI>();
        Debug.Log("COLLIDED: " + collision.gameObject.name);
        if (collision.gameObject.GetComponent<Rigidbody>() == null)
            Debug.Log("no rigidbody");
        rigidbody.velocity = Vector3.zero;
        navmeshagent.enabled = false;
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        collision.gameObject.transform.position = spawnPoint.transform.position;
        StartCoroutine(Spawn_Coroutine(rigidbody, navmeshagent, creatureAI));
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("COLLIDED: " + collision.gameObject.name);
        var rigidbody = collision.gameObject.GetComponent<Rigidbody>();
        var navmeshagent = collision.gameObject.GetComponent<NavMeshAgent>();
        var creatureAI = collision.gameObject.GetComponent<CreatureAI>();
        if (collision.gameObject.GetComponent<Rigidbody>() == null)
            Debug.Log("no rigidbody");
        rigidbody.velocity = Vector3.zero;
        navmeshagent.enabled = false;
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        collision.gameObject.transform.position = spawnPoint.transform.position;
        StartCoroutine(Spawn_Coroutine(rigidbody, navmeshagent, creatureAI));
       
    }

    private IEnumerator Spawn_Coroutine(Rigidbody rigidbody, NavMeshAgent navmeshagent, CreatureAI creatureAI)
    {
        yield return new WaitForSeconds(1.5f);
        navmeshagent.enabled = true;
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
        creatureAI.SwitchBehaviour();
    }
    
}
