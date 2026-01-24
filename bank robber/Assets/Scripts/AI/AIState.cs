using UnityEngine;
[DisallowMultipleComponent]
public class AIState : MonoBehaviour
{
    // =========================
    // Identity (ScoreBoard/UI 공통용)
    // =========================
    [Header("Identity")]
    [Tooltip("ScoreBoard에서 킬/데스를 집계할 때 사용하는 유니크 ID")]
    public int actorId;                 // TODO: 스폰 시 발급/세팅
    [Tooltip("팀전이면 사용. 팀 없으면 0")]
    public int teamId = 0;              // TODO: 필요 없으면 제거
    [Tooltip("디버그용 이름/종류")]
    public string actorName = "AI";     // TODO: 선택

    // =========================
    // Refs (AI 모듈들이 공유)
    // =========================
    [Header("Refs")]
    public Rigidbody rb;                // TODO: 이동 모듈에서 사용
    public Transform muzzle;            // TODO: 전투 모듈에서 발사 원점
    public Transform modelRoot;         // TODO: 시각 회전/연출용(선택)
    public Collider mainCollider;       // TODO: 히트 판정/비활성(선택)

    // =========================
    // Runtime Status
    // =========================
    [Header("Runtime Status")]
    [Tooltip("Health 스크립트가 죽음/리스폰을 관리한다면, 그 상태를 여기에도 반영(읽기용)")]
    public bool isDead;                 // TODO: Health에서 동기화하거나 이벤트로 갱신
    [Tooltip("리스폰 무적 등 전투/피격 무시 플래그(선택)")]
    public bool invulnerable;           // TODO: Health에서 관리하면 읽기만 하도록

    // =========================
    // Targeting (공유 타겟 상태)
    // =========================
    [Header("Targeting")]
    public Transform currentTarget;     // TODO: AITargeting이 갱신
    public float lastTargetTime;        // TODO: 마지막 타겟 확보 시간(선택)
    public float targetRefreshTimer;    // TODO: 타겟 갱신 타이머(Brain/Targeting에서 사용)

    // =========================
    // Combat Runtime (쿨다운/연출)
    // =========================
    [Header("Combat Runtime")]
    public float fireTimer;             // TODO: 발사 쿨다운
    public float lastFireTime;          // TODO: 마지막 발사 시간(선택)

    // =========================
    // Movement Runtime (선택: 조향/방황)
    // =========================
    [Header("Movement Runtime")]
    public float wanderSeed;            // TODO: 개체별 랜덤 워크 패턴용
    public Vector3 desiredMoveDir;      // TODO: 디버그/모듈 간 공유(선택)

    // =========================
    // Optional: local cache (디버그용)
    // - 정답은 ScoreBoard가 들고, 여기 값은 UI 캐시/디버그용으로만 쓰기
    // =========================
    [Header("Score Cache (Optional)")]
    public int cachedKills;             // TODO: ScoreBoard에서 읽어와 표시할 때만 사용
    public int cachedDeaths;            // TODO: ScoreBoard에서 읽어와 표시할 때만 사용

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();
        modelRoot = transform;
        muzzle = null;
    }

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (modelRoot == null) modelRoot = transform;

        
        // // TODO:
        // // - 개체별 방황 시드 부여(고정된 랜덤 패턴을 원하면 instance id 기반)
        // wanderSeed = GetInstanceID() * 0.001f;

    }

    public Vector3 GetMuzzlePosition(float fallbackUp = 1.2f)
    {
        if (muzzle != null)
            return muzzle.position;

        if (rb != null)
            return rb.position + Vector3.up * fallbackUp;

        return transform.position + Vector3.up * fallbackUp;   
    }

    public bool HasTarget => currentTarget != null;

    public void ClearTarget()
    {
        currentTarget = null;

        lastTargetTime = 0f;
        targetRefreshTimer = 0f;
    }
}
