using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    public bool isGamePaused = false;
    public bool isNightTime = false;
    
    [Header("Player Reference")]
    public PlayerController playerController;
    
    [Header("Game Systems")]
    public GameObject pauseMenuUI;
    
    private SaveData currentSaveData;
    private int currentSaveSlot;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        InitializeGame();
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    private void InitializeGame()
    {
        // 현재 세이브 슬롯 가져오기
        currentSaveSlot = PlayerPrefs.GetInt("CurrentSaveSlot", 0);
        
        // 세이브 데이터 로드
        LoadGameData();
        
        // 플레이어 찾기 (혹시 연결이 안되어 있다면)
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        
        Debug.Log($"Game initialized for save slot {currentSaveSlot}");
    }
    
    private void LoadGameData()
    {
        if (SaveSystem.Instance != null)
        {
            currentSaveData = SaveSystem.Instance.LoadGame(currentSaveSlot);
            
            if (currentSaveData != null && !currentSaveData.isEmpty)
            {
                // 게임 상태 복원
                isNightTime = currentSaveData.isNightTime;
                
                Debug.Log($"Game data loaded: Day {currentSaveData.dayCount}, Level {currentSaveData.level}");
            }
        }
    }
    
    private void HandleInput()
    {
        // ESC 키로 일시정지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        
        // F5 키로 퀵 세이브
        if (Input.GetKeyDown(KeyCode.F5))
        {
            QuickSave();
        }
    }
    
    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        
        Debug.Log("Game paused");
    }
    
    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        
        Debug.Log("Game resumed");
    }
    
    public void QuickSave()
    {
        if (playerController != null)
        {
            playerController.SavePlayerData();
        }
        
        Debug.Log("Quick save completed");
    }
    
    public void ReturnToMainMenu()
    {
        // 게임 상태 초기화
        Time.timeScale = 1f;
        isGamePaused = false;
        
        // 메인 메뉴로 이동
        SceneManager.LoadScene("MainScene");
    }
    
    public SaveData GetCurrentSaveData()
    {
        return currentSaveData;
    }
    
    public void UpdateSaveData(SaveData newData)
    {
        currentSaveData = newData;
    }
}