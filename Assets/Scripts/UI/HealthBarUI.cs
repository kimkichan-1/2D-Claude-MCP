using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;
    
    [Header("Health Bar Colors")]
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color dangerColor = Color.red;
    [SerializeField] private float warningThreshold = 0.5f;
    [SerializeField] private float dangerThreshold = 0.25f;
    
    [Header("Animation")]
    [SerializeField] private float animationSpeed = 5f;
    
    private float targetHealthPercentage = 1f;
    private PlayerHealthController playerHealth;
    
    void Start()
    {
        // 플레이어 체력 컨트롤러 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealthController>();
            if (playerHealth != null)
            {
                // 체력 변경 이벤트 구독
                playerHealth.OnHealthChanged.AddListener(UpdateHealthDisplay);
                
                // 초기 체력 설정
                UpdateHealthDisplay(playerHealth.GetCurrentHealth(), playerHealth.GetMaxHealth());
            }
        }
        
        // UI 초기화
        if (healthBarSlider != null)
            healthBarSlider.value = 1f;
    }
    
    void Update()
    {
        // 부드러운 체력바 애니메이션
        if (healthBarSlider != null)
        {
            healthBarSlider.value = Mathf.Lerp(healthBarSlider.value, targetHealthPercentage, animationSpeed * Time.deltaTime);
        }
    }
    
    public void UpdateHealthDisplay(float currentHealth, float maxHealth)
    {
        targetHealthPercentage = currentHealth / maxHealth;
        
        // 체력 색상 업데이트
        UpdateHealthBarColor(targetHealthPercentage);
        
        // 체력 텍스트 업데이트
        if (healthText != null)
        {
            healthText.text = $"{Mathf.Ceil(currentHealth)}/{maxHealth}";
        }
    }
    
    private void UpdateHealthBarColor(float healthPercentage)
    {
        if (healthBarFill == null) return;
        
        Color targetColor;
        
        if (healthPercentage > warningThreshold)
        {
            targetColor = healthyColor;
        }
        else if (healthPercentage > dangerThreshold)
        {
            targetColor = warningColor;
        }
        else
        {
            targetColor = dangerColor;
        }
        
        healthBarFill.color = targetColor;
    }
    
    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthDisplay);
        }
    }
}