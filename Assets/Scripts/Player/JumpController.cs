using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour
{

    [Header("Jump Parameters")]
    [HideInInspector] public bool readyToJump { get; private set; } = true;
    [SerializeField][Range(0f, 500f)] private float jumpForce = 5.0f;
    [SerializeField][Range(0f, 3f)] private float jumpCooldown = 0.1f; // Prevents multiple calls of jump while close to ground
    [SerializeField] private bool jumpEnabled = false;

    [Header("Ground Check")]
    [HideInInspector] public bool isGrounded { get; private set; }
    [SerializeField] private float bodyHeight = 2.0f;
    private const float groundRayExtension = 0.2f;

    [Header("Falling")]
    [SerializeField] private float extraGravity = 5.0f;
    [Header("References")]
    [SerializeField] private Transform bodyCenter;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask groundLayer;

    private void Update()
    {
        isGrounded = CheckIsGrounded();

        if (!isGrounded)
        {
            ExtraGravity();
        }
    }

    public void JumpPressed()
    {
        if (isGrounded && readyToJump  && jumpEnabled)
        {
            Jump();
            readyToJump = false;
            Invoke("EnableReadyToJump", jumpCooldown);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void EnableReadyToJump()
    {
        readyToJump = true;
    }


    private bool CheckIsGrounded()
    {
        return Physics.Raycast(bodyCenter.position, Vector3.down, bodyHeight * 0.5f + groundRayExtension, groundLayer);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(bodyCenter.position, Vector3.down * (bodyHeight * 0.5f + groundRayExtension));
    }
    private void ExtraGravity()
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y  - extraGravity * Time.deltaTime, rb.velocity.z);
    }
}
