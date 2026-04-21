using Unity.VisualScripting;
using UnityEngine;

public class SpaceShipScript : MonoBehaviour
{
    [Header("SpaceShip Acceleration")]
    [SerializeField] private float acceleration = 10f;     
    [SerializeField] private float maxSpeed = 100f;         
    [SerializeField] private float boostMultiplier = 2.5f; 
    [SerializeField] private float dragCoefficient = 0.5f; // feels better with drag over time so the player just doesn't continue to the infinite
    [Header("Boost Settings")]
    [SerializeField] private Rigidbody rb;
    private bool isThrusting = false;
    private bool isBoosting = false;
    
    [Header("SpaceShip Rotation Settings")]
    [SerializeField] private float baseRotation = 0.1f;
    [SerializeField] private float maxRotation = 5;
    [SerializeField] private float smooth = 10f;
    [SerializeField] private float velocityThreshold = 5f;
    
    [Header("SpaceShip Roll Settings")]
    [SerializeField] private float rollSpeed = 100f;
    
    [Header("SpaceShip Axis Settings")]
    [SerializeField] private bool invertY = false;
    [SerializeField] private bool invertX = false;
    
    // Mouse Tracking
    private Vector2 previousMousePosition;
    private Vector2 currentMousePosition;
    private Vector2 mouseSpeed;
    private float previousTime;
    private Vector2 mouseDelta;  
    
    private Vector3 targetRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (rb == null)
        {
            Debug.LogError("Spaceship requires a Rigidbody component!");
            return;
        }
        
        rb.useGravity = false;   // Disable default gravity if forgotten & add drag
        rb.linearDamping = dragCoefficient;
        
        previousMousePosition = Input.mousePosition;
        currentMousePosition = previousMousePosition; // is done this way on start cuz previous must logically be loaded before current.
        previousTime = Time.time;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Confined)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
        MovementInputHandler();
        ThrustInputHandler();
        MouseInputHandler();
        RotationHandler(); 
        RollHandler();
        SmoothRotation();
    }

    private void MovementInputHandler()
    {
        isThrusting = Input.GetKey(KeyCode.W);
        isBoosting = Input.GetKey(KeyCode.LeftShift);
    }

    private void ThrustInputHandler()
    {
        if (!isThrusting) return;
        
        var currentMaxSpeed = isBoosting ? maxSpeed * boostMultiplier : maxSpeed;

        if (rb.linearVelocity.magnitude < currentMaxSpeed)
        {
            var thrustForce = transform.forward * acceleration * rb.mass;
            rb.AddForce(thrustForce, ForceMode.Force);
        }
        
        if (rb.linearVelocity.magnitude > currentMaxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentMaxSpeed;
        }
    }

    private void MouseInputHandler()
    {
        mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        mouseSpeed = Vector2.Lerp(mouseSpeed, mouseDelta / Time.deltaTime, 0.1f);
    }

    private void RotationHandler()
    {
        if (mouseDelta == Vector2.zero) return;

        var horizontalInput = mouseDelta.x;
        var verticalInput = mouseDelta.y;

        if (invertX) horizontalInput = -horizontalInput;
        if (invertY) verticalInput = -verticalInput;

        
        var speedMultiplier = Mathf.Clamp01(mouseSpeed.magnitude / maxRotation);
        var currentRotationSpeed = baseRotation * (1 + speedMultiplier * 9); // 1x to 10x scaling

        var pitch = verticalInput * currentRotationSpeed;
        var yaw = horizontalInput * currentRotationSpeed;

        targetRotation += new Vector3(-pitch, yaw, 0);
        targetRotation.x = Mathf.Clamp(targetRotation.x, -80f, 80f);
        targetRotation.y = Mathf.Repeat(targetRotation.y, 360f);
    }

    private void SmoothRotation()
    {
        var targetQuaternion = Quaternion.Euler(targetRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, smooth * Time.deltaTime);
    }
    
    private void ResetRotation()
    {
        targetRotation = Vector3.zero;
    }

    private void RollHandler()
    {
        var rollInput = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            rollInput -= 1f; // Left roll
        }
        if (Input.GetKey(KeyCode.D))
        {
            rollInput += 1f; // Right roll
        }
        if (rollInput == 0f) return;

        var roll = rollInput * rollSpeed * Time.deltaTime;
        targetRotation += new Vector3(0, 0, -roll); 

        targetRotation.z = Mathf.Repeat(targetRotation.z, 360f);
    }

}








/* Deprecated Code due to issues with locking the screen
 
private void Update()
{  
        var delta = (Vector2)Input.mousePosition - previousMousePosition;
        if (delta.magnitude > 0)
        {
            transform.Rotate(-delta.y * 0.1f, delta.x * 0.1f, 0);
        }
        previousMousePosition = Input.mousePosition;
  }
  
  ----------------------------------------------------------------------------------
  
  
 private void MouseInputHandler()
 {
currentMousePosition = Input.mousePosition;
var currentTime = Time.time;
var deltaTime = currentTime - previousTime;

if (deltaTime > 0.0001f)
{
    Vector2 deltaPosition = currentMousePosition - previousMousePosition;
    mouseSpeed = deltaPosition / deltaTime;

    mouseSpeed = Vector2.ClampMagnitude(mouseSpeed, 5000f);

    if (mouseSpeed.magnitude < velocityThreshold)
    {
        mouseSpeed = Vector2.zero;
    }
}
else
{
    mouseSpeed = Vector2.zero;
}

previousMousePosition = currentMousePosition;
previousTime = currentTime;
}

-----------------------------------------------------------------------


private void RotationHandler()
{
if (mouseSpeed == Vector2.zero) return;

var horizontalInput = mouseSpeed.x;
var verticalInput = mouseSpeed.y;

if (invertX) horizontalInput = -horizontalInput;
if (invertY) verticalInput = -verticalInput;

var velocityMagnitude = mouseSpeed.magnitude;
var speedMultiplier = Mathf.Clamp01(velocityMagnitude / maxRotation);
var currentRotationSpeed = baseRotation * speedMultiplier;

var pitch = verticalInput * currentRotationSpeed * Time.deltaTime;
var yaw = horizontalInput * currentRotationSpeed * Time.deltaTime;

targetRotation += new Vector3(-pitch, yaw, 0);
targetRotation.x = Mathf.Clamp(targetRotation.x, -80f, 80f);


targetRotation.y = Mathf.Repeat(targetRotation.y, 360f);
}
*/