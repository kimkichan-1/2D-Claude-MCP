using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("메인 메뉴 UI")]
    public GameObject mainMenuPanel;
    public Button startGameButton;
    public Button settingsButton;
    public Button exitButton;
    
    [Header("세이브 슬롯 UI")]
    public GameObject saveSlotPanel;
    public Button[] saveSlotButtons = new Button[3];
    public Text[] saveSlotTexts = new Text[3];
    public Text[] saveSlotInfoTexts = new Text[3];
    public Button backFromSlotsButton;
    
    [Header("설정 UI")]
    public GameObject settingsPanel;
    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Button backFromSettingsButton;
    
    [Header("캐릭터 선택 UI")]
    public GameObject characterSelectionPanel;
    public Button[] characterButtons = new Button[4];
    public Image[] characterImages = new Image[4];
    public Text[] characterNameTexts = new Text[4];
    public Button backFromCharacterButton;
    
    [Header("오디오")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    
    private SaveData[] currentSaveSlots;
    private int selectedSlotIndex = -1;
    
    private void Start()
    {
        InitializeUI();
        LoadSaveSlots();
        SetupAudio();
    }
    
    private void InitializeUI()
    {
        // 메인 메뉴 버튼 설정
        startGameButton.onClick.AddListener(ShowSaveSlots);
        settingsButton.onClick.AddListener(ShowSettings);
        exitButton.onClick.AddListener(ExitGame);
        
        // 세이브 슬롯 버튼 설정
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            int slotIndex = i; // 클로저를 위한 로컬 변수
            saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotSelected(slotIndex));
        }
        backFromSlotsButton.onClick.AddListener(ShowMainMenu);
        
        // 설정 버튼 설정
        backFromSettingsButton.onClick.AddListener(ShowMainMenu);
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
        // 캐릭터 선택 버튼 설정
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int charIndex = i;
            characterButtons[i].onClick.AddListener(() => OnCharacterSelected(charIndex));
            characterNameTexts[i].text = $"Farmer {i}";
        }
        backFromCharacterButton.onClick.AddListener(ShowSaveSlots);
        
        // 초기 패널 설정
        ShowMainMenu();
    }
    
    private void LoadSaveSlots()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.OnSaveSlotsUpdated += UpdateSaveSlotUI;
            currentSaveSlots = SaveSystem.Instance.GetAllSaveSlots();
            UpdateSaveSlotUI(currentSaveSlots);
        }
    }
    
    private void SetupAudio()
    {
        // 저장된 볼륨 설정 로드
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        
        masterVolumeSlider.value = masterVolume;
        sfxVolumeSlider.value = sfxVolume;
        
        AudioListener.volume = masterVolume;
        if (audioSource != null)
        {
            audioSource.volume = sfxVolume;
        }
    }
    
    private void UpdateSaveSlotUI(SaveData[] saveSlots)
    {
        currentSaveSlots = saveSlots;
        
        for (int i = 0; i < saveSlotButtons.Length && i < saveSlots.Length; i++)
        {
            SaveData slotData = saveSlots[i];
            
            if (slotData.isEmpty)
            {
                saveSlotTexts[i].text = $"빈 게임 {i + 1}";
                saveSlotInfoTexts[i].text = "새 게임 시작";
            }
            else
            {
                saveSlotTexts[i].text = slotData.slotName;
                saveSlotInfoTexts[i].text = $"레벨 {slotData.level} | {slotData.dayCount}일차\n{slotData.lastSaveTime:yyyy/MM/dd HH:mm}";
            }
        }
    }
    
    public void ShowMainMenu()
    {
        PlayButtonSound();
        mainMenuPanel.SetActive(true);
        saveSlotPanel.SetActive(false);
        settingsPanel.SetActive(false);
        characterSelectionPanel.SetActive(false);
    }
    
    public void ShowSaveSlots()
    {
        PlayButtonSound();
        mainMenuPanel.SetActive(false);
        saveSlotPanel.SetActive(true);
        settingsPanel.SetActive(false);
        characterSelectionPanel.SetActive(false);
    }
    
    public void ShowSettings()
    {
        PlayButtonSound();
        mainMenuPanel.SetActive(false);
        saveSlotPanel.SetActive(false);
        settingsPanel.SetActive(true);
        characterSelectionPanel.SetActive(false);
    }
    
    public void ShowCharacterSelection()
    {
        PlayButtonSound();
        mainMenuPanel.SetActive(false);
        saveSlotPanel.SetActive(false);
        settingsPanel.SetActive(false);
        characterSelectionPanel.SetActive(true);
    }
    
    private void OnSaveSlotSelected(int slotIndex)
    {
        PlayButtonSound();
        selectedSlotIndex = slotIndex;
        
        if (currentSaveSlots[slotIndex].isEmpty)
        {
            // 새 게임 시작 - 캐릭터 선택으로
            ShowCharacterSelection();
        }
        else
        {
            // 기존 게임 로드
            LoadExistingGame(slotIndex);
        }
    }
    
    private void OnCharacterSelected(int characterIndex)
    {
        PlayButtonSound();
        
        if (selectedSlotIndex >= 0)
        {
            // 새 게임 데이터 생성
            SaveData newGameData = SaveSystem.Instance.CreateNewGameData(characterIndex);
            
            // 세이브
            if (SaveSystem.Instance.SaveGame(selectedSlotIndex, newGameData))
            {
                // 게임 씬으로 이동
                PlayerPrefs.SetInt("CurrentSaveSlot", selectedSlotIndex);
                SceneManager.LoadScene("GameScene");
            }
        }
    }
    
    private void LoadExistingGame(int slotIndex)
    {
        SaveData saveData = SaveSystem.Instance.LoadGame(slotIndex);
        
        if (saveData != null && !saveData.isEmpty)
        {
            PlayerPrefs.SetInt("CurrentSaveSlot", slotIndex);
            SceneManager.LoadScene("GameScene");
        }
    }
    
    private void OnMasterVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        if (audioSource != null)
        {
            audioSource.volume = value;
        }
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }
    
    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    private void ExitGame()
    {
        PlayButtonSound();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    private void OnDestroy()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.OnSaveSlotsUpdated -= UpdateSaveSlotUI;
        }
    }
}