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

    private Vector2 lookInput;
    private float pitch;
    private bool isAiming;

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
        if (Mathf.Abs(lookInput.y) < pitchDeadzone) lookInput.y = 0f;
    }


    public void OnAim(InputValue value)
    {
        SetAimCamera(value.isPressed);
        Debug.Log(value.isPressed);
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
        float pitchDelta = lookInput.y * pitchSensitivity* Time.deltaTime;

        PlayerRoot.Rotate(0f, yaw, 0f, Space.Self);

        pitch -= pitchDelta;
        pitch = Mathf.Clamp(pitch, -30f, 30f);

        cameraPivot.localRotation = Quaternion.Euler(0f, 0f, pitch);
    }
}
