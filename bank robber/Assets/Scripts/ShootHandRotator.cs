using UnityEngine;

public class ShootHandRotator : MonoBehaviour
{
    [SerializeField] private Camera defaultCamera;
    [SerializeField] private Camera aimCamera;

    private void LateUpdate()
    {
        Camera activeCam = GetActiveCamera();
        if (activeCam == null) return;

        Vector3 camForward = activeCam.transform.forward;

        // 로컬 -Y축을 카메라 forward에 정렬
        transform.rotation = Quaternion.FromToRotation(
            -Vector3.up,
            camForward
        );
    }

    private Camera GetActiveCamera()
    {
        if (defaultCamera != null && defaultCamera.enabled) return defaultCamera;
        if (aimCamera != null && aimCamera.enabled) return aimCamera;
        return null;
    }
}
