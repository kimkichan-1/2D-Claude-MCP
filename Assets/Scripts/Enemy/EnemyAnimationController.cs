using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [Header("Animation Components")]
    public Animator animator;
    
    [Header("Animation Parameters")]
    private readonly string PARAM_IS_MOVING = "IsMoving";
    private readonly string PARAM_IS_DEAD = "IsDead";
    private readonly string TRIGGER_HIT = "Hit";
    
    private EnemyController enemyController;
    private Rigidbody2D rb;
    private bool wasMoving = false;
    private bool isDead = false;
    
    private void Start()
    {
        // 컴포넌트 참조 설정
        if (animator == null)
            animator = GetComponent<Animator>();
        
        enemyController = GetComponent<EnemyController>();
        rb = GetComponent<Rigidbody2D>();
        
        if (animator == null)
        {
            Debug.LogError("EnemyAnimationController: Animator component not found!");
        }
    }
    
    private void Update()
    {
        if (animator == null) return;
        
        UpdateMovementAnimation();
    }
    
    private void UpdateMovementAnimation()
    {
        // 현재 이동 상태 확인
        bool isMoving = IsMoving();
        
        // 이동 상태가 변경되었을 때만 애니메이터 업데이트
        if (isMoving != wasMoving)
        {
            animator.SetBool(PARAM_IS_MOVING, isMoving);
            wasMoving = isMoving;
        }
    }
    
    private bool IsMoving()
    {
        if (rb == null) return false;
        
        // 속도가 일정 임계값보다 크면 이동 중으로 판단
        return rb.linearVelocity.magnitude > 0.1f;
    }
    
    public void PlayHitAnimation()
    {
        if (animator == null || isDead) return;
        
        // Hit 트리거 실행
        animator.SetTrigger(TRIGGER_HIT);
        Debug.Log("Enemy Hit animation triggered");
    }
    
    public void PlayDeathAnimation()
    {
        if (animator == null || isDead) return;
        
        isDead = true;
        animator.SetBool(PARAM_IS_DEAD, true);
        Debug.Log("Enemy Death animation triggered");
    }
    
    // 애니메이션 상태 확인용 메서드들
    public bool IsPlayingHitAnimation()
    {
        if (animator == null) return false;
        
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        return currentState.IsName("Hit");
    }
    
    public bool IsPlayingDeathAnimation()
    {
        if (animator == null) return false;
        
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        return currentState.IsName("Dead");
    }
    
    // 디버그용 메서드
    private void OnValidate()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }
}