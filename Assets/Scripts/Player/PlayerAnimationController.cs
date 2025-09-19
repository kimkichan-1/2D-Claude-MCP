using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animation Components")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    
    [Header("Animation Parameters")]
    private readonly string ANIM_IS_MOVING = "IsMoving";
    private readonly string ANIM_IS_GROUNDED = "IsGrounded";
    private readonly string ANIM_VERTICAL_SPEED = "VerticalSpeed";
    private readonly string ANIM_IS_DEAD = "IsDead";
    private readonly string ANIM_DEATH_TRIGGER = "Death";
    
    private PlayerController playerController;
    private Rigidbody2D rb;
    private bool wasMoving = false;
    private bool isDead = false;
    
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        UpdateAnimationParameters();
        
        // 사망 상태일 때는 다른 애니메이션 비활성화
        if (isDead) return;
UpdateAnimationParameters();
    }
    
    private void UpdateAnimationParameters()
    {
        if (animator == null || rb == null) return;
        
        // 이동 상태 체크
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        animator.SetBool(ANIM_IS_MOVING, isMoving);
        
        // 지면 상태 체크 (PlayerController의 isGrounded 값 사용)
        bool isGrounded = CheckGrounded();
        animator.SetBool(ANIM_IS_GROUNDED, isGrounded);
        
        // 수직 속도 (점프/낙하 애니메이션용)
        animator.SetFloat(ANIM_VERTICAL_SPEED, rb.linearVelocity.y);
        
        // 이동 시작/정지 시 로그 (디버깅용)
        if (isMoving != wasMoving)
        {
            Debug.Log($"Player animation: {(isMoving ? "Moving" : "Idle")}");
            wasMoving = isMoving;
        }
    }
    
    public void PlayDeathAnimation()
    {
        if (animator == null) return;
        
        isDead = true;
        
        // 사망 애니메이션 재생
        animator.SetBool(ANIM_IS_DEAD, true);
        animator.SetTrigger(ANIM_DEATH_TRIGGER);
        
        Debug.Log("Player death animation played");
    }
    
    public void ResetDeathAnimation()
    {
        if (animator == null) return;
        
        isDead = false;
        
        // 사망 애니메이션 리셋
        animator.SetBool(ANIM_IS_DEAD, false);
        
        Debug.Log("Player death animation reset");
    }
    
    public bool IsDeadAnimationPlaying()
    {
        return isDead;
    }
    
    private bool CheckGrounded()
    {
        // PlayerController의 GroundCheck 로직과 동일하게 구현
        if (playerController != null && playerController.groundCheck != null)
        {
            return Physics2D.OverlapCircle(
                playerController.groundCheck.position, 
                playerController.groundCheckRadius, 
                playerController.groundLayerMask
            );
        }
        return false;
    }
    
    public void SetCharacterAnimator(RuntimeAnimatorController animatorController)
    {
        if (animator != null && animatorController != null)
        {
            animator.runtimeAnimatorController = animatorController;
            Debug.Log($"Animator controller set: {animatorController.name}");
        }
    }
    
    public void PlaySpecialAnimation(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }
    
    // 애니메이션 이벤트용 메서드들
    public void OnFootstep()
    {
        // 발걸음 소리나 이펙트 재생
        Debug.Log("Footstep");
    }
    
    public void OnJumpStart()
    {
        Debug.Log("Jump animation started");
    }
    
    public void OnLanding()
    {
        Debug.Log("Landing animation");
    }
}