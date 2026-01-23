using UnityEngine;
using UnityEngine.InputSystem;

public class TpsMover : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform cam;
    [SerializeField] private CharacterController controller;

    [Header("Move")]
    [SerializeField] private float speed = 6f;

    [Header("Jump/Gravity")]
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravity = -20f;       // 음수로 두는 게 핵심
    [SerializeField] private float groundedStick = -2f;

    private Vector2 moveInput;
    private float vY;

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && controller != null && controller.isGrounded)
        {
            // v = sqrt(2 * h * -g)  (g는 음수)
            vY = Mathf.Sqrt(2f * jumpHeight * -gravity);
        }
    }

    private void Update()
    {
        if (cam == null || controller == null) return;

        // 카메라 기준 이동 벡터
        Vector3 fwd = cam.forward;  fwd.y = 0f;  fwd.Normalize();
        Vector3 right = cam.right;  right.y = 0f; right.Normalize();

        Vector3 moveDir = fwd * moveInput.y + right * moveInput.x;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        Vector3 horizontal = moveDir * speed;

        // grounded 처리(땅에 붙여두기) - grounded일 때만
        if (controller.isGrounded && vY < 0f)
            vY = groundedStick;

        // 중력 적용 (아래로)
        vY += gravity * Time.deltaTime;

        Vector3 velocity = horizontal + Vector3.up * vY;
        controller.Move(velocity * Time.deltaTime);
    }
}
