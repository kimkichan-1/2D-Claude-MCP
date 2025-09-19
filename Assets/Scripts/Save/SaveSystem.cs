using System;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    
    [Header("세이브 설정")]
    public int maxSaveSlots = 3;
    
    private string saveDirectoryPath;
    private SaveData[] saveSlots;
    
    public event Action<SaveData[]> OnSaveSlotsUpdated;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSaveSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeSaveSystem()
    {
        saveDirectoryPath = Path.Combine(Application.persistentDataPath, "Saves");
        
        if (!Directory.Exists(saveDirectoryPath))
        {
            Directory.CreateDirectory(saveDirectoryPath);
        }
        
        saveSlots = new SaveData[maxSaveSlots];
        LoadAllSaveSlots();
    }
    
    public SaveData[] GetAllSaveSlots()
    {
        return saveSlots;
    }
    
    public SaveData GetSaveSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < maxSaveSlots)
        {
            return saveSlots[slotIndex];
        }
        return null;
    }
    
    public bool SaveGame(int slotIndex, SaveData saveData)
    {
        if (slotIndex < 0 || slotIndex >= maxSaveSlots)
        {
            Debug.LogError($"Invalid save slot index: {slotIndex}");
            return false;
        }
        
        try
        {
            saveData.lastSaveTime = DateTime.Now;
            saveData.slotName = $"Save Slot {slotIndex + 1}";
            saveData.isEmpty = false;
            
            string filePath = Path.Combine(saveDirectoryPath, $"save_slot_{slotIndex}.json");
            string jsonData = JsonUtility.ToJson(saveData, true);
            
            File.WriteAllText(filePath, jsonData);
            
            saveSlots[slotIndex] = saveData;
            OnSaveSlotsUpdated?.Invoke(saveSlots);
            
            Debug.Log($"Game saved to slot {slotIndex + 1}.");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
            return false;
        }
    }
    
    public SaveData LoadGame(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSaveSlots)
        {
            Debug.LogError($"Invalid save slot index: {slotIndex}");
            return null;
        }
        
        try
        {
            string filePath = Path.Combine(saveDirectoryPath, $"save_slot_{slotIndex}.json");
            
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(jsonData);
                
                if (saveData != null)
                {
                    Debug.Log($"Game loaded from slot {slotIndex + 1}.");
                    return saveData;
                }
            }
            
            Debug.Log($"Slot {slotIndex + 1} is empty.");
            return CreateEmptySlot(slotIndex);
        }
        catch (Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
            return CreateEmptySlot(slotIndex);
        }
    }
    
    public bool DeleteSave(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSaveSlots)
        {
            Debug.LogError($"Invalid save slot index: {slotIndex}");
            return false;
        }
        
        try
        {
            string filePath = Path.Combine(saveDirectoryPath, $"save_slot_{slotIndex}.json");
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            saveSlots[slotIndex] = CreateEmptySlot(slotIndex);
            OnSaveSlotsUpdated?.Invoke(saveSlots);
            
            Debug.Log($"Slot {slotIndex + 1} deleted.");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Delete failed: {e.Message}");
            return false;
        }
    }
    
    private void LoadAllSaveSlots()
    {
        for (int i = 0; i < maxSaveSlots; i++)
        {
            string filePath = Path.Combine(saveDirectoryPath, $"save_slot_{i}.json");
            
            if (File.Exists(filePath))
            {
                try
                {
                    string jsonData = File.ReadAllText(filePath);
                    SaveData saveData = JsonUtility.FromJson<SaveData>(jsonData);
                    saveSlots[i] = saveData ?? CreateEmptySlot(i);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Slot {i + 1} load failed: {e.Message}");
                    saveSlots[i] = CreateEmptySlot(i);
                }
            }
            else
            {
                saveSlots[i] = CreateEmptySlot(i);
            }
        }
        
        OnSaveSlotsUpdated?.Invoke(saveSlots);
    }
    
    private SaveData CreateEmptySlot(int slotIndex)
    {
        return new SaveData
        {
            slotName = $"Empty Slot {slotIndex + 1}",
            isEmpty = true,
            level = 1,
            maxHealthPoints = 100,
            healthPoints = 100,
            money = 100,
            dayCount = 1,
            timeOfDay = 0.25f // 오전 6시 시작
        };
    }
    
    public SaveData CreateNewGameData(int characterIndex)
    {
        SaveData newSave = new SaveData
        {
            playerName = $"Survivor {UnityEngine.Random.Range(1000, 9999)}",
            selectedCharacterIndex = characterIndex,
            playerPosition = Vector3.zero,
            level = 1,
            experience = 0,
            healthPoints = 100,
            maxHealthPoints = 100,
            money = 100,
            dayCount = 1,
            timeOfDay = 0.25f,
            isNightTime = false,
            infectionLevel = 0f,
            isInfected = false,
            availableTraitPoints = 1,
            isEmpty = false
        };
        
        // 초기 특성 포인트 설정 (캐릭터별 고유 특성)
        switch (characterIndex)
        {
            case 0: // Farmer 0 - 생산형
                newSave.traitPoints[1] = 1;
                break;
            case 1: // Farmer 1 - 전투형
                newSave.traitPoints[0] = 1;
                break;
            case 2: // Farmer 2 - 연구형
                newSave.traitPoints[2] = 1;
                break;
            case 3: // Farmer 3 - 균형형
                newSave.availableTraitPoints = 3;
                break;
        }
        
        // Initial inventory setup
        newSave.inventory.Add("Wood", 10);
        newSave.inventory.Add("Stone", 5);
        newSave.inventory.Add("Food", 3);
        
        return newSave;
    }
}