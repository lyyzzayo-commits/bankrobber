using UnityEngine;

public class ShootHandRotator : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera defaultCamera;
    [SerializeField] private Camera aimCamera;

    [Header("Gun Follow (no parenting)")]
    [SerializeField] private Transform gun;                // 총 루트(부모로 붙이지 않을 것)
    [SerializeField] private Vector3 gunLocalPosOffset;    // 손 기준 로컬 위치 오프셋
    [SerializeField] private Vector3 gunLocalRotOffset;    // 손 기준 로컬 회전 오프셋(오일러)

    private void LateUpdate()
    {
        // 1) 손 회전: 로컬 -Y축이 활성 카메라 forward를 바라보게
        Camera activeCam = GetActiveCamera();
        if (activeCam != null)
        {
            Vector3 camForward = activeCam.transform.forward;

            transform.rotation = Quaternion.FromToRotation(-Vector3.up, camForward);
        }

        // 2) 총을 손에 "부모처럼" 따라오게 (부모-자식 없이)
        if (gun != null)
        {
            // 손의 로컬 기준 오프셋을 월드로 변환
            gun.position = transform.TransformPoint(gunLocalPosOffset);

            // 손 회전에 오프셋 회전 추가
            gun.rotation = transform.rotation * Quaternion.Euler(gunLocalRotOffset);
        }
    }

    private Camera GetActiveCamera()
    {
        // aim이 켜지면 aim을 우선
        if (aimCamera != null && aimCamera.enabled) return aimCamera;
        if (defaultCamera != null && defaultCamera.enabled) return defaultCamera;
        return null;
    }
}
