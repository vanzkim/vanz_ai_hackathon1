# Project Overview
- Game Title: Hospital Horror (Inferred from scene name)
- High-Level Concept: Horror game set in an abandoned hospital.
- Players: Single player.
- Render Pipeline: URP.
- Input System: New Input System.

# Game Mechanics
## Core Gameplay Loop
Exploration and surviving jump scares while navigating the hospital.
## Controls and Input Methods
Standard first/third person movement.

# UI
Not directly relevant to this task.

# Key Asset & Context
- **Trigger_BigGirl**: A trigger object (InstanceID: 319108) with a BoxCollider.
- **Big_Girl_Window**: A GameObject (InstanceID: -114954) that should be shown temporarily.
- **TimedActivationTrigger.cs**: New script to handle the timed activation and sound effect.
- **AudioManager**: Existing manager to handle SFX playback.

# Implementation Steps
1. **Create Script**: Create `Assets/Scripts/Triggers/TimedActivationTrigger.cs` to handle the activation logic.
2. **Attach Script**: Add `TimedActivationTrigger` component to `Trigger_BigGirl`.
3. **Configure Trigger**:
    - Assign `Big_Girl_Window` to the `Target Object` field.
    - Set `Duration` to `2.5` seconds.
    - Select a suitable impact sound (e.g., `Monster_Scream` or `radio_bigNoise`) for the `Impact Sfx` field.
4. **Initial State**: Set `Big_Girl_Window` to inactive (Active = false) in the scene so it starts hidden.
5. **Add AudioSource**: Add an `AudioSource` to `Trigger_BigGirl` for 3D sound positioning.

# Verification & Testing
- Enter Play mode.
- Walk into the `Trigger_BigGirl` collider.
- Verify `Big_Girl_Window` appears immediately.
- Verify the impact sound plays.
- Verify `Big_Girl_Window` disappears after ~2.5 seconds.
- Verify the trigger does not activate again (OneShot).
