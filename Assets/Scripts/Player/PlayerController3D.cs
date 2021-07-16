using UnityEngine;

public class PlayerController3D : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float mouseSensitivity;
    private CharacterController charController;
    private string mouseXInputName, mouseYInputName;
    private string horizontalInputName, verticalInputName;
    private float xAxisClamp;

    void Start()
    {
        charController = GetComponent<CharacterController>();
        // assigned and stored once as fields, instead of using string literals in the update method, 
        // which would generate a new string object every update call, which would have to be deleted by the garbage collector afterwards
        // see 'Edit -> Project Setting -> Input' for names 
        mouseXInputName = "Mouse X";
        mouseYInputName = "Mouse Y";
        horizontalInputName = "Horizontal";
        verticalInputName = "Vertical";
        xAxisClamp = 0;
        transform.position = new Vector3(1, 1, -1);
    }
    void Update()
    {
        // lock cursor while in player mode, else unlock to be able to interact with UI
        if (playerCamera.enabled)
            LockCursor();
        else
            UnlockCursor();
        // handle mouse input (looking around) by rotating the player camera (up and down) and player object
        PlayerLook();
        // handle keyboard input (moving around) by moving the player object;
        PlayerMovement();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void PlayerLook()
    {
        // get the input axis values from the mouse and calculate the rotation value for x and y taking into consideration the mouse sensitivity
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        // clamp the rotation values at 90 and -90 respectively for looking up and down to avoid flipping the camera
        xAxisClamp += mouseY;
        if (xAxisClamp > 90f)
        {
            xAxisClamp = 90f;
            mouseY = 0f;
            // clamp x axis rotation to 270 which is looking straight up from the player cameras perspective, 180 = looking straight forward
            ClampXAxisRotationToValue(270f);
        }
        else if (xAxisClamp < -90f)
        {
            xAxisClamp = -90f;
            mouseY = 0f;
            // clamp x axis rotation to 90 which is looking straight down from the player cameras perspective, 180 = looking straight forward
            ClampXAxisRotationToValue(90);
        }
        //rotate the camera around the x axis (up and down) by multiplying mouseY input with Vector3.left = (-1,0,0), negative x direction = up, positive = down
        playerCamera.transform.Rotate(Vector3.left * mouseY);
        //rotate the player object on y axis (horizonatl) Vector3.up = (0,1,0), camera will rotate with it
        transform.Rotate(Vector3.up * mouseX);
    }

    /* This method is needed to gurantee clamping at the exact maximum/minimum values. Otherwise with fast mouse movement due to inaccuracies in and between Update calls
    you could slightly overshoot the cap everytime and that way flick the camera  */
    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = playerCamera.transform.eulerAngles;
        eulerRotation.x = value;
        playerCamera.transform.eulerAngles = eulerRotation;

    }

    private void PlayerMovement()
    {
        // calculate the vertical movement by multiplying the inputs vertical axis value (s pressed -> -1, w pressed -> 1, nothing -> 0) with movement speed
        // multiplying with Time.deltaTime to make speed independent from frame rate is not needed, since the SimpleMove method 
        // of the CharacterControlelr is doing that automatically
        float vertical = Input.GetAxis(verticalInputName) * movementSpeed;
        float horizontal = Input.GetAxis(horizontalInputName) * movementSpeed;

        // transform.forward always points forward from the object, right always points to the right, regardless of rotation
        Vector3 forwardMovement = transform.forward * vertical;
        Vector3 rightMovement = transform.right * horizontal;
        // actually move the player object by calling SimpleMove of the CharacerController and pass in the added Vector of vertical and horizontal movement
        charController.SimpleMove(forwardMovement + rightMovement);
    }

}
