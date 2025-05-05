using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class BuzzKIll : MonoBehaviour
{
    [SerializeField] public NavMeshAgent navMeshAgent;
    [SerializeField] public Animator animator;
    [SerializeField] public CharacterController characterController;
    [SerializeField] public Transform playerBody;
    [SerializeField] protected GameObject meshObject;

    [SerializeField][Range(0.0f, 10.0f)] private float speed = 1.0f;

    [HideInInspector] public CapsuleCollider capsuleCollider;
    [HideInInspector] public Rigidbody rigidBody;

    private Vector3 initialPos;

    float angle = 0.0f;

    // Start is called before the first frame update
    void Start()
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
    void Update()
    {
        characterController.Move(transform.forward * (0.01f * speed));

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag != "Garden" && collider.tag != "Critter")
        {
            Roam();
            Debug.Log("Roam Called: " + collider.gameObject.name);

        }
    }

    public void Roam()
    {
        float randomAngle = UnityEngine.Random.Range(140.0f, 220.0f);
        angle += randomAngle;

        transform.localRotation = Quaternion.Euler(0.0f, angle, 0.0f);
        transform.localPosition = new Vector3(transform.position.x, initialPos.y, transform.position.z);
        animator.SetTrigger("Run");

    }
}
