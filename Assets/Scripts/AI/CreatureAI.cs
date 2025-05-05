using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class CreatureAI : MonoBehaviour, ICapturable
{
    [Header("Components")]
    [SerializeField] public NavMeshAgent navMeshAgent;
    [SerializeField] public Animator animator;
    [SerializeField] public CharacterController characterController;
    [SerializeField] public Transform playerBody;
    [SerializeField] protected GameObject meshObject;
    [SerializeField] protected BoxCollider throwCollider;
    protected Vector3 meshOriginalRelativePosition;
    protected Quaternion meshOriginalRelativeOrientation;


    [Header("Variables")]
    [SerializeField][Range(0f, 10f)] public float movementSpeed = 5.0f;
    [SerializeField] public float hidingDelay = 15.0f;
    [SerializeField] public float roamingDelay = 5.0f;
    [SerializeField] public float slideOff = 1.0f;
    [SerializeField] public float stunDelay = 2.0f;
    [SerializeField][Range(0f,3.0f)] protected float distanceFromObject = 2.0f;
    private float canCaptureDelay = 5.0f;

    [Header("Random Position")]
    [SerializeField] public List<GameObject> runPos = new List<GameObject>();
    [SerializeField] public List<GameObject> roamingPos = new List<GameObject>();
    [SerializeField] public List<GameObject> hidingPos = new List<GameObject>();

    [Header("Behaviour")]
    [SerializeField] public List<CreatureBehaviours> behaviours = new List<CreatureBehaviours>();

    [Header("Bools")]
    [HideInInspector]public bool isRoaming = false;
    [HideInInspector] public bool isRunning = false;
    [HideInInspector] public bool isHiding = false;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isThrown = false;
    [HideInInspector] public bool isCaught = false;
    protected bool canCapture = true;

    [Header("Cache")]
    [HideInInspector] public int currentBehaviourIndex = 0;
    [HideInInspector] public Vector3 selectedPos;
    [HideInInspector] public CapsuleCollider capsuleCollider;
    [HideInInspector] public Rigidbody rigidBody;

    public enum CreatureBehaviours
    {
        Roam,
        Run,
        Hide,
        Attack
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        if(navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();
        if(animator == null)
            animator = GetComponent<Animator>();
        if(characterController == null)
            characterController = GetComponent<CharacterController>();
        if(playerBody == null)
            playerBody = FindFirstObjectByType<FirstPersonController>().GetPlayerBody();
        if(capsuleCollider  == null)
            capsuleCollider = GetComponent<CapsuleCollider>();
        if(rigidBody == null)
            rigidBody = GetComponent<Rigidbody>();

        navMeshAgent.speed = movementSpeed;
        currentBehaviourIndex = -1;
        SwitchBehaviour();


        if (meshObject != null)
        {
            // Save the original displaced position of the mesh
            Quaternion orientation;
            Vector3 originalPos;
            meshObject.transform.GetLocalPositionAndRotation(out originalPos, out orientation);
            meshOriginalRelativePosition = originalPos;
            meshOriginalRelativeOrientation = orientation;
        }
        else
        {
            Debug.LogError("Need to set the body mesh for the critter");
        }

    }

    // Update is called once per frame
    public virtual void Update()
    {

        if (isRunning)
        {
            var val = Vector3.Distance(transform.position, selectedPos);
            if (val < distanceFromObject)
            {
                isRunning = false;
                animator.SetTrigger("Idle");

                if (!isHiding)
                    SwitchBehaviour();
                else
                    StartCoroutine(HideDelay());

            }
            isRoaming = false;
        }

        if (isAttacking)
        {
            if (playerBody == null)
                isAttacking = false;
            // Update target position
            transform.LookAt(playerBody.transform);
            var pos = playerBody.transform.position - new Vector3(2f, 0, 2f);
            navMeshAgent.SetDestination(playerBody.transform.position);

            var val = Vector3.Distance(transform.position, playerBody.transform.position);
            if (val < distanceFromObject)
            {
                AttackPlayer();
                isAttacking = false;
                isRoaming = false;
            }
        }

        if (isRoaming)
        {
            var val = Vector3.Distance(transform.position, selectedPos);
            if (val < distanceFromObject)
            {
                animator.SetTrigger("Idle");

                StartCoroutine(Roam_Coroutine());
                isRoaming = false;
            }
        }
    }

    public virtual void OnTriggerEnter(Collider collider)
    {
        if (isCaught)
        {
            return;
        }
        if (collider.tag == "Player")
        {
            Debug.Log("player entered");
            SwitchBehaviour();
        }

        if (collider.tag == "Garden" && isThrown)
        {
            throwCollider.enabled = false;
            animator.SetTrigger("Idle");
            characterController.enabled = true;
            navMeshAgent.enabled = true;
            isThrown = false;
            canCapture = true;
            StopCoroutine(CanCaptureDelay());
            StartCoroutine(SlideOff_Coroutine());
            StopCoroutine(DelayReAlign());
        }
    }

    public virtual void OnTriggerExit(Collider collider)
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Critter2") || collision.collider.CompareTag("Critter") || collision.collider.CompareTag("Player"))
            return;
        Debug.Log("Hit Throwing");
        StartCoroutine(DelayReAlign());
    }

    protected IEnumerator DelayReAlign()
    {
        yield return new WaitForSeconds(1.0f);

        if (isThrown)
        {
            TurnOffThrowCollision();
        }
    }
    public void SwitchBehaviour()
    {
        if (behaviours.Count == 0) return;

        if (currentBehaviourIndex + 1 <= behaviours.Count - 1)
            ++currentBehaviourIndex;
        else
            currentBehaviourIndex = 0;

        var behaviour = behaviours[currentBehaviourIndex];

        ResetBools();

        switch (behaviour)
        {
            case CreatureBehaviours.Roam:
                Roam();
                break;
            case CreatureBehaviours.Run:
                Run();
                break;
            case CreatureBehaviours.Attack:
                Attack();
                break;
            case CreatureBehaviours.Hide:
                Hide();
                break;
            default:
                Roam();
                break;
        }
    }

    public virtual void Run()
    {
        if (runPos.Count < 1 || isCaught)
            return;
        animator.SetTrigger("Run");
        var random = Random.Range(0, runPos.Count);
        selectedPos = runPos[random].transform.position;
        transform.LookAt(selectedPos);
        navMeshAgent.SetDestination(selectedPos);
        isRunning = true;
    }

    public virtual void Roam()
    {
        if (roamingPos.Count == 0 || isCaught)
            return;
        var random = Random.Range(0, roamingPos.Count - 1);
        var roamPos = roamingPos[random];
        transform.LookAt(roamPos.transform);
        navMeshAgent.SetDestination(roamPos.transform.position);
        animator.SetTrigger("Run");
        selectedPos = roamPos.transform.position;
        isRoaming = true;

    }

    public virtual IEnumerator Roam_Coroutine()
    {

        yield return new WaitForSeconds(roamingDelay);
        Roam();
    }

    public void Attack()
    {
        // To be done
        
        if (playerBody == null)
            return;
        Debug.Log(playerBody.transform.position);
        transform.LookAt(playerBody.transform);
        navMeshAgent.SetDestination(playerBody.transform.position);
        isAttacking = true;
        animator.SetTrigger("Run");
    }

    public void AttackPlayer()
    {
        //do attack
        animator.SetTrigger("Attack");
        Debug.Log("Attacked Player");
        playerBody.GetComponentInParent<FirstPersonController>().StunPlayer(stunDelay);
        SwitchBehaviour();
    }

    public void Hide()
    {
        if (hidingPos.Count < 1)
            return;
        var random = Random.Range(0, hidingPos.Count);
        selectedPos = hidingPos[random].transform.position;
        transform.LookAt(selectedPos);
        if(navMeshAgent.isOnNavMesh)
            navMeshAgent.SetDestination(selectedPos);
        animator.SetTrigger("Run");
        isRunning = true;
        isHiding = true;
    }

    public IEnumerator HideDelay()
    {
        yield return new WaitForSeconds(hidingDelay);
        SwitchBehaviour();
    }

    public void ResetBools()
    {
        isRoaming = false;
        isRunning = false;
        isHiding = false;
        isAttacking = false;
    }


    public virtual void Captured()
    {
        // Stops any hover, or move to coroutines and snaps mesh object to the hand
        StopAllCoroutines();
        meshObject.transform.SetPositionAndRotation(this.transform.position, meshObject.transform.rotation);
        canCapture = false;
    }

    public virtual void Thrown(Vector3 direction)
    {
        // Sets the meshobject back to the original displacement so it looks like it hovers correctly
        animator.SetTrigger("fall");
        meshObject.transform.SetLocalPositionAndRotation(meshOriginalRelativePosition, meshOriginalRelativeOrientation);
        TurnOnThrowCollision();
        StartCoroutine(CanCaptureDelay());
    }
    public virtual bool CanCapture()
    {
        return canCapture;
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
        ResetBools();
        animator.SetTrigger("Idle"); // add caught animation
        rigidBody.useGravity = false;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        animator.StopPlayback();
        characterController.enabled = false;
        navMeshAgent.enabled = false;
        isCaught = true;

    }
    public virtual void EnableCollision()
    {
        rigidBody.useGravity = true;
        isThrown = true;
        characterController.enabled = true;
        isCaught = false;
        animator.SetTrigger("Idle");
    }

    public IEnumerator SlideOff_Coroutine()
    {
        yield return new WaitForSeconds(slideOff);
        navMeshAgent.enabled = true;
        rigidBody.isKinematic = true;
        rigidBody.isKinematic = false;
        currentBehaviourIndex = 0;
        SwitchBehaviour();
        Debug.Log("Called slide off");
    }

    protected IEnumerator CanCaptureDelay()
    {
        // A fallback in case ground is never hit
        yield return new WaitForSeconds(canCaptureDelay);
        canCapture = true;
    }

    protected void TurnOnThrowCollision()
    {
        throwCollider.enabled = true;
    }

    protected virtual void TurnOffThrowCollision()
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

        StopCoroutine(CanCaptureDelay());
        StartCoroutine(SlideOff_Coroutine());
        animator.SetTrigger("Idle");
    }
}
