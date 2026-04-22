# Project Overview
- Game Title: Hospital Horror (based on scene name and content)
- High-Level Concept: Horror adventure game set in a hospital.
- Players: Single player.
- Render Pipeline: Universal Render Pipeline (URP) - based on PC_RPAsset.
- Target Platform: Standalone (OSX).
- Screen Orientation: Landscape.

# Game Mechanics
- Core Gameplay Loop: Exploration, survival, and experiencing cinematic sequences.
- Controls and Input Methods: New Input System, Third Person Controller.

# Key Asset & Context
- **HospitalStartController.cs**: New script to manage the startup sequence (Death Cutscene -> Intro -> Player Activation).
- **Death_Cutscene_Manager**: PlayableDirector for the initial death sequence.
- **Intro_Timeline**: PlayableDirector and IntroSequenceManager for the intro sequence.
- **Player_Model**: The gameplay player model.

# Implementation Steps
1. **Create HospitalStartController Script**
   - Create a new script `Assets/Scripts/Sequences/HospitalStartController.cs`.
   - The script will:
     - Reference `Death_Cutscene_Manager` (PlayableDirector).
     - Reference `IntroSequenceManager`.
     - In `Start()`, subscribe to `Death_Cutscene_Manager.stopped`.
     - Ensure `Death_Cutscene_Manager` is playing (it is set to Play on Awake, but can be explicitly started).
     - When `Death_Cutscene_Manager` stops, call `IntroSequenceManager.PlayIntro()`.

2. **Modify IntroSequenceManager**
   - Open `Assets/Scripts/IntroSequenceManager.cs`.
   - Ensure `autoPlayOnAwake` is set to `false` in the inspector (or modify default).
   - The script already handles activating the player model via `CutsceneManager.Instance.EndCutscene()`.

3. **Configure Scene (scene_hospital)**
   - **Player_Model**: Deactivate the GameObject (set `m_IsActive` to `false`).
   - **Intro_Timeline**:
     - Enable the `IntroSequenceManager` component (it is currently disabled).
     - Ensure `autoPlayOnAwake` is `false`.
   - **Create Start Controller**:
     - Create a new GameObject named `StartSequenceController`.
     - Add the `HospitalStartController` component.
     - Assign `Death_Cutscene_Manager`'s PlayableDirector to the `Death Director` field.
     - Assign `Intro_Timeline`'s IntroSequenceManager to the `Intro Manager` field.

4. **Verification**
   - Enter Play mode.
   - Verify `Death_Cutscene_Manager` plays first.
   - Verify `Intro_Timeline` plays immediately after.
   - Verify `Player_Model` is activated and controllable after the intro ends.

# Verification & Testing
- **Test Case 1: Sequence Order**: Verify that the death cutscene plays fully before the intro starts.
- **Test Case 2: Player Activation**: Verify that the player model is visible and controllable only after the intro timeline completes.
- **Test Case 3: Camera Transition**: Verify Cinemachine cameras transition correctly between sequences and back to the gameplay camera.
