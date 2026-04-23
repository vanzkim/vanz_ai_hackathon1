# Project Overview
- Game Title: Hospital Escape (Inferred from scene name scene_hospital)
- High-Level Concept: A horror escape game set in a hospital.
- Players: Single player.
- Render Pipeline: URP.
- Input System: New Input System.

# Game Mechanics
## Core Gameplay Loop
- Exploration, dodging monsters, and progressing through cutscenes.
- Sound is a key atmospheric element.

## Controls and Input Methods
- Third-person movement via `ThirdPersonController`.

# UI
- N/A (Focused on Audio)

# Key Asset & Context
- `Assets/Scripts/Managers/AudioManager.cs`: New central audio manager.
- `Assets/Scripts/Player/PlayerBreathing.cs`: Component for player breathing sounds.
- `Assets/Scripts/Monster/MonsterScream.cs`: Component for monster chase screams.
- `Assets/Scripts/Audio/CutsceneMusicTrigger.cs`: Script to trigger music during Timeline playback.
- `Assets/Scripts/ThirdPersonController.cs`: Reference for player speed.
- `Assets/Scripts/MonsterChase.cs`: Reference for monster detection.

# Implementation Steps
## Phase 1: Core Audio System
1. **Create `AudioManager.cs` in `Assets/Scripts/Managers/`**:
   - Implement Singleton pattern.
   - Add `AudioSource` for Music and a pool/method for SFX.
   - `PlayMusic(AudioClip clip, bool loop)`: Plays background music with fading.
   - `StopMusic()`: Stops music with fading.
   - `PlaySFX(AudioClip clip, Vector3 position)`: Plays a one-shot sound at a location.

## Phase 2: Player Breathing
1. **Create `PlayerBreathing.cs` in `Assets/Scripts/Player/`**:
   - Fields: `AudioClip[] breathingClips`, `float minInterval`, `float maxInterval`, `float speedThreshold`.
   - In `Update`, check `CharacterController.velocity.magnitude` or `ThirdPersonController` state.
   - If speed > `speedThreshold`, start/continue a timer. When timer hits 0, play a random clip and reset timer.
2. **Setup in Scene**:
   - Find the Player object (using `VanzConstants.PlayerModelName`).
   - Attach `PlayerBreathing` and assign breathing clips.

## Phase 3: Monster Scream
1. **Create `MonsterScream.cs` in `Assets/Scripts/Monster/`**:
   - Fields: `AudioClip screamClip`, `float detectionRange`.
   - Reference `MonsterChase` component.
   - Monitor the distance to player. Trigger `AudioManager.PlaySFX(screamClip, transform.position)` when the player enters `detectionRange`.
   - Ensure it only plays once per chase session (reset when player leaves range).
2. **Setup in Scene**:
   - Find the Monster object.
   - Attach `MonsterScream` and assign the scream clip.

## Phase 4: Cutscene Music
1. **Create `CutsceneMusicTrigger.cs` in `Assets/Scripts/Audio/`**:
   - Inherit from `MonoBehaviour`.
   - Fields: `AudioClip musicClip`.
   - Reference `PlayableDirector`.
   - Subscribe to `director.played` and `director.stopped`.
   - Call `AudioManager.Instance.PlayMusic(musicClip)` on `played`.
   - Call `AudioManager.Instance.StopMusic()` on `stopped`.
2. **Apply to Cutscenes**:
   - **Meet_white**: Attach `CutsceneMusicTrigger` to the `Meet_white` GameObject.
   - **Intro_Timeline**: Find the GameObject in the scene that has the `Intro` playable and attach `CutsceneMusicTrigger`.

# Verification & Testing
1. **Player Breathing**: Walk around and verify that heavy breathing sounds occur at irregular intervals.
2. **Monster Chase**: Approach the monster and verify the scream plays exactly once when the chase starts.
3. **Cutscenes**:
   - Trigger `Intro_Timeline` and verify the assigned music starts.
   - Trigger `Meet_white` and verify the assigned music starts.
   - Ensure music stops when cutscenes end.
