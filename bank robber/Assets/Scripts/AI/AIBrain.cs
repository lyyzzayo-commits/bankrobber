using UnityEngine;

[DisallowMultipleComponent]
public class AIBrain : MonoBehaviour
{
    [Header("Modules")]
    [SerializeField] private AIState state;
    [SerializeField] private AITargeting targeting;
    [SerializeField] private AICombatShooter combat;
    [SerializeField] private AIMovementMotor motor;

    private void Awake()
    {
        // TODO:
        // - state/모듈 자동 할당(GetComponent<>)
        // - 필수 참조 검증
        // - state 초기화(HP 등)
    }

    private void Update()
    {
        // TODO:
        // - if (state.IsDead) return; (또는 death 처리)
        // - targeting.Tick(state)
        // - combat.Tick(state)
        // - (필요하면 간단한 상태머신: Idle/Chase/Combat)
    }

    private void FixedUpdate()
    {
        // TODO:
        // - motor.FixedTick(state)
    }
}
