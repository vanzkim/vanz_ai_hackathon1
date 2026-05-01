# Project Overview
- **Game Title**: Vanz AI Hackathon Project
- **High-Level Concept**: A horror/escape game where the player navigates through scenes like a hospital and eventually reaches an ending.
- **Players**: Single player.
- **Target Platform**: Standalone (macOS/PC).
- **Render Pipeline**: URP (PC_RPAsset).

# Game Mechanics
## Core Gameplay Loop
The player explores an environment, avoids monsters, and triggers sequences that lead to the game's conclusion.
## Cutscene Transition
Smooth transition between gameplay control and cinematic sequences. When a cutscene starts, the gameplay character (`Player_Model`) should be swapped with a cinematic version (`Player_Cutscene`). When it ends, the reverse should happen, syncing the final position of the cutscene model back to the gameplay model.

# UI
- **Credits UI**: Displayed at the end of the game.
- **Cutscene UI/Overlays**: Managed by the `EndingSequenceManager`.

# Key Asset & Context
- **EndingSequenceManager.cs**: Manages the sequence of events in the ending scene.
- **CutsceneManager.cs**: A singleton that handles the logic for swapping players and restoring cameras.
- **EndingScene**: The scene containing the final sequence.
- **Player_Cutscene**: The cinematic model used during the "Escape" cutscene.
- **Player_Model**: The actual player-controlled model.

# Implementation Steps
1. **Modify EndingSequenceManager.cs**:
   - Add a `GameObject` field for `playerCutscene1` (and `playerCutscene2` for completeness) to reference the cinematic models.
   - Update `ExecuteSequence` to use `VanzAI.Managers.CutsceneManager.Instance.StartCutscene()` and `EndCutscene()` instead of manual `SetActive` calls.
   - This ensures that `Player_Cutscene` is hidden automatically and `Player_Model` is restored at the correct position.

2. **Update Inspector References**:
   - Select the `EndingSequenceManager` object in the `EndingScene`.
   - Assign the `Player_Cutscene` GameObject to the new field in the `EndingSequenceManager` component.

3. **Refine Cutscene Manager Integration**:
   - Ensure the `EndingSequenceManager` handles the timeline state properly while the `CutsceneManager` manages the actor visibility.

# Verification & Testing
1. **Manual Playtest**: Start the `EndingScene`.
2. **Observe Start**: Verify `Player_Model` disappears and `Player_Cutscene` appears when the sequence begins.
3. **Observe End**: Verify `Player_Cutscene` is hidden and `Player_Model` is active and positioned where the cutscene ended once the timeline stops.
4. **Log Check**: Monitor the Console for `[CutsceneManager]` logs confirming registration and finalization.
