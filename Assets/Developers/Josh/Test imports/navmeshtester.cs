using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class navmeshtester : MonoBehaviour
{
    public NavMeshAgent agent;

    void Start()
    {
       NavMeshAgent agent= GetComponent<NavMeshAgent>();
        marklocation();
    }

    void marklocation()
    {
        var randomlocation = new Vector3 (Random.Range(0,100), 0, Random.Range(0, 100));
        agent.destination = randomlocation;

    }

    private void Update()
    {
        if (agent.remainingDistance < 0.5f )
        {

            marklocation();
        }
    }






}
