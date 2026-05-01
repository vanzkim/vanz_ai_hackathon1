# Project Overview
- Game Title: Hospital Horror (Assumed)
- High-Level Concept: Horror game set in a hospital with monsters chasing the player.
- Players: Single player
- Render Pipeline: URP
- Input System: New Input System

# Game Mechanics
## Core Gameplay Loop
- Exploration and survival in a hospital.
- Monsters chase the player when in range.
- Cutscenes occur at specific points (e.g., meeting NPCs, caught by monsters).

## Controls and Input Methods
- Character movement and interaction.

# UI
- N/A for this task.

# Key Asset & Context
- `Assets/Scripts/MonsterChase.cs`: Handles monster movement and chasing logic.
- `Assets/Scripts/Managers/CutsceneManager.cs`: Manages cutscene states and player swapping.
- `Assets/Scripts/Audio/CutsceneMusicTrigger.cs`: Plays music during cutscenes and is attached to major cutscenes like `Meet_white`.
- `Assets/Scripts/Sequences/HospitalStartController.cs`: Manages the opening sequence.
- `Assets/Scripts/CutsceneTrigger.cs`: Trigger for cutscenes.
- `Assets/Scripts/MonsterCaughtHandler.cs`: Handles being caught by a monster.

# Implementation Steps
## 1. Modify `CutsceneManager.cs`
- Add a private counter `_activeCutsceneCount` to track active cutscenes.
- Update `IsCutsceneActive` property to return true if `_activeCutsceneCount > 0` OR `_activeCutscenePlayer != null`.
- Add `RegisterCutscene(bool active)` method to increment/decrement the counter.
- Call `RegisterCutscene` inside `StartCutscene` and `EndCutscene` to ensure existing systems are covered.

## 2. Modify `MonsterChase.cs`
- In the `Update` method, check `VanzAI.Managers.CutsceneManager.Instance.IsCutsceneActive`.
- If a cutscene is active:
    - Call `StopChase()`.
    - Ensure animations are updated to idle/stopped state.
    - `return` early to skip movement logic.

## 3. Modify `CutsceneMusicTrigger.cs`
- Update `OnPlayableDirectorPlayed` to call `CutsceneManager.Instance.RegisterCutscene(true)`.
- Update `OnPlayableDirectorStopped` to call `CutsceneManager.Instance.RegisterCutscene(false)`.
- This ensures cutscenes like `Meet_white` (which use this script) are tracked by `CutsceneManager`.

## 4. Modify `HospitalStartController.cs`
- Notify `CutsceneManager` when the death cutscene starts and ends.
- This covers the initial opening cutscene which doesn't use the standard `CutsceneManager.StartCutscene` flow.

# Verification & Testing
- **Test Opening Cutscene**: Verify monsters do not move while the opening death cutscene is playing.
- **Test Meet_white Cutscene**: Verify monsters stop chasing when `Meet_white` starts and resume when it ends.
- **Test Monster Catch**: Verify that if one monster catches the player, others stop moving during the caught cutscene.
- **Manual Check**: Ensure `CutsceneManager.Instance.IsCutsceneActive` correctly returns to `false` after each cutscene.
