using UnityEngine;

public class ActionsController : MonoBehaviour
{
    [SerializeField] private AudioSource m_AudioSource;


    [Header("Swinging")]
    [SerializeField] private float swingCooldown = 0.75f;
    [SerializeField] private float swingDistanceInFront = 1.25f;
    [SerializeField] private float swingRadius = 1f;
    private bool readyToSwing = true;

    [Header("Grabbing")]
    [SerializeField] private float grabRange = 2.0f;
    [SerializeField] private float grabCooldown = 0.25f;
    private bool readyToGrab = true;

    [Header("Throwing")]
    [SerializeField] private float throwForce = 5f;

    [Header("References")]
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private LayerMask throwMask;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform capturePoint; //Possibly will be removed in future with animations
    [SerializeField] private ProjectileTrajectoryPredictor trajectory;
    private ICapturable grabbedObject;

    public ICapturable GetGrabbedObject()
    {
        return grabbedObject;
    }
    public void SwingPressed()
    {
        if (readyToSwing)
        {
            readyToSwing = false;

            grabbedObject = Swing();
            if (grabbedObject != null)
            {
                AttachCapturedObject();
                grabbedObject.Captured();
            }

            Invoke("SetReadyToSwingToTrue", swingCooldown);
        }
    }
    public void GrabPressed()
    {
        if (readyToGrab)
        {
            readyToGrab = false;

            grabbedObject = Grab();
            if (grabbedObject != null)
            {
                AttachCapturedObject();
                grabbedObject.Captured();
            }

            Invoke("SetReadyToGrabToTrue", grabCooldown);
        }
    }
    public bool PredictThrow()
    {
        if (grabbedObject != null)
        {
            ProjectileProperties property;
            property.initialPosition = grabbedObject.GetRoot().transform.position;
            property.initialSpeed = throwForce;
            property.direction = cameraTransform.forward;
            property.drag = grabbedObject.GetRigidbody().drag;
            property.mass = grabbedObject.GetRigidbody().mass;
            trajectory.PredictTrajectory(property);
            return true;
        }
        trajectory.SetTrajectoryVisible(false);
        return false;
    }
    public bool ThrowObjectIfHeld()
    {
        if (grabbedObject != null)
        {
            if(IsFrontClear())
            {
                ICapturable obj = DetachCapturedObject();
                obj.GetRigidbody().AddForce(cameraTransform.forward * throwForce, ForceMode.Impulse);
                obj.Thrown(cameraTransform.forward);
                trajectory.SetTrajectoryVisible(false);
                return true;
            }
            else
            {
                Debug.Log("Front is not clear for throwing");
            }

        }
        return false;
    }
    public bool IsObjectHeld()
    {
        return grabbedObject != null;
    }
    private ICapturable Swing()
    {
        m_AudioSource.Play();
        // Spherecast in front of controller and break any objects hit, and capture only 1 creature if not already holding creature
        RaycastHit[] hits = Physics.SphereCastAll(new Ray(cameraTransform.transform.position, cameraTransform.transform.forward * swingDistanceInFront), swingRadius, 1f, hitMask);

        BreakObjectsHit(ref hits);
        ICapturable capturedObject = GetFirstCreatureHit(ref hits);

        return capturedObject;
    }

    private ICapturable Grab()
    {
        RaycastHit hit; 
        Physics.Raycast(new Ray(cameraTransform.transform.position, cameraTransform.transform.forward),out hit, grabRange,hitMask);

        // Grab can only capture
        if (hit.collider != null)
        {
            ICapturable capturedObject = null;
            hit.collider.TryGetComponent<ICapturable>(out capturedObject);
            if (capturedObject != null)
            {
                if (capturedObject.CanCapture())
                {
                    return capturedObject;
                }
            }
        }
        return null;
    }
    private void AttachCapturedObject()
    {
        grabbedObject.GetRoot().transform.position = capturePoint.gameObject.transform.position;
        grabbedObject.GetRoot().transform.parent = capturePoint.gameObject.transform;
        grabbedObject.GetRoot().transform.rotation = Quaternion.identity;
        grabbedObject.DisableCollision();
    }
    public ICapturable DetachCapturedObject()
    {
        ICapturable obj = grabbedObject;
        if (obj != null)
        {
            grabbedObject.GetRoot().transform.parent = null;
            grabbedObject.EnableCollision();
            grabbedObject = null;
        }
        return obj;

    }
    private void BreakObjectsHit(ref RaycastHit[] hits)
    {
        foreach (RaycastHit hit in hits)
        {
            IBreakable breakable = null;
            hit.collider.TryGetComponent<IBreakable>(out breakable);
            if (breakable != null)
            {
                breakable.BreakObject();
            }
        }
    }

    private ICapturable GetFirstCreatureHit(ref RaycastHit[] hits)
    {
        foreach (RaycastHit hit in hits)
        {
            ICapturable objectHit = null;
            hit.collider.TryGetComponent<ICapturable>(out objectHit);
            if (objectHit != null)
            {
                if (objectHit.CanCapture())
                {
                    return objectHit;
                }
            }
        }

        return null;
    }

    private void SetReadyToSwingToTrue()
    {
        readyToSwing = true;
    }
    private void SetReadyToGrabToTrue()
    {
        readyToGrab = true;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraTransform.transform.position, cameraTransform.transform.forward * swingDistanceInFront);
        Gizmos.DrawWireSphere(cameraTransform.transform.position + cameraTransform.transform.forward * swingDistanceInFront, swingRadius);
    }
    private bool IsFrontClear()
    {
        float boxDistanceInFront = 3f;
        RaycastHit hit;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward * boxDistanceInFront);
        Physics.Raycast(ray, out hit, boxDistanceInFront, throwMask);
        if (hit.collider != null)
        {
            Debug.Log(hit.transform.name);
            return false;
        }
        return true;
    }
}
