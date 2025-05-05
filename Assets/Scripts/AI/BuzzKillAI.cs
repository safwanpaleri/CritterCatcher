using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuzzKillAI : CreatureAI
{
    //[SerializeField] public NavMeshAgent navMeshAgent;
    //[SerializeField] public Animator animator;
    //[SerializeField] public CharacterController characterController;
    //[SerializeField] public Transform playerBody;
    //[SerializeField] protected GameObject meshObject;

    [SerializeField][Range(0.0f, 10.0f)] private float speed = 1.0f;
    [SerializeField] private LayerMask objectsHitMask;

    //[HideInInspector] public CapsuleCollider capsuleCollider;
    //[HideInInspector] public Rigidbody rigidBody;

    private Vector3 initialPos;

    float angle = 0.0f;

    // Start is called before the first frame update
    public override void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        playerBody = FindFirstObjectByType<FirstPersonController>().GetPlayerBody();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rigidBody = GetComponent<Rigidbody>();
        initialPos = transform.position;
    }

    // Update is called once per frame
    public override void Update()
    {
        if(!isCaught && Time.timeScale != 0)
        {
            characterController.Move(transform.forward * (0.01f * speed));
            if (IsObjectInFront())
            {
                Debug.Log("Roam");
                Roam();
            }
        }
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Garden" && isThrown)
        {
            characterController.enabled = true;
            transform.localRotation = Quaternion.identity;
            meshObject.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
            isThrown = false;
            canCapture = true;
            isCaught = false;
            Roam();
            StopCoroutine(DelayReAlign());

        }
        if (isCaught)
            return;

    }
    public bool IsObjectInFront()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1.0f, objectsHitMask);

        if (hits.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public override void Roam()
    {
        float randomAngle = UnityEngine.Random.Range(140.0f, 220.0f);
        angle += randomAngle;

        transform.localRotation = Quaternion.Euler(0.0f, angle, 0.0f);
        transform.position = new Vector3(transform.position.x, initialPos.y, transform.position.z);
        animator.SetTrigger("Run");

    }

    public override void Thrown(Vector3 direction)
    {
        meshObject.transform.SetLocalPositionAndRotation(meshOriginalRelativePosition, meshOriginalRelativeOrientation);
        transform.localRotation = Quaternion.identity;
        meshObject.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        TurnOnThrowCollision();
        StartCoroutine(CanCaptureDelay());
    }

    public override void Captured()
    {
        base.Captured();
        characterController.enabled = false;
    }

    public override void EnableCollision()
    {
        rigidBody.useGravity = true;
        isThrown = true;
        isCaught = false;
    }

    protected override void TurnOffThrowCollision()
    {
        throwCollider.enabled = false;
        navMeshAgent.enabled = true;
        characterController.enabled = true;
        isThrown = false;
        canCapture = true;


        NavMeshHit hit;
        navMeshAgent.FindClosestEdge(out hit);
        transform.position = hit.position + Vector3.up * 2.0f;
        rigidBody.angularVelocity = Vector3.zero;
        rigidBody.velocity = Vector3.zero;

        Roam();

    }
}
