# Project Overview
- Game Title: Hospital Escape (Inferred from scene names)
- High-Level Concept: Horror/Mystery exploration in a hospital.
- Players: Single player.
- Render Pipeline: URP.
- Target Platform: Standalone.

# Game Mechanics
## Core Gameplay Loop
- Exploration of the hospital environment.
- Finding keys to unlock progress/ending.
- Reaching specific triggers to advance the story.

## Interaction System
- Trigger-based interactions for door locking and key pickup.
- Audio feedback for player actions and environment state.

# UI
- No specific UI changes requested, but a HUD notification for "Key Obtained" could be a nice addition (will focus on core request first).

# Key Asset & Context
- `EndingTrigger.cs`: Existing script that handles scene transition. Needs modification to check for key.
- `PlayerInventory.cs`: New script to store player state (hasKey).
- `KeyPickup.cs`: New script for the `Keys` object to handle the pickup logic and sound.
- `Trigger_WhiteExit` (GameObject): Needs an `AudioSource` and reference to "Locked" sound.
- `Keys` (GameObject): Needs a `BoxCollider` (Trigger), `AudioSource`, and the `KeyPickup` script.

# Implementation Steps
## 1. Create Player Inventory System
- **File:** `Assets/Scripts/PlayerInventory.cs`
- **Logic:** A simple singleton or component that holds a `public bool hasKey`.
- **Dependency:** None.

## 2. Create Key Pickup Logic
- **File:** `Assets/Scripts/KeyPickup.cs`
- **Logic:** 
    - Detects player entry via `OnTriggerEnter`.
    - Plays "keys shaking" audio.
    - Sets `PlayerInventory.hasKey = true`.
    - Destroys or disables the `Keys` mesh.
- **Dependency:** Step 1.

## 3. Modify Ending Trigger Logic
- **File:** `Assets/Scripts/EndingTrigger.cs`
- **Logic:**
    - Add fields for `AudioSource` and `AudioClip` (locked sound).
    - In `OnTriggerEnter`, check if the player has the key using `PlayerInventory`.
    - If `!hasKey`: Play locked sound effect and return (don't transition).
    - If `hasKey`: Proceed with `GameSceneManager.Instance.LoadScene`.
- **Dependency:** Step 1.

## 4. Audio Generation & Assignment
- **Action:** Use `GenerateAsset` to create:
    - `door_locked_click.wav`: A sharp, metallic clicking sound.
    - `keys_rattling.wav`: A jingle of metallic keys.
- **Action:** Assign these clips to the respective `AudioSource` components in the scene.
- **Dependency:** None.

## 5. Scene Setup
- **Action:** 
    - Add `PlayerInventory` to the `Player_Model` object.
    - Add `BoxCollider` (Set to `Is Trigger`) to the `Keys` object.
    - Add `AudioSource` to `Keys` and `Trigger_WhiteExit`.
    - Configure the `EndingTrigger` and `KeyPickup` components with their audio clips.
- **Dependency:** Steps 1, 2, 3, 4.

# Verification & Testing
1. **Initial Trigger Test:** Walk into `Trigger_WhiteExit` immediately. Verify the "locked" sound plays and the scene does NOT change.
2. **Key Collection Test:** Walk to the `Keys` object. Verify the "rattling" sound plays and the `Keys` object disappears (or is disabled).
3. **Ending Test:** Walk back to `Trigger_WhiteExit` after collecting the key. Verify the "locked" sound does NOT play and the scene transitions to `EndingScene`.
