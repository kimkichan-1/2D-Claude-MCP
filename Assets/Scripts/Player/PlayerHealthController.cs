using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    [Header("Damage Settings")]
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private bool isInvincible = false;
    
    [Header("Death Settings")]
    [SerializeField] private Vector3 respawnPosition;
    [SerializeField] private float respawnDelay = 2f;
    
    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged; // current, max
    public UnityEvent OnPlayerDied;
    public UnityEvent OnPlayerRespawned;
    
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        currentHealth = maxHealth;
        respawnPosition = transform.position;
        
        // 체력 UI 초기화
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public void TakeDamage(float damage)
    {
        if (isInvincible || currentHealth <= 0)
            return;
            
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // 체력 변경 이벤트 발생
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // 무적 시간 시작
        StartCoroutine(InvincibilityCoroutine());
        
        // 데미지 받는 효과 (깜빡임)
        StartCoroutine(DamageFlashCoroutine());
        
        // 사망 체크
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public void SetRespawnPosition(Vector3 newRespawnPosition)
    {
        respawnPosition = newRespawnPosition;
    }
    
    private void Die()
    {
        // 플레이어 입력 비활성화
        if (playerController != null)
            playerController.enabled = false;
            
        // 물리 효과 제거
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
            
        // 사망 이벤트 발생
        OnPlayerDied?.Invoke();
        
        // 일정 시간 후 부활
        Invoke(nameof(Respawn), respawnDelay);
    }
    
    private void Respawn()
    {
        // 위치 리셋
        transform.position = respawnPosition;
        
        // 체력 복구
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // 플레이어 컨트롤 재활성화
        if (playerController != null)
            playerController.enabled = true;
            
        // 무적 시간 적용
        StartCoroutine(InvincibilityCoroutine());
        
        // 부활 이벤트 발생
        OnPlayerRespawned?.Invoke();
    }
    
    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
    
    private System.Collections.IEnumerator DamageFlashCoroutine()
    {
        if (spriteRenderer == null) yield break;
        
        Color originalColor = spriteRenderer.color;
        Color flashColor = new Color(1f, 0.5f, 0.5f, 1f); // 빨간색 깜빡임
        
        float flashDuration = 0.1f;
        int flashCount = 3;
        
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }
    
    // 게터 메서드들
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;
    public bool IsAlive() => currentHealth > 0;
    public bool IsInvincible() => isInvincible;
}