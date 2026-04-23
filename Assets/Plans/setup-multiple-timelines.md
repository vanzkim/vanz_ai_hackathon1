# Project Overview
- Game Title: Cutscene Trigger System Setup
- High-Level Concept: Implementing multiple triggers in the scene that play unique Timelines when the player enters them.
- Players: Single player (Third Person Controller)
- Render Pipeline: URP (PC_RPAsset detected)
- Input System: New Input System

# Game Mechanics
## Core Gameplay Loop
The player explores the environment. Upon entering specific "Trigger Zones", a cutscene (Timeline) is played, temporarily disabling player control to show narrative or environmental events.

## Controls and Input Methods
- Movement: WASD/Joystick (handled by `ThirdPersonController`).
- Input is automatically disabled during cutscenes via the `CutsceneTrigger` script's `disableDuringCutscene` field.

# UI
- No specific UI required for this setup, though the `CutsceneTrigger` can be extended to show subtitles if needed.

# Key Asset & Context
- **Scripts**: 
    - `Assets/Scripts/CutsceneTrigger.cs`: The core logic for detecting players and playing Timelines.
    - `Assets/Scripts/ThirdPersonController.cs`: The player controller script to be disabled during cutscenes.
- **Components**:
    - `PlayableDirector`: Needed for each Timeline.
    - `BoxCollider` (IsTrigger): Needed for detection.

# Implementation Steps
1. **Prepare the Player**:
    - Ensure the player GameObject has the name `Player_Model` (default in `CutsceneTrigger`) or a specific Tag (e.g., "Player").
    - Ensure the player has a `ThirdPersonController` component.

2. **Create a Cutscene Trigger Object**:
    - Create a new empty GameObject in the scene (e.g., `TimelineTrigger_01`).
    - Add a `BoxCollider` component and check **Is Trigger**. Position/Scale it to the area where the cutscene should start.
    - Add the `CutsceneTrigger` script to this object.

3. **Set up the Timeline**:
    - Create a new Timeline asset (Right-click in Project -> Create -> Timeline).
    - Create a new GameObject (e.g., `CutsceneDirector_01`) and add a `PlayableDirector` component.
    - Assign the Timeline asset to the `PlayableDirector`.
    - Set **Play On Awake** to **False** on the `PlayableDirector`.

4. **Wire the Trigger**:
    - In the `CutsceneTrigger` component:
        - Set **Mode** to `Timeline`.
        - Assign the `CutsceneDirector_01` (PlayableDirector) to the **Director** field.
        - Add the player's `ThirdPersonController` component to the **Disable During Cutscene** list.
    - (Optional) Configure **Scene Action** if you want to toggle lights or objects when the trigger fires.

5. **Repeat for Multiple Timelines**:
    - Duplicate the trigger and director setup for each additional cutscene you want to trigger in the scene.

# Verification & Testing
1. **Detection Test**: Walk the player into the Trigger zone. Verify the Timeline starts playing.
2. **Input Test**: Verify that the player cannot move while the cutscene is running.
3. **Completion Test**: Verify that the player regains control once the Timeline finishes (the `stopped` event on the Director should fire).
4. **Boundary Test**: Ensure the trigger only fires once (the script has a `hasPlayed` check).
