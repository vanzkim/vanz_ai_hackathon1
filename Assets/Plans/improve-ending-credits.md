# Project Overview
- Game Title: vanz_ai_hackathon1
- High-Level Concept: Horror/Adventure game with a cinematic ending sequence.
- Players: Single player.
- Render Pipeline: URP.
- Target Platform: StandaloneOSX (Mac).
- Input System: New Input System.

# Game Mechanics
## Core Gameplay Loop
The player experiences an ending sequence consisting of two cutscenes, followed by an interactive credits screen that returns to the main menu.
## Controls and Input Methods
- Keyboard: Any key to skip credits.
- Mouse: Left click to skip credits.

# UI
- **Credits_UI**: A full-screen UI panel with a black background and text. It will now support fading and input-based skipping.

# Key Asset & Context
- **Assets/Scripts/EndingSequenceManager.cs**: The main script controlling the flow of the ending sequence.
- **Assets/Scenes/EndingScene.unity**: The scene containing the ending sequence and credits UI.
- **Credits_UI (GameObject)**: The UI panel for credits.

# Implementation Steps
## 1. Update Credits UI Component
- **File**: `Assets/Scenes/EndingScene.unity`
- **Change**: 
    - Locate `Credits_UI` (Instance ID: 69906).
    - Add a `CanvasGroup` component to `Credits_UI`.
    - Set the `CanvasGroup.alpha` to `0` initially.
    - Set the `Image` component (Instance ID: 69908) color on `Credits_UI` to solid black (RGBA: 0, 0, 0, 1). Currently it is 0.5 alpha.

## 2. Enhance EndingSequenceManager Script
- **File**: `Assets/Scripts/EndingSequenceManager.cs`
- **Change**:
    - Add `using UnityEngine.InputSystem;` to the namespaces.
    - Modify the `ExecuteSequence` coroutine in the credits display section (Step 4):
        - Implement a fade-in loop for the `CanvasGroup.alpha`.
        - Implement a wait loop that monitors for 10 seconds OR any keyboard/mouse input.
        - Use `GameSceneManager.Instance.LoadScene(GameSceneManager.SceneType.Opening)` for the final transition to ensure a smooth fade-out if the manager supports it.
    - **Dependencies**: Step 1 (CanvasGroup must exist or be added in code).

# Verification & Testing
- **Manual Check**: Play the ending sequence and verify:
    1. The credits background is fully black.
    2. The entire credits panel fades in smoothly.
    3. Pressing any key or clicking the mouse during the credits immediately returns to the Title scene.
    4. If no input is given, it returns to the Title scene after 10 seconds.
