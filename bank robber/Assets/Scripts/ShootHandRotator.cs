using UnityEngine;

public class ShootHandRotator : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera defaultCamera;
    [SerializeField] private Camera aimCamera;

    [Header("Pitch Reference")]
    [Tooltip("If null, uses this transform's parent as the pitch reference space.")]
    [SerializeField] private Transform pitchReference;

    [Header("Pitch Limits")]
    [SerializeField] private float pitchMin = -70f;
    [SerializeField] private float pitchMax = 70f;

    [Header("Smoothing")]
    [SerializeField] private float turnSpeed = 25f;

    [Header("Gun (rotation only)")]
    [SerializeField] private Transform gun;
    [SerializeField] private Vector3 gunLocalRotOffset;

    private Quaternion armBindLocal;

    private void Awake()
    {
        armBindLocal = transform.localRotation;
    }

    private void LateUpdate()
    {
        Camera cam = GetActiveCamera();
        if (cam == null) return;

        Transform reference = pitchReference != null ? pitchReference : transform.parent;
        if (reference == null) return;

        Vector3 localDir = reference.InverseTransformDirection(cam.transform.forward).normalized;

        float pitch = -Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        Quaternion pitchRot = Quaternion.AngleAxis(pitch, Vector3.right);
        Quaternion targetLocal = armBindLocal * pitchRot;

        transform.localRotation = SmoothTo(transform.localRotation, targetLocal, turnSpeed);

        if (gun != null)
        {
            Quaternion gunTargetRot = transform.rotation * Quaternion.Euler(gunLocalRotOffset);
            gun.rotation = SmoothTo(gun.rotation, gunTargetRot, turnSpeed);
        }
    }

    private Camera GetActiveCamera()
    {
        if (aimCamera != null && aimCamera.enabled) return aimCamera;
        if (defaultCamera != null && defaultCamera.enabled) return defaultCamera;
        return null;
    }

    private static Quaternion SmoothTo(Quaternion from, Quaternion to, float speed)
    {
        return Quaternion.Slerp(from, to, 1f - Mathf.Exp(-speed * Time.deltaTime));
    }
}
