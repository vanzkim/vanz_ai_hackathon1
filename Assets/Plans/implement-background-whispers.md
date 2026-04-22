# Project Overview
- Game Title: Hospital Escape
- High-Level Concept: Horror/Escape game.
- Feature: Background "whisper" sounds (`ItsYou_02.wav`, `ItsYou_02 1.wav`) playing periodically.
- Constraint: Only active when the player is in gameplay mode (moving/active), and **disabled during cutscenes**.

# Game Mechanics
- Ambient whispers that pause when a cutscene begins and resume when the player regains control.
- Adjustable frequency (interval range) and volume.

# Key Asset & Context
- **AudioManager.cs**: To be extended with whisper logic.
- **CutsceneManager.cs**: To be updated with a state check (`IsCutsceneActive`).
- **ItsYou_02.wav**, **ItsYou_02 1.wav**: The audio clips.

# Implementation Steps
1. **Update CutsceneManager.cs**:
    - Add a public property `public bool IsCutsceneActive => _activeCutscenePlayer != null;` to easily check current state.
2. **Extend AudioManager.cs**:
    - Add fields: `AudioClip[] whisperClips`, `float whisperMinInterval`, `float whisperMaxInterval`, `float whisperVolume`, `bool whispersEnabled`.
    - Implement `WhisperRoutine` (Coroutine):
        - Loop while `whispersEnabled`.
        - Calculate random delay.
        - Wait for delay.
        - **Condition Check**: Only play if `!CutsceneManager.Instance.IsCutsceneActive` and the player model is active.
        - Play random clip via `PlaySFXGlobal`.
3. **Setup Scene and Assets**:
    - Create a persistent `AudioManager` object in the **OpeningScene**.
    - Assign the specific clips and default intervals/volume.
4. **Trigger Whispers**:
    - Ensure `ToggleWhispers(true)` is called when gameplay starts (e.g., in `scene_hospital` or after the intro cutscene).

# Verification & Testing
- Start gameplay. Verify whispers play during exploration.
- Trigger a cutscene. Verify whispers stop or do not trigger.
- Finish the cutscene. Verify whispers resume.
- Adjust parameters in Inspector at runtime to verify responsiveness.
