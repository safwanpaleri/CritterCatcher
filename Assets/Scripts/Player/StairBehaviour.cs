using UnityEngine;

public class StairBehaviour : MonoBehaviour
{
    [SerializeField] private float playerHalfHeight = 1.0f;
    [SerializeField] private float distanceToFront = 0.25f;
    [SerializeField] private float stepHeight = 0.25f;

    [SerializeField] private Rigidbody rb;

    [Header("Debug Purposes")]
    [HideInInspector] private Vector3 dDirectionOfMovement = Vector3.forward;
    [SerializeField] private bool dDrawWires = true;

    public void TryClimbStairs(Vector3 directionOfMovement, bool isGrounded)
    {
        // Ray down forward, to see if the step is higher
        dDirectionOfMovement = directionOfMovement;

        if (CheckForHigherStep(directionOfMovement, isGrounded))
        {
            return;
        }
        //Ray down underneath to see if the step is lower
        if (CheckForLowerStep(directionOfMovement, isGrounded))
        {
            return;
        }

    }

    private bool CheckForHigherStep(Vector3 directionOfMovement, bool isGrounded)
    {
        Vector3 raypos = rb.transform.position + directionOfMovement * distanceToFront;
        Ray ray = new Ray(raypos, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, playerHalfHeight + stepHeight))
        {
            float feetHeight = rb.transform.position.y - playerHalfHeight;
            float heightDiff = hit.point.y - feetHeight;
            if (hit.normal == Vector3.up)
            {
                if (heightDiff < stepHeight && heightDiff > 0)
                {
                    ClimbStairsUp(hit.point.y);
                }
            }

            return true;
        }
        return false;
    }

    private bool CheckForLowerStep(Vector3 directionOfMovement, bool isGrounded)
    {
        Vector3 raypos = rb.transform.position;
        Ray ray = new Ray(raypos, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, playerHalfHeight + stepHeight))
        {
            float feetHeight = rb.transform.position.y - playerHalfHeight;
            float heightDiff = feetHeight - hit.point.y;
            if (hit.normal == Vector3.up)
            {
                if (heightDiff < stepHeight && heightDiff > 0)
                {
                    ClimbStairsDown(hit.point.y);
                }
            }
  
            return true;
        }
        return false;
    }
    private void ClimbStairsUp(float yPosition)
    {
        Vector3 targetVector = new Vector3(rb.position.x, yPosition + playerHalfHeight, rb.position.z);
        rb.position = Vector3.Lerp(rb.position, targetVector, Time.deltaTime / 0.1f);
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }

    private void ClimbStairsDown(float yPosition)
    {
        rb.position = new Vector3(rb.position.x, yPosition + playerHalfHeight, rb.position.z);
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }

    void OnDrawGizmos()
    {
        if (dDrawWires)
        {
            Gizmos.color = Color.yellow;
            // Downwards stair ray
            //Vector3 rayTop = transform.position + dDirectionOfMovement * distanceToFront;
            //Vector3 raybottom = Vector3.down * (playerHalfHeight + stepHeight);
            //Gizmos.DrawLine(rayTop, rayTop + raybottom);

            // Step boundary
            Vector3 stepBottom = rb.transform.position + (dDirectionOfMovement * (distanceToFront + 0.01f));
            stepBottom += Vector3.down * (playerHalfHeight + stepHeight);
            Vector3 stepTop = Vector3.up * (stepHeight * 2);
            Gizmos.DrawLine(stepBottom, stepBottom + stepTop);
        }
    }

}
