# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 2D project (Unity 6000.0.41f1) implementing a zombie survival platformer game inspired by Terraria but with day/night cycle mechanics. The project includes:
- Unity MCP integration via `com.coplaydev.unity-mcp` for direct Unity Editor control
- Universal Render Pipeline (URP) 17.0.4 for optimized 2D rendering
- New Input System 1.13.1 with comprehensive action maps
- Character-based gameplay with 4 farmer characters (each with unique traits)
- Persistent save system with 3 save slots supporting JSON serialization
- BigManJD Platformer Tileset assets for 2D development
- Day/night survival loop with zombie wave defense mechanics
- Infection/cure system with character zombification consequences

## Development Commands

Since this is a Unity project, all build and testing is handled through the Unity Editor:

### Unity Editor Operations
- **Play Mode Testing**: Use Unity Editor Play button or Unity MCP `manage_editor` action with `"play"`
- **Build Project**: File → Build Settings in Unity Editor, or use Unity MCP menu operations
- **Console Debugging**: Use `mcp__UnityMCP__read_console` to check Unity console messages
- **Scene Management**: Use `mcp__UnityMCP__manage_scene` for loading/saving scenes

### Unity MCP Commands
```
# Read Unity console for errors/warnings
mcp__UnityMCP__read_console with action="get"

# Execute Unity menu items (like builds)
mcp__UnityMCP__manage_menu_item with action="execute" and menu_path="File/Build Settings"

# Validate scripts for syntax errors
mcp__UnityMCP__validate_script with uri="unity://path/Assets/Scripts/..."
```

## Game Architecture

### Core Systems
The game follows a modular singleton pattern with these key systems:

**GameManager** (`Assets/Scripts/Managers/GameManager.cs`)
- Central game state management (pause/resume, night/day cycle)
- Save slot coordination and quick save (F5 key)
- Scene transition management
- Player reference management

**SaveSystem** (`Assets/Scripts/Save/SaveSystem.cs`)
- Persistent data management with JSON serialization
- 3-slot save system with automatic slot validation
- Save directory: `%USERPROFILE%\AppData\LocalLow\DefaultCompany\2d Claude MCP\Saves`
- Event-driven save slot updates (`OnSaveSlotsUpdated`)

**PlayerController** (`Assets/Scripts/Player/PlayerController.cs`)
- 2D platformer movement with Rigidbody2D physics
- New Input System integration with fallback to legacy input
- Character sprite management (4 farmer variants)
- Mouse-based sprite flipping and ground detection
- Automatic save data loading and position restoration

### Data Architecture
**SaveData** (`Assets/Scripts/Save/SaveData.cs`)
- Comprehensive player state including position, health, inventory, traits
- Character-specific trait point allocation (0=Combat, 1=Production, 2=Research, 3=Balanced)
- Day/night cycle progression and infection mechanics
- Dictionary-based inventory system

### Scene Structure
- **MainScene.unity**: Main menu and character selection (Save slot selection UI)
- **GameScene.unity**: Primary gameplay scene (Day/night cycle, zombie survival)
- **SampleScene.unity**: Original demo scene

### Game Design Patterns

**Day/Night Cycle Loop**
- Day: Farming (food production/sales), exploration (resource gathering), base fortification
- Night: Zombie wave defense with increasing difficulty over time
- Money earned during day used for base expansion and NPC survivor hiring

**Infection System Workflow**
- Zombie bites during night increase infection gauge
- Cure crafting required with failure probability
- Failed cure → Character zombification → Save slot loss
- Zombified characters become world NPCs (can be encountered/defeated/potentially cured)
- New survivor selection allows game continuation

**Character Progression**
- Experience-based leveling with stat increases and trait points
- Starting traits: Combat (damage/crit), Production (farming/crafting), Research (cure success/NPC efficiency)
- Zombification transforms traits into zombie-specific abilities

## Unity MCP Integration

**ALWAYS use Unity MCP tools for Unity operations instead of traditional file tools:**

### Script Management
- `mcp__UnityMCP__create_script` - Create new C# scripts with proper Unity structure
- `mcp__UnityMCP__script_apply_edits` - Structured edits (methods/classes) with safer boundaries
- `mcp__UnityMCP__apply_text_edits` - Precise line/column text replacements
- `mcp__UnityMCP__validate_script` - Check script syntax and common Unity issues

### Scene and GameObject Operations
- `mcp__UnityMCP__manage_scene` - Load, save, create scenes and get hierarchy
- `mcp__UnityMCP__manage_gameobject` - Create, modify, find GameObjects and components
- `mcp__UnityMCP__manage_asset` - Asset operations (import, create, modify, delete)

