using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform muzzle;

    [Header("Fire")]
    [SerializeField] private float range = 100f;
    [SerializeField] private int damage = 10;
    [SerializeField] private LayerMask hitMask;

    public void Fire()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.Log("fire sucess");

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
        {
            // 데미지
            if (hit.collider.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage);
                Debug.Log("맞춤");
            }

            // 임팩트 연출
            Debug.DrawLine(ray.origin, hit.point, Color.red, 0.2f);
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * range, Color.yellow, 0.2f);
        }
    }
}
