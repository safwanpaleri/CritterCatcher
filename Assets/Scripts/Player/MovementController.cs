using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    enum MovementMode
    {
        Force,
        Velocity
    }
    [Header("Movement Type")]
    [SerializeField] private MovementMode movementType = MovementMode.Force;
    [Header("Movement Speeds")]
    [HideInInspector] public float externalMoveMultiplier = 1.0f; // Movement multiplier set by external factors/scripts
    [HideInInspector] public bool isSprinting { get; set; } = false;
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;
    [SerializeField] private float speedDampener = 0.4f;
    private const float movementForceMultiplier = 900.0f; // Flat amount to increase force calculations
    private const float movementVelocityMultiplier = 2.0f; // Flat amount to increase velocity movement
    private List<float> externalMovementMultipliers = new List<float>();
    private Dictionary<string, float> namedExternalMovementMultipliers = new Dictionary<string, float>();


    [Header("References")]
    [SerializeField] private Transform cameraBodyOrientator;
    [SerializeField] private Rigidbody rb;

    private Vector3 currentMovement;

    private void FixedUpdate()
    {
        switch (movementType)
        {
            case MovementMode.Force:
                Move();
                break;
            case MovementMode.Velocity:
                MoveVelocity();
                break;
            default:
                break;
        }
        if (currentMovement == Vector3.zero)
        {
            DampenMovement();
        }
        CalculateExternalMovementMultipliers();

    }
    public void AddNamedMovementMultipler(string name, float amount)
    {
        if(!namedExternalMovementMultipliers.ContainsKey(name))
        {
            namedExternalMovementMultipliers.Add(name, amount);
        }
    }
    public void RemoveNamedMovementMultipler(string name)
    {
        namedExternalMovementMultipliers.Remove(name);
    }

    public void AddMovementMultiplier(float multiplier, float duration)
    {
        Debug.Log("Movement Multiplied Call");
        externalMovementMultipliers.Add(multiplier);
        Invoke("RemoveExternalMovementMultiplier", duration);
    }
    public void MovePressed(Vector2 input)
    {
        // Save desired movement with input
        Vector3 worldDirection = CalculateWorldDirection(input);
        currentMovement = worldDirection * walkSpeed * (isSprinting ? sprintMultiplier : 1f);
    }
    public void DampenMovement(float dampenAmount)
    {
        // Slow movement per call for x,z axis, intended for air control
        Vector3 currentVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.velocity = new Vector3(currentVelocity.x * dampenAmount, rb.velocity.y, currentVelocity.z * dampenAmount);
    }
    private void CalculateExternalMovementMultipliers()
    {
        externalMoveMultiplier = 1.0f;

        foreach (var item in externalMovementMultipliers)
        {
            externalMoveMultiplier *= item;
        }
        foreach (var item in namedExternalMovementMultipliers)
        {
            externalMoveMultiplier *= item.Value;
        }
    }

    private void Move()
    {
        rb.AddForce(currentMovement * Time.deltaTime * movementForceMultiplier * externalMoveMultiplier, ForceMode.Force);
    }

    private void MoveVelocity()
    {
        rb.velocity = new Vector3 ( currentMovement.x * externalMoveMultiplier * movementVelocityMultiplier, rb.velocity.y, currentMovement.z * externalMoveMultiplier * movementVelocityMultiplier);
    }

    private void DampenMovement()
    {
        float dampenAmount = speedDampener * Time.deltaTime;
        rb.velocity = new Vector3(rb.velocity.x * dampenAmount, rb.velocity.y, rb.velocity.z * dampenAmount);
    }
    public Vector3 CalculateWorldDirection(Vector2 input)
    {
        Vector3 worldDirection = cameraBodyOrientator.forward * input.y + cameraBodyOrientator.right * input.x;
        return worldDirection.normalized;
    }

    [HideInInspector] private float maxSpeed = 10.0f;
    private void LimitMaxSpeed()
    {
        Vector3 currentVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // Set rigidbody velocity to the max speed if our current force velocity exceeds it
        if (currentVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = currentVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void OnDrawGizmos()
    {
        float length = 3.0f;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(cameraBodyOrientator.position, cameraBodyOrientator.forward * length);
        Gizmos.DrawRay(cameraBodyOrientator.position, cameraBodyOrientator.right * length);
    }
    private void RemoveExternalMovementMultiplier()
    {
        Debug.Log("Movement Multiplied Remove");
        if (externalMovementMultipliers.Count > 0)
        {
            externalMovementMultipliers.RemoveAt(0);
        }
    }
}
