using UnityEngine;
using UnityEngine.InputSystem;

public class LookController : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera DefaultCamera;
    [SerializeField] private Camera AimCamera;
    [SerializeField] private Transform PlayerRoot;
    [SerializeField] private float sensitivity = 10f;
    [SerializeField] private float pitchSensitivity = 5f;
    [SerializeField] private float pitchDeadzone = 10f;
    [SerializeField] private float pitchUpMultiplier = 0.6f;
    [SerializeField] private float pitchDownMultiplier = 1f;
    [SerializeField] private float pitchCurve = 1.2f;
    [SerializeField] private float pitchSmoothTime = 0.05f;
    [SerializeField] private float pitchMin = -25f;
    [SerializeField] private float pitchMax = 20f;
    

    private Vector2 lookInput;
    private float pitch;
    private float smoothedLookY;
    private float lookYVelocity;
    private bool isAiming;
    public static event System.Action<bool> OnAimStateChanged;

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }


    public void OnAim(InputValue value)
    {
        bool next = value.isPressed;

        // 상태 변화 없으면 무시
        if (isAiming == next) return;

        isAiming = next;

        SetAimCamera(isAiming);

        OnAimStateChanged?.Invoke(isAiming);
    }

    private void SetAimCamera(bool value)
    {
        isAiming = value;
        AimCamera.enabled = value;
        DefaultCamera.enabled = !value;
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float yaw = lookInput.x * sensitivity * Time.deltaTime;
        float deadzone = Mathf.Clamp(pitchDeadzone, 0f, 0.99f);
        float absY = Mathf.Abs(lookInput.y);
        float dzY = absY <= deadzone ? 0f : Mathf.InverseLerp(deadzone, 1f, absY);
        float filteredY = Mathf.Sign(lookInput.y) * dzY;
        smoothedLookY = Mathf.SmoothDamp(smoothedLookY, filteredY, ref lookYVelocity, pitchSmoothTime);
        float curvedY = Mathf.Sign(smoothedLookY) * Mathf.Pow(Mathf.Abs(smoothedLookY), pitchCurve);
        if (curvedY > 0f) curvedY *= pitchUpMultiplier;
        else curvedY *= pitchDownMultiplier;
        float pitchDelta = curvedY * pitchSensitivity * Time.deltaTime;

        PlayerRoot.Rotate(0f, yaw, 0f, Space.Self);

        pitch += pitchDelta;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        cameraPivot.localRotation = Quaternion.Euler(0f, 0f, pitch);
    }
}
