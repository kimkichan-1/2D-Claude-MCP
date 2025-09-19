using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask = 1;
    
    [Header("Character References")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] characterSprites = new Sprite[4]; // Farmer 0~3
    public RuntimeAnimatorController[] characterAnimators = new RuntimeAnimatorController[4]; // Farmer 0~3 Animators
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private Camera playerCamera;
    private PlayerAnimationController animationController;
    private WeaponController weaponController;
    public bool facingRight = true; // WeaponController가 참조할 수 있도록 public
    
    // Input System Actions
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCamera = Camera.main;
        animationController = GetComponent<PlayerAnimationController>();
        weaponController = GetComponent<WeaponController>();

        // Input System 액션 설정
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
            attackAction = playerInput.actions["Attack"];
        }
    }
    
    private void OnEnable()
    {
        if (moveAction != null) moveAction.Enable();
        if (jumpAction != null) 
        {
            jumpAction.Enable();
            jumpAction.performed += OnJump;
        }
        if (attackAction != null) 
        {
            attackAction.Enable();
            attackAction.performed += OnAttack;
        }
    }
    
    private void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (jumpAction != null)
        {
            jumpAction.Disable();
            jumpAction.performed -= OnJump;
        }
        if (attackAction != null)
        {
            attackAction.Disable();
            attackAction.performed -= OnAttack;
        }
    }
    
    private void Start()
    {
        LoadPlayerData();
    }
    
    private void Update()
    {
        HandleInput();
        CheckGrounded();
        HandleMouseDirection();
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    private void HandleInput()
    {
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }
        else
        {
            // Fallback to legacy input
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
    }
    
    private void HandleMovement()
    {
        // 수평 이동
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }
    
    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (weaponController != null)
        {
            weaponController.PerformAttack();
        }
    }
    
    private void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        }
    }
    
    private void HandleMouseDirection()
    {
        if (playerCamera == null || spriteRenderer == null) return;
        
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPos = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // 플레이어가 마우스 방향을 향하도록 설정
        bool shouldFaceRight = mouseWorldPos.x > transform.position.x;
        
        // 스프라이트 뒤집기
        spriteRenderer.flipX = !shouldFaceRight;
        
        // facingRight 업데이트 (WeaponController에서 사용)
        facingRight = shouldFaceRight;
    }
    
    public void SetCharacterSprite(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < characterSprites.Length)
        {
            if (spriteRenderer != null && characterSprites[characterIndex] != null)
            {
                spriteRenderer.sprite = characterSprites[characterIndex];
            }
            
            // 애니메이터 컨트롤러도 함께 변경
            if (characterIndex < characterAnimators.Length && characterAnimators[characterIndex] != null)
            {
                Animator animator = GetComponent<Animator>();
                if (animator != null)
                {
                    animator.runtimeAnimatorController = characterAnimators[characterIndex];
                }
            }
        }
    }
    
    public void LoadPlayerData()
    {
        int currentSlot = PlayerPrefs.GetInt("CurrentSaveSlot", 0);
        
        if (SaveSystem.Instance != null)
        {
            SaveData saveData = SaveSystem.Instance.LoadGame(currentSlot);
            
            if (saveData != null && !saveData.isEmpty)
            {
                // 위치 로드
                transform.position = saveData.playerPosition;
                
                // 캐릭터 스프라이트 설정
                SetCharacterSprite(saveData.selectedCharacterIndex);
                
                Debug.Log($"Player data loaded from slot {currentSlot}");
            }
            else
            {
                Debug.Log("No save data found or empty slot");
            }
        }
    }
    
    public void SavePlayerData()
    {
        int currentSlot = PlayerPrefs.GetInt("CurrentSaveSlot", 0);
        
        if (SaveSystem.Instance != null)
        {
            SaveData saveData = SaveSystem.Instance.LoadGame(currentSlot);
            
            if (saveData != null)
            {
                // 현재 위치 저장
                saveData.playerPosition = transform.position;
                
                // 세이브
                SaveSystem.Instance.SaveGame(currentSlot, saveData);
                Debug.Log("Player data saved");
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Ground check 영역 시각화
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}