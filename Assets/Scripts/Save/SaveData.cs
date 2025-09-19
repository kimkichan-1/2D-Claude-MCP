using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Header("플레이어 정보")]
    public string playerName;
    public int selectedCharacterIndex; // Farmer 0~3
    public Vector3 playerPosition;
    public bool isInfected;
    public float infectionLevel;
    
    [Header("캐릭터 스탯")]
    public int level;
    public int experience;
    public int healthPoints;
    public int maxHealthPoints;
    
    [Header("특성 시스템")]
    public int[] traitPoints = new int[3]; // 전투형, 생산형, 연구형
    public int availableTraitPoints;
    
    [Header("게임 진행")]
    public int dayCount;
    public float timeOfDay; // 0-1 사이 값 (0=자정, 0.5=정오)
    public bool isNightTime;
    
    [Header("자원 및 인벤토리")]
    public int money;
    public SerializableDictionary<string, int> inventory;
    
    [Header("좀비화된 캐릭터들")]
    public ZombifiedCharacterData[] zombifiedCharacters;
    
    [Header("메타 정보")]
    public DateTime lastSaveTime;
    public string slotName;
    public bool isEmpty = true;
    
    public SaveData()
    {
        inventory = new SerializableDictionary<string, int>();
        zombifiedCharacters = new ZombifiedCharacterData[0];
        lastSaveTime = DateTime.Now;
    }
}

[Serializable]
public class ZombifiedCharacterData
{
    public string characterName;
    public int originalCharacterIndex;
    public Vector3 lastKnownPosition;
    public int[] originalTraitPoints;
    public DateTime zombificationTime;
    public bool canBeHealed;
}

[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField] private TKey[] keys;
    [SerializeField] private TValue[] values;
    
    private System.Collections.Generic.Dictionary<TKey, TValue> dictionary = new System.Collections.Generic.Dictionary<TKey, TValue>();
    
    public System.Collections.Generic.Dictionary<TKey, TValue> Dictionary => dictionary;
    
    public void OnBeforeSerialize()
    {
        keys = new TKey[dictionary.Count];
        values = new TValue[dictionary.Count];
        
        int i = 0;
        foreach (var kvp in dictionary)
        {
            keys[i] = kvp.Key;
            values[i] = kvp.Value;
            i++;
        }
    }
    
    public void OnAfterDeserialize()
    {
        dictionary.Clear();
        
        if (keys != null && values != null)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                dictionary[keys[i]] = values[i];
            }
        }
    }
    
    public TValue this[TKey key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }
    
    public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
    public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);
    public void Add(TKey key, TValue value) => dictionary.Add(key, value);
    public bool Remove(TKey key) => dictionary.Remove(key);
    public void Clear() => dictionary.Clear();
}