using UnityEngine;

public class DistinationArrow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform arrow;
    [SerializeField] private float rotateSpeed = 10f;

    void Update()
    {
        if (target == null) return;

        Vector3 dir = target.position - arrow.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetrot = Quaternion.LookRotation(dir);
        arrow.rotation = Quaternion.Slerp(
            arrow.rotation,
            targetrot,
            rotateSpeed * Time.deltaTime
        );
    }
}
