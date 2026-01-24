
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerCarController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Rigidbody rb;

    [Header("Input (debug)")]
    [SerializeField, Range(-1f, 1f)] private float throttleInput; // -1..1
    [SerializeField, Range(-1f, 1f)] private float steerInput;    // -1..1
    [SerializeField] private bool brakeInput;

    [Header("Move Tuning")]
    [SerializeField] private float accel = 45f;          // m/s^2 느낌
    [SerializeField] private float maxSpeed = 25f;       // m/s
    [SerializeField] private float brakePower = 40f;     // 감속 강도
    [SerializeField, Range(0f, 1f)] private float sideFriction = 0.2f; // (옵션) 추후 사용

    [SerializeField] private float stopThreshold;

    [SerializeField] private bool allowReverse = true;

    [Header("Steer Tuning")]
    [SerializeField] private float turnRate = 120f;      // deg/sec at speedFactor=1
    [SerializeField] private float minSteerSpeed = 1.0f; // 너무 저속일 때 회전 억제
    [SerializeField] private float maxSteerSpeed = 20f;

    [Header("Wheel Visuals")]
    [SerializeField] private Transform wheelF;
    [SerializeField] private Transform wheelB;

    [Tooltip("앞바퀴 조향이 wheel Transform 자체가 아니라 피벗(부모)에서 일어나야 하면 여기에 할당")]
    [SerializeField] private Transform steerPivotF;
    

    [SerializeField] private float wheelRadius = 0.33f;     // meter
    [SerializeField] private float maxSteerAngle = 30f;     // degrees

    [Header("State")]
    [SerializeField] private bool controlEnabled = true;

    // Cached
    private Transform[] wheelMeshes;
    private float wheelRollAccumDeg;

    private float yawDeltaDeg;

    

    private void Awake()
    {
        
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            CacheWheelRefs();
        }
    }


    

    private void FixedUpdate()
    {
        if (!controlEnabled) return;
        ApplyMovePhysics();
        ApplySteerPhysics();
        ApplyBrakePhysics();
    }

    private void LateUpdate()
    {
        UpdateWheelVisuals();
    }

    // -------------------------
    // Functions (TODO skeleton)
    // -------------------------

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if(!controlEnabled)
        {
            throttleInput = 0f;
            steerInput = 0f;
            return;
        }
        Vector2 v = ctx.ReadValue<Vector2>();
        steerInput = Mathf.Clamp(v.x,-1f,1f);
        throttleInput = Mathf.Clamp(v.y,-1f,1f);
    }

    public void OnBrake(InputAction.CallbackContext ctx)
    {
        if(!controlEnabled)
        {
            brakeInput = false;
            return;
        }

        float pressed = ctx.ReadValue<float>();
        brakeInput = pressed > 0.5f;
    }

    private void CacheWheelRefs()
    {
        wheelMeshes = new Transform[2] { wheelF, wheelB };

        if (wheelF == null) Debug.LogWarning($"{name}: wheelF is missing.");
        if (wheelB == null) Debug.LogWarning($"{name}: wheelB is missing.");
        
        // const bool fallbackToWheelAsPivot = true;
        if (steerPivotF == null)
        {
            if(wheelF != null)
            {
                steerPivotF = wheelF;
                Debug.LogWarning($"{name}: steerPivotF is not assigned. Falling back to wheelFL as pivot (prototype only).");

            }
            else
            {
                Debug.LogWarning($"{name}: steerPivotFL is not assigned. Create a parent pivot for Wheel_FL and assign it.");
            }
        }

        
            
    }

    private void ApplyMovePhysics()
    {
       if (rb == null) return;

       float throttle = throttleInput;

       Vector3 force = transform.forward * throttle * accel;
       rb.AddForce(force,ForceMode.Acceleration);

       Vector3 vel = rb.linearVelocity;
       
       float forwardSpeed = Vector3.Dot(vel, transform.forward);

       float clampedForwardSpeed = Mathf.Clamp(forwardSpeed,-maxSpeed,maxSpeed);

       Vector3 forwardVel = transform.forward * clampedForwardSpeed;

       Vector3 lateralVel = vel - transform.forward * forwardSpeed;

       rb.linearVelocity = forwardVel + lateralVel;
    }

    

    private void ApplySteerPhysics()
    {
        Vector3 vel = rb.linearVelocity;

        float forwardSpeed = Vector3.Dot(vel,transform.forward);
        
        float absSpeed = Mathf.Abs(forwardSpeed);
        if (absSpeed < minSteerSpeed) return;
        
        float speedFactor = Mathf.InverseLerp(minSteerSpeed,maxSteerSpeed,absSpeed);

        float reverseFactor = (forwardSpeed >= 0f) ? 1.0f : -1.0f;
        
        yawDeltaDeg = steerInput * turnRate * speedFactor * Time.fixedDeltaTime * reverseFactor;

        Quaternion delta = Quaternion.Euler(0f,yawDeltaDeg,0f);

        rb.MoveRotation(rb.rotation * delta);

    }

    private void ApplyBrakePhysics()
    {
        if (!brakeInput) return;

        Vector3 vel = rb.linearVelocity;

        float forwardSpeed = Vector3.Dot(vel,transform.forward);

        if (Mathf.Abs(forwardSpeed) < stopThreshold) // 속도가 0에 가까워지면 떨림을 방지하기 위해
        {
            rb.linearVelocity = vel - transform.forward * forwardSpeed;
            return;
        }
        
        float decel = brakePower * Time.fixedDeltaTime;

        float newForwardSpeed = Mathf.MoveTowards(forwardSpeed,0f,decel);

        Vector3 forwardVel = transform.forward * forwardSpeed;

        Vector3 lateralVel = vel - forwardVel;
        
        rb.linearVelocity = lateralVel + transform.forward * newForwardSpeed;
    }

    private void UpdateWheelVisuals()
    {
        if (rb == null) return;

        if (wheelMeshes == null || wheelMeshes.Length < 4) return;
        if (wheelMeshes[0] == null || wheelMeshes[1] == null) return;

        Vector3 vel = rb.linearVelocity;
        float forwardSpeed = Vector3.Dot(vel,transform.forward);

        float rollDeg = ComputeWheelRollDegrees(forwardSpeed,Time.deltaTime);

        wheelRollAccumDeg += rollDeg;

        ApplyWheelRollLocalX(wheelMeshes[0], wheelRollAccumDeg); 
        ApplyWheelRollLocalX(wheelMeshes[1], wheelRollAccumDeg); 
        

        if (steerPivotF != null)
            steerPivotF.localRotation = Quaternion.Euler(0f,yawDeltaDeg,0f);
        
    }    
    private float ComputeWheelRollDegrees(float forwardSpeed, float dt)
    {
        if (wheelRadius <= 0f)
            return 0f;
        
        float circumference = 2f * Mathf.PI * wheelRadius;

        float distance = forwardSpeed * dt;

        float revolutions = distance / circumference;

        float degrees = revolutions * 360f;

        return degrees;
    }

    private void ApplyWheelRollLocalX(Transform wheel,float rollAccumDeg)
    {
        if (wheel == null) return;

        Vector3 e = wheel.localEulerAngles;
        e.x = rollAccumDeg;
        wheel.localEulerAngles = e;
    }
}
