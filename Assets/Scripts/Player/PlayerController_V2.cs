using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_V2 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask = 1;
    
    [Header("Character System")]
    public CharacterData[] availableCharacters = new CharacterData[4];
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    
    [Header("Current Character")]
    public CharacterData currentCharacter;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private Camera playerCamera;
    private PlayerAnimationController animationController;
    
    // Input System Actions
    private InputAction moveAction;
    private InputAction jumpAction;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCamera = Camera.main;
        animationController = GetComponent<PlayerAnimationController>();
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        if (animator == null)
            animator = GetComponent<Animator>();
        
        // Input System 액션 설정
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
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
    }
    
    private void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (jumpAction != null)
        {
            jumpAction.Disable();
            jumpAction.performed -= OnJump;
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
        // 현재 캐릭터의 moveSpeed 사용
        float currentMoveSpeed = currentCharacter != null ? currentCharacter.moveSpeed : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput.x * currentMoveSpeed, rb.linearVelocity.y);
    }
    
    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            // 현재 캐릭터의 jumpForce 사용
            float currentJumpForce = currentCharacter != null ? currentCharacter.jumpForce : jumpForce;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForce);
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
        
        // 플레이어 위치와 마우스 위치 비교
        if (mouseWorldPos.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }
    
    private void LoadPlayerData()
    {
        // 현재 세이브 슬롯에서 플레이어 데이터 로드
        int currentSlot = PlayerPrefs.GetInt("CurrentSaveSlot", 0);
        
        if (SaveSystem.Instance != null)
        {
            SaveData saveData = SaveSystem.Instance.LoadGame(currentSlot);
            
            if (saveData != null && !saveData.isEmpty)
            {
                // 캐릭터 설정
                SetCharacter(saveData.selectedCharacterIndex);
                
                // 플레이어 위치 설정
                transform.position = saveData.playerPosition;
                
                Debug.Log($"Player data loaded: Character {saveData.selectedCharacterIndex}, Position {saveData.playerPosition}");
            }
        }
    }
    
    public void SetCharacter(int characterIndex)
    {
        if (availableCharacters == null || characterIndex < 0 || characterIndex >= availableCharacters.Length)
        {
            Debug.LogError($"Invalid character index: {characterIndex}");
            return;
        }
        
        CharacterData characterData = availableCharacters[characterIndex];
        if (characterData == null)
        {
            Debug.LogError($"Character data is null for index: {characterIndex}");
            return;
        }
        
        // 현재 캐릭터 설정
        currentCharacter = characterData;
        
        // 스프라이트 설정
        if (spriteRenderer != null && characterData.characterSprite != null)
        {
            spriteRenderer.sprite = characterData.characterSprite;
        }
        
        // 애니메이터 컨트롤러 설정
        if (animator != null && characterData.animatorController != null)
        {
            animator.runtimeAnimatorController = characterData.animatorController;
        }
        
        // 스탯 적용
        moveSpeed = characterData.moveSpeed;
        jumpForce = characterData.jumpForce;
        
        Debug.Log($"Character set: {characterData.characterName} (Index: {characterIndex})");
        Debug.Log($"Stats - Speed: {moveSpeed}, Jump: {jumpForce}");
        Debug.Log($"Traits - Combat: {characterData.combatTrait}, Production: {characterData.productionTrait}, Research: {characterData.researchTrait}");
    }
    
    public CharacterData GetCurrentCharacter()
    {
        return currentCharacter;
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
                
                // 캐릭터 특성 저장 (현재 캐릭터 기반)
                if (currentCharacter != null)
                {
                    saveData.traitPoints[0] = currentCharacter.combatTrait;
                    saveData.traitPoints[1] = currentCharacter.productionTrait;
                    saveData.traitPoints[2] = currentCharacter.researchTrait;
                }
                
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