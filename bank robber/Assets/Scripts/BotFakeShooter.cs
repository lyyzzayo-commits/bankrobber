using UnityEngine;

public class BotFakeShooter : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private float searchRadius = 12f;
    [SerializeField] private LayerMask targetMask = ~0;
    [SerializeField] private Transform targetPoint;

    [Header("Firing")]
    [SerializeField] private float fireInterval = 0.3f;
    [SerializeField, Range(0f, 1f)] private float hitChance = 0.35f;
    [SerializeField] private Vector2 damageRange = new Vector2(8f, 14f);

    [Header("Rotation")]
    [SerializeField] private float turnSpeed = 12f;

    private float nextFireTime;
    private Transform currentTarget;

    private void Update()
    {
        AcquireTarget();
        RotateTowardsTarget();
        TryFire();
    }

    private void AcquireTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRadius, targetMask, QueryTriggerInteraction.Ignore);
        float bestSqr = float.PositiveInfinity;
        Transform best = null;

        for (int i = 0; i < hits.Length; i++)
        {
            Transform t = hits[i].transform;
            float sqr = (t.position - transform.position).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = t;
            }
        }

        currentTarget = best;
    }

    private void RotateTowardsTarget()
    {
        if (currentTarget == null) return;

        Vector3 targetPos = targetPoint != null ? targetPoint.position : currentTarget.position;
        Vector3 toTarget = targetPos - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1f - Mathf.Exp(-turnSpeed * Time.deltaTime));
    }

    private void TryFire()
    {
        if (currentTarget == null) return;
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireInterval;

        if (Random.value > hitChance) return;

        int dmg = 10;
        Health health = currentTarget.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage(dmg);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
