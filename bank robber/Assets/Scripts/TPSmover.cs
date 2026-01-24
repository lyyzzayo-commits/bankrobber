using UnityEngine;
using UnityEngine.InputSystem;

public class TpsMover : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform cam;
    [SerializeField] private CharacterController controller;

    [Header("Move")]
    [SerializeField] private float speed = 6f;

    [Tooltip("짧은 탭이 Update 관측에 걸리지 않으면, 최소 n프레임은 마지막 입력을 1번 더 써서 '살짝 움직임'을 보장")]
    [SerializeField] private int minMoveFramesOnTap = 1;

    [Header("Jump/Gravity")]
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravity = -20f;   // 음수
    [SerializeField] private float groundedStick = -2f;

    private Vector2 moveInput;          // 현재 입력 상태
    private Vector2 lastNonZeroMove;    // 마지막 비0 입력
    private int pendingMoveFrames;      // 탭 보장 프레임 카운트
    private float vY;                   // 수직 속도

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();

        // 입력이 0이 아니면 "탭 보장"을 예약
        if (moveInput.sqrMagnitude > 0f)
        {
            lastNonZeroMove = moveInput;
            pendingMoveFrames = Mathf.Max(0, minMoveFramesOnTap);
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (controller == null) return;

        if (ctx.performed && controller.isGrounded)
        {
            // v = sqrt(2 * h * -g)  (g는 음수)
            vY = Mathf.Sqrt(2f * jumpHeight * -gravity);
        }
    }

    private void Update()
    {
        if (cam == null || controller == null) return;

        // 1) 이번 프레임에 사용할 입력 결정 (탭 보장)
        Vector2 effectiveMove = moveInput;
        if (effectiveMove.sqrMagnitude == 0f && pendingMoveFrames > 0)
        {
            effectiveMove = lastNonZeroMove;
            pendingMoveFrames--;
        }

        // 2) 카메라 기준 이동 방향
        Vector3 fwd = cam.forward;   fwd.y = 0f;   fwd.Normalize();
        Vector3 right = cam.right;   right.y = 0f; right.Normalize();

        Vector3 moveDir = fwd * effectiveMove.y + right * effectiveMove.x;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        Vector3 horizontal = moveDir * speed;

        // 3) 땅에 붙이기 + 중력
        if (controller.isGrounded && vY < 0f)
            vY = groundedStick;

        vY += gravity * Time.deltaTime;

        // 4) 적용
        Vector3 velocity = horizontal + Vector3.up * vY;
        controller.Move(velocity * Time.deltaTime);
    }
}
