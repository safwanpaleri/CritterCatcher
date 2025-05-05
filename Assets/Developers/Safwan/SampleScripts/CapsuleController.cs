using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Objects")]
    [SerializeField] private GameObject childCamera;

    [Header("Variables")]
    [SerializeField] [Range(0f, 10f)] private float movementSpeed = 5.0f; 
    [SerializeField] [Range(0f, 10f)] private float rotationSpeed = 5.0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3 (moveHorizontal,0f, moveVertical) * movementSpeed * Time.deltaTime;
        transform.Translate(movement);

        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        transform.localRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + mouseX,0f);
        childCamera.transform.localRotation = Quaternion.Euler(childCamera.transform.rotation.eulerAngles.x - mouseY, 0f, 0f);

    }
}
