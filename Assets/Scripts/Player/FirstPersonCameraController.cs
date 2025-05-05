using UnityEngine;
//Credits: https://youtu.be/f473C43s8nE?si=sP3vS8Cw1WLd-tr0 && https://youtu.be/vBWcb_0HF1c?si=evpS0rS6HtbmqZ07

//TODO: Cleanup
public class FirstPersonCameraController : MonoBehaviour
{

    [Header("Look Parameters")]
    [SerializeField] [Range(0f,2f)] private float mouseSensitivty = 0.1f;
    [SerializeField] [Range(5f,90f)] private float upDownLookRange = 80f;

    [Header("References")]
    [SerializeField] private Transform cameraBodyOrientor;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private float verticalRotation;
    private float horizontalRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //Update cameras position to the position of the body
        mainCamera.transform.position = cameraBodyOrientor.position;

        // Camera rotation is handled seperatley from position and body
        HandleRotation();
    }


    private void HandleRotation()
    {
        float mouseXRotation = playerInputHandler.LookInput.x * mouseSensitivty;
        float mouseYRotation = playerInputHandler.LookInput.y * mouseSensitivty;

        //XRotation becomes vertical and y rotation becomes horizontal due to how the input is collected
        verticalRotation += mouseXRotation;
        horizontalRotation = Mathf.Clamp(horizontalRotation - mouseYRotation, -upDownLookRange, upDownLookRange);

        mainCamera.transform.rotation = Quaternion.Euler(horizontalRotation, verticalRotation, 0);

        // The CBO gets rotated on y axis so that the movement controller can use the forward and right vector for movement
        cameraBodyOrientor.transform.rotation = Quaternion.Euler(0, verticalRotation, 0);
    }


}
