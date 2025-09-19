using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform weaponHand;  // 무기를 잡을 손 위치
    public GameObject currentWeapon;  // 현재 장착된 무기
    
    [Header("Attack Settings")]
    public float attackAngle = 120f;  // 공격 부채꼴 각도
    public float attackDuration = 0.3f;  // 공격 모션 지속 시간
    public float attackDamage = 10f;  // 공격 데미지
    public LayerMask enemyLayer = 1;  // 적 레이어
    
    [Header("Mouse Direction")]
    public bool useMouseDirection = true;  // 마우스 방향 사용 여부
    
    private Camera mainCamera;
    private bool isAttacking = false;
    private PlayerController playerController;
    
    private void Start()
    {
        mainCamera = Camera.main;
        playerController = GetComponent<PlayerController>();
        
        // 기본 무기가 없으면 기본 무기 생성
        if (currentWeapon == null)
        {
            CreateDefaultWeapon();
        }
    }
    
    private void Update()
    {
        // 마우스 방향에 따른 무기 기본 각도 설정
        if (useMouseDirection && currentWeapon != null)
        {
            UpdateWeaponDirection();
        }
    }
    
    public void PerformAttack()
    {
        if (isAttacking || currentWeapon == null) return;
        
        StartCoroutine(AttackCoroutine());
    }
    
    private System.Collections.IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        
        // 공격 시작 각도와 끝 각도 계산
        float startAngle = GetCurrentWeaponAngle() - (attackAngle / 2f);
        float endAngle = GetCurrentWeaponAngle() + (attackAngle / 2f);
        
        float elapsedTime = 0f;
        Vector3 attackCenter = weaponHand.position;
        
        while (elapsedTime < attackDuration)
        {
            float progress = elapsedTime / attackDuration;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, progress);
            
            // 무기 회전
            currentWeapon.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            
            // 공격 판정 (중간 지점에서만)
            if (progress >= 0.4f && progress <= 0.6f)
            {
                CheckAttackHit(attackCenter, currentAngle);
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        isAttacking = false;
    }
    
    private void UpdateWeaponDirection()
    {
        if (mainCamera == null) return;
        
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.transform.position.z));
        
        Vector2 direction = (worldMousePosition - weaponHand.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // 캐릭터가 왼쪽을 보고 있으면 각도 조정
        if (playerController != null && !playerController.facingRight)
        {
            angle = 180f - angle;
        }
        
        // 공격 중이 아닐 때만 기본 방향 설정
        if (!isAttacking)
        {
            currentWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    
    private float GetCurrentWeaponAngle()
    {
        return currentWeapon.transform.eulerAngles.z;
    }
    
    private void CheckAttackHit(Vector3 center, float angle)
    {
        // TODO: 부채꼴 범위 내의 적들을 찾아 데미지 적용
        // 현재는 간단한 원형 범위로 구현
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, 1.5f, enemyLayer);
        
        foreach (Collider2D hit in hits)
        {
            // 적에게 데미지 적용 (나중에 Enemy 스크립트와 연동)
            Debug.Log($"Hit enemy: {hit.name} for {attackDamage} damage");
        }
    }
    
    private void CreateDefaultWeapon()
    {
        // 기본 검 무기 생성
        GameObject weapon = new GameObject("DefaultSword");
        weapon.transform.SetParent(weaponHand);
        weapon.transform.localPosition = Vector3.zero;
        
        // 스프라이트 렌더러 추가 (나중에 실제 무기 스프라이트로 교체)
        SpriteRenderer weaponRenderer = weapon.AddComponent<SpriteRenderer>();
        weaponRenderer.color = Color.gray;
        weaponRenderer.sortingOrder = 1;
        
        // 임시 사각형 스프라이트 생성 (검 모양)
        Texture2D weaponTexture = new Texture2D(4, 20);
        for (int i = 0; i < weaponTexture.width * weaponTexture.height; i++)
        {
            weaponTexture.SetPixel(i % weaponTexture.width, i / weaponTexture.width, Color.white);
        }
        weaponTexture.Apply();
        
        Sprite weaponSprite = Sprite.Create(weaponTexture, new Rect(0, 0, weaponTexture.width, weaponTexture.height), new Vector2(0.5f, 0f));
        weaponRenderer.sprite = weaponSprite;
        
        currentWeapon = weapon;
    }
}