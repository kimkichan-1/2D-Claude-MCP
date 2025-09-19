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
        private System.Collections.Generic.HashSet<EnemyController> hitEnemiesThisAttack = new System.Collections.Generic.HashSet<EnemyController>();
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
        
        // 처음에는 무기를 숨김
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }
    }
    
    private void Update()
    {
        // WeaponHand 위치를 캐릭터 방향에 따라 조정
        UpdateWeaponHandPosition();
        
        // 마우스 방향에 따른 무기 기본 각도 설정
        if (useMouseDirection && currentWeapon != null)
        {
            UpdateWeaponDirection();
        }
    }

    private void UpdateWeaponHandPosition()
    {
        if (weaponHand != null && playerController != null)
        {
            // 캐릭터가 바라보는 방향에 따라 WeaponHand x 위치 조정
            Vector3 handPos = weaponHand.localPosition;
            handPos.x = playerController.facingRight ? 0.3f : -0.3f;
            weaponHand.localPosition = handPos;
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
        
        // 이번 공격에서 맞은 적들 목록 초기화
        hitEnemiesThisAttack.Clear();
        
        // 공격 시작 시 무기 표시
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(true);
        }
        
        // 마우스 방향을 기준으로 ±60도 범위로 공격
        float mouseAngle = GetMouseAngle();
        
        float startAngle, endAngle;
        
        // 캐릭터 방향에 따라 공격 방향 조정 (항상 아래에서 위로)
        if (playerController != null && playerController.facingRight)
        {
            // 오른쪽을 볼 때: 아래에서 위로
            startAngle = mouseAngle + 60f;
            endAngle = mouseAngle - 60f;
        }
        else
        {
            // 왼쪽을 볼 때: 아래에서 위로
            startAngle = mouseAngle - 60f;
            endAngle = mouseAngle + 60f;
        }
        
        float elapsedTime = 0f;
        Vector3 attackCenter = weaponHand.position;
        bool hasAttacked = false; // 한 번만 공격하도록 플래그
        
        while (elapsedTime < attackDuration)
        {
            float progress = elapsedTime / attackDuration;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, progress);
            
            // 무기 회전
            currentWeapon.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            
            // 공격 판정 (중간 지점에서 한 번만)
            if (!hasAttacked && progress >= 0.4f && progress <= 0.6f)
            {
                CheckAttackHit(attackCenter, currentAngle);
                hasAttacked = true; // 한 번 공격했으므로 플래그 설정
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 공격 종료 시 무기 숨김
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }
        
        isAttacking = false;
    }
    
    private void UpdateWeaponDirection()
    {
        // 공격 중이 아닐 때는 무기가 비활성화되어 있으므로 아무것도 하지 않음
        // (공격 중에만 무기가 보이므로 기본 방향 설정 불필요)
    }
    
    private float GetMouseAngle()
    {
        if (mainCamera == null || weaponHand == null) return 0f;
        
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.transform.position.z));
        
        Vector2 direction = (worldMousePosition - weaponHand.position).normalized;
        // x에 음수를 붙여서 좌우 방향 수정
        float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
        
        return angle;
    }
    
    private void CheckAttackHit(Vector3 center, float angle)
    {
        // 범위 내의 적들을 찾아 데미지 적용
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, 1.5f, enemyLayer);
        
        foreach (Collider2D hit in hits)
        {
            // Enemy에게 데미지 적용
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null && !hitEnemiesThisAttack.Contains(enemy))
            {
                // 이번 공격에서 아직 맞지 않은 적에게만 데미지
                enemy.TakeDamage(attackDamage);
                hitEnemiesThisAttack.Add(enemy); // 맞은 적 목록에 추가
                Debug.Log($"Hit enemy: {hit.name} for {attackDamage} damage");
            }
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
        
        // 더 큰 사각형 스프라이트 생성 (검 모양)
        Texture2D weaponTexture = new Texture2D(8, 40); // 크기를 2배로 증가
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