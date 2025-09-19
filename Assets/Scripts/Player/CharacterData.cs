using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Character Info")]
    public string characterName;
    public int characterIndex;
    
    [Header("Visual")]
    public Sprite characterSprite;
    public RuntimeAnimatorController animatorController;
    
    [Header("Stats")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public int maxHealth = 100;
    
    [Header("Traits")]
    public int combatTrait = 0;      // 전투형
    public int productionTrait = 0;  // 생산형
    public int researchTrait = 0;    // 연구형
    
    [Header("Starting Equipment")]
    public string[] startingItems;
}