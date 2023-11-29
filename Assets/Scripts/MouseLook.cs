using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MouseLook : MonoBehaviour 
{
    public float sens;

    public Transform playerBody;
    public PlayerMovement playerMovementScript;

    float _xRotation = 0f;

    public Vector3 lookInput;
    public float rollMultiplier = 2f;


    public float tiltAngle = 15.0f; 
    public float tiltSmoothness = 5.0f; 

    float currentTilt = 0.0f;

    void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        lookInput.x = Input.GetAxis("Mouse X") * Time.deltaTime * sens;
        lookInput.y = Input.GetAxis("Mouse Y") * Time.deltaTime * sens;
        _xRotation -= lookInput.y;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);


        float horizontalInput = Input.GetAxis("Horizontal");
        float targetTilt = horizontalInput * tiltAngle;

        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSmoothness);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, -currentTilt);
        playerBody.Rotate(Vector3.up * lookInput.x);



    }
}