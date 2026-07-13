using UnityEngine;

public class SpaceShipScript : MonoBehaviour
{
    [Header("Thrust & Speed")]
    [SerializeField] private float forwardThrust = 50f;
    [SerializeField] private float reverseThrust = 30f;
    [SerializeField] private float lateralThrust = 40f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float boostMultiplier = 2.5f;
    [SerializeField] private float dragCoefficient = 0.2f;

    [Header("Rotation (Torque)")] // how fast rotation slows
    [SerializeField] private float pitchTorque = 5f;
    [SerializeField] private float yawTorque = 5f;
    [SerializeField] private float rollTorque = 8f;
    [SerializeField] private float angularDrag = 0.5f;  
    [SerializeField] private float maxSpinningVelocity = 20f;

    [Header("Boost")]
    [SerializeField] private KeyCode boostKey = KeyCode.LeftShift;

    [Header("Mouse Settings")]
    [SerializeField] private bool invertY = false;
    [SerializeField] private bool invertX = false;
    [SerializeField] private float mouseSensitivity = 1f;
    
    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform leftGunSpawn;
    [SerializeField] private Transform rightGunSpawn;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float bulletSpeed = 180f;
    private float nextFireTime;

    private Rigidbody rb;
    private bool isBoosting;
    
    public GameOverMenu gameOverMenu;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Spaceship requires a Rigidbody!");
            return;
        }

        rb.useGravity = false;
        rb.linearDamping = dragCoefficient;     
        rb.angularDamping = angularDrag; 
        
        InputState.current = new InputState();
    }

    void Update()
    {
        HandleInput();
        HandleShooting();
        ToggleCursorLock();
    }

    void FixedUpdate()
    {
        ApplyThrust();
        ApplyRotationTorque();
    }
    
    void HandleInput()
    {
        // Thrust (W=1, S=-1) 
        var forwardInput = Input.GetAxis("Vertical");
        isBoosting = Input.GetKey(boostKey);
        
        var rollInput = 0f;
        if (Input.GetKey(KeyCode.E)) rollInput = -1f;
        if (Input.GetKey(KeyCode.Q)) rollInput = 1f;
    

        // Lateral strafe (A=-1, D=1)
        var lateralInput = Input.GetAxis("Horizontal");
        
        var mouseDelta = new Vector2(
            Input.GetAxis("Mouse X") * mouseSensitivity,
            Input.GetAxis("Mouse Y") * mouseSensitivity
        );

       
        if (invertX) mouseDelta.x = -mouseDelta.x;
        if (invertY) mouseDelta.y = -mouseDelta.y;
        
        InputState.current = new InputState
        {
            forward = forwardInput,
            lateral = lateralInput,
            roll = rollInput,
            pitch = mouseDelta.y,
            yaw = mouseDelta.x,
            boosting = isBoosting
        };
    }

    
    private struct InputState
    {
        public float forward, lateral, roll, pitch, yaw;
        public bool boosting;
        public static InputState current;
    }

    
    void ApplyThrust()
    {
        var state = InputState.current;
        
        var forwardSpeed = state.forward > 0 ? forwardThrust : reverseThrust;
        var maxAllowed = state.boosting ? maxSpeed * boostMultiplier : maxSpeed;
        
        if (Mathf.Abs(rb.linearVelocity.magnitude) < maxAllowed || state.forward < 0)
        {
            Vector3 thrustDir = transform.forward * state.forward;
            Vector3 lateralDir = transform.right * state.lateral;
            Vector3 totalForce = (thrustDir * forwardSpeed + lateralDir * lateralThrust) * rb.mass;
            rb.AddForce(totalForce, ForceMode.Force);
        }
        
        if (rb.linearVelocity.magnitude > maxAllowed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxAllowed;
        }
    }

    // ---------- PHYSICS ROTATION (TORQUE) ----------
    void ApplyRotationTorque()
    {
        var state = InputState.current;

        // Pitch around local X, Yaw around local Y, Roll around local Z
        Vector3 torque = new Vector3(
            state.pitch * pitchTorque,    // local X
            state.yaw * yawTorque,        // local Y
            state.roll * rollTorque       // local Z
        ) * rb.mass;

       
        rb.AddRelativeTorque(torque, ForceMode.Force);  // torque in local space
        
        if (rb.angularVelocity.magnitude > 10f)
        {
                rb.angularVelocity = rb.angularVelocity.normalized * 10f;
        }
    }

    
    void HandleShooting()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (!Input.GetMouseButton(0)) return;
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        if (bulletPrefab == null || leftGunSpawn == null || rightGunSpawn == null)
        {
            Debug.LogWarning("Shooting not set up.");
            return;
        }

        SpawnBullet(leftGunSpawn);
        SpawnBullet(rightGunSpawn);
    }

    void SpawnBullet(Transform spawn)
    {
        GameObject bullet = Instantiate(bulletPrefab, spawn.position, spawn.rotation);
        BulletBehaviour script = bullet.GetComponent<BulletBehaviour>();
        if (script != null) script.SetSpeed(bulletSpeed);
    }

  
    private static void ToggleCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ?
                CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = (Cursor.lockState == CursorLockMode.None);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Planet") || other.CompareTag("Sun"))
        {
            PlanetBehaviour planet = other.GetComponent<PlanetBehaviour>();
            if (planet != null) planet.DestroyPlanet();
            else Destroy(other.gameObject);
            Death();
            SunBehaviour sun = other.GetComponent<SunBehaviour>();
            if (sun != null)
            {
                Death();
            }
        }
    }

    public void Death()
    {
        Time.timeScale = 0f;
        gameOverMenu.EnableGameOverScreen();
    }
}