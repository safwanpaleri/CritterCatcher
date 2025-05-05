using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// From: https://www.youtube.com/watch?v=rgJGkYM2_6E&t=264s
public struct ProjectileProperties
{
    public Vector3 direction;
    public Vector3 initialPosition;
    public float initialSpeed;
    public float mass;
    public float drag;
}

[RequireComponent(typeof(LineRenderer))]
public class ProjectileTrajectoryPredictor : MonoBehaviour
{
    [SerializeField]
    private Vector3 startLineOffset = Vector3.zero;

    LineRenderer trajectoryLine;
    [SerializeField, Range(10, 100), Tooltip("The maximum number of points the LineRenderer can have")]
    int maxPoints = 50;
    [SerializeField, Range(0.01f, 0.5f), Tooltip("The time increment used to calculate the trajectory")]
    float increment = 0.025f;
    [SerializeField, Range(1.05f, 2f), Tooltip("The raycast overlap between points in the trajectory, this is a multiplier of the length between points. 2 = twice as long")]
    float rayOverlap = 1.1f;
    [SerializeField] 
    LayerMask layers;

    private void Start()
    {
        if (trajectoryLine == null)
            trajectoryLine = GetComponent<LineRenderer>();

        SetTrajectoryVisible(false);
    }

    public void PredictTrajectory(ProjectileProperties projectile)
    {
        Vector3 velocity = projectile.direction * (projectile.initialSpeed / projectile.mass);
        Vector3 position = Camera.main.transform.position + Camera.main.transform.forward + startLineOffset;
        Vector3 nextPosition;
        float overlap;

        SetTrajectoryVisible(true);
        UpdateLineRender(maxPoints, (0, position));

        for (int i = 1; i < maxPoints; i++)
        {
            // Estimate velocity and update next predicted position
            velocity = CalculateNewVelocity(velocity, projectile.drag, increment);
            nextPosition = position + velocity * increment;

            // Overlap our rays by small margin to ensure we never miss a surface
            overlap = Vector3.Distance(position, nextPosition) * rayOverlap;

            //When hitting a surface we want to show the surface marker and stop updating our line
            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap, layers))
            {
                UpdateLineRender(i, (i - 1, hit.point));
                break;
            }

            //If nothing is hit, continue rendering the arc without a visual marker
            position = nextPosition;
            UpdateLineRender(maxPoints, (i, position)); //Unneccesary to set count here, but not harmful
        }
    }
    /// <summary>
    /// Allows us to set line count and an induvidual position at the same time
    /// </summary>
    /// <param name="count">Number of points in our line</param>
    /// <param name="pointPos">The position of an induvidual point</param>
    private void UpdateLineRender(int count, (int point, Vector3 pos) pointPos)
    {
        trajectoryLine.positionCount = count;
        trajectoryLine.SetPosition(pointPos.point, pointPos.pos);
    }

    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
    {
        velocity += Physics.gravity * increment;
        velocity *= Mathf.Clamp01(1f - drag * increment);
        return velocity;
    }


    public void SetTrajectoryVisible(bool visible)
    {
        trajectoryLine.enabled = visible;
    }
}