### Debugging and Editor Control
- `mcp__UnityMCP__read_console` - Read Unity console messages for errors/warnings
- `mcp__UnityMCP__manage_editor` - Control Unity editor state (play/pause/tools)
- `mcp__UnityMCP__manage_menu_item` - Execute Unity menu commands

### Resource Access
- `mcp__UnityMCP__read_resource` - Read Unity files with proper URI handling
- `mcp__UnityMCP__find_in_file` - Search within Unity scripts using regex
- `mcp__UnityMCP__list_resources` - List project files under Assets/

## Input System Configuration

Configured in `Assets/InputSystem_Actions.inputactions` with two action maps:

**Player Map**: Move (Vector2), Look, Attack, Interact (Hold), Crouch, Jump, Sprint, Previous, Next
**UI Map**: Navigate, Submit, Cancel, Point, Click, RightClick, MiddleClick, ScrollWheel, TrackedDevicePosition, TrackedDeviceOrientation

Supports multiple control schemes: Keyboard & Mouse, Gamepad, Touch, Joystick, XR

## Key Development Patterns

### Singleton Pattern
- GameManager and SaveSystem use DontDestroyOnLoad singleton pattern
- Instance null checks with automatic destruction of duplicates

### Save System Integration
- PlayerController automatically loads character data and position on Start()
- GameManager coordinates save operations and maintains current slot reference
- Use `PlayerPrefs.GetInt("CurrentSaveSlot", 0)` for slot persistence across scenes

### Input Handling
- Primary: New Input System with PlayerInput component and action references
- Fallback: Legacy Input.GetAxisRaw() for backwards compatibility
- Mouse input handled separately for sprite direction (not through Input System)

## Script Organization

- `Assets/Scripts/Managers/` - GameManager and system-level singletons
- `Assets/Scripts/Player/` - PlayerController, CharacterData, PlayerAnimationController
- `Assets/Scripts/Save/` - SaveSystem, SaveData
- `Assets/Scripts/UI/` - MainMenuUI, MainMenuUI_TMP
- `Assets/BigManJD/` - Complete 2D platformer art pipeline and demo scene

## Asset Structure

### Character Assets
- Farmer sprites: `Assets/Undead Survivor/Sprites/Farmer 0~3` (referenced in Prompt.md)
- Character selection system supports 4 unique farmer variants with individual traits

### Environment Assets
- Tilemap textures: `Assets/BigManJD/Platformer Tileset - Pixelart Grasslands/Sprites/Textures/Tiles`
- Platform-based tilemap system for 2D level construction

## Key Implementation Notes

### Save System Integration
- Save directory: `%USERPROFILE%\AppData\LocalLow\DefaultCompany\2d Claude MCP\Saves`
- PlayerController automatically loads character data and position on Start()
- Use `PlayerPrefs.GetInt("CurrentSaveSlot", 0)` for slot persistence across scenes
- SaveSystem uses event-driven updates: `OnSaveSlotsUpdated` for UI synchronization

### Input Handling Strategy
- **Primary**: New Input System with PlayerInput component and action references
- **Fallback**: Legacy Input.GetAxisRaw() for backwards compatibility
- **Mouse Control**: Handled separately for sprite direction (not through Input System)
- **Quick Save**: F5 key triggers GameManager quick save functionality

### Performance Considerations
- All managers use DontDestroyOnLoad singleton pattern with Instance null checks
- Automatic cleanup of duplicate manager instances to prevent memory leaks
- URP 2D renderer optimized for 2D sprite batching and lighting effects

## Procedural World Generation

### Hybrid System Architecture
- **Core Areas**: Hand-crafted spawn points, major outposts, special dungeons, city ruins
- **General Areas**: Auto-generated fields, forests, expansion areas using Perlin Noise + biome rules
- **Special Structures**: Hand-crafted map pieces randomly placed in auto-generated regions

### Chunk System
- **Chunk Size**: 32x32 tiles
- **Loading**: Player-centered 3x3 chunk system (9 active chunks)
- **Biomes**: Plains (grass, trees, farming), Forest (wood, medium danger), Ruins (rare resources, high zombie density), Mountains (minerals, cave systems)

### Performance Optimization
- Dynamic chunk loading/unloading based on player position
- Tilemap collider optimization for large worlds
- Object pooling for enemies and items
- Sprite Atlas batching for consistent performance