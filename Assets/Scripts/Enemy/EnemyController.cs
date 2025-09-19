using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 30f;
    public float currentHealth;
    public float moveSpeed = 2f;
    public float attackDamage = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    
    [Header("AI Settings")]
    public float detectionRange = 8f;
    public LayerMask playerLayer = 1;
    
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Color normalColor = Color.white;
    public Color damageColor = Color.red;
    
    private Transform playerTransform;
    private Rigidbody2D rb;
    private bool isPlayerInRange = false;
    private bool canAttack = true;
    private float lastAttackTime;
        private EnemyAnimationController animationController;
private bool isDead = false;
    
    // AI 상태
    private enum EnemyState
    {
        Idle,
        Chasing,
        Attacking
    }
    private EnemyState currentState = EnemyState.Idle;
    
    private void Start()
    {
                animationController = GetComponent<EnemyAnimationController>();
// 컴포넌트 초기화
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 체력 초기화
        currentHealth = maxHealth;
        
        // 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure Player GameObject has 'Player' tag.");
        }
    }
    
    private void Update()
    {
        if (isDead || playerTransform == null) return;
        
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        // 상태 업데이트
        UpdateEnemyState(distanceToPlayer);
        
        // 상태에 따른 행동
        HandleCurrentState(distanceToPlayer);
    }
    
    private void UpdateEnemyState(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chasing;
        }
        else
        {
            currentState = EnemyState.Idle;
        }
    }
    
    private void HandleCurrentState(float distanceToPlayer)
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                // 가만히 있기
                rb.linearVelocity = Vector2.zero;
                break;
                
            case EnemyState.Chasing:
                // 플레이어 추적
                ChasePlayer();
                break;
                
            case EnemyState.Attacking:
                // 공격
                rb.linearVelocity = Vector2.zero;
                TryAttackPlayer();
                break;
        }
    }
    
    private void ChasePlayer()
    {
        // 플레이어 방향 계산
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        
        // 이동
        rb.linearVelocity = direction * moveSpeed;
        
        // 스프라이트 방향 조정
        if (direction.x > 0)
            spriteRenderer.flipX = false;
        else if (direction.x < 0)
            spriteRenderer.flipX = true;
    }
    
    private void TryAttackPlayer()
    {
        if (canAttack && Time.time >= lastAttackTime + attackCooldown)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
        }
    }
    
private void AttackPlayer()
    {
        // 플레이어에게 실제 데미지 주기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealthController playerHealth = player.GetComponent<PlayerHealthController>();
            if (playerHealth != null && !playerHealth.IsInvincible())
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"Enemy attacks player for {attackDamage} damage!");
            }
        }
        
        // 공격 이팩트 (색상 변경)
        StartCoroutine(AttackFlash());
        
        canAttack = false;
        
        // 쿨다운 후 다시 공격 가능하게 설정
        Invoke(nameof(ResetAttack), attackCooldown);
    }
    
    private System.Collections.IEnumerator AttackFlash()
    {
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = normalColor;
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        // 피해 애니메이션 재생
        if (animationController != null)
        {
            animationController.PlayHitAnimation();
        }
        
        // 피해 이팩트
        StartCoroutine(DamageFlash());
        
        // 죽음 체크
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private System.Collections.IEnumerator DamageFlash()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = normalColor;
    }
    
    private void ResetAttack()
    {
        canAttack = true;
    }
    
    private void Die()
    {
        isDead = true;
        currentState = EnemyState.Idle;
        rb.linearVelocity = Vector2.zero;
        
        Debug.Log("Enemy died!");
        
        // 죽음 애니메이션 재생
        if (animationController != null)
        {
            animationController.PlayDeathAnimation();
        }
        
        // 죽음 이팩트
        spriteRenderer.color = Color.gray;
        
        // 3초 후 제거 (죽음 애니메이션이 재생될 시간 여유)
        Destroy(gameObject, 3f);
    }
    
    private void OnDrawGizmosSelected()
    {
        // 감지 범위 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // 공격 범위 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}