# Project Overview
- The user wants monsters to remain inactive (no chasing) until the `Meet_white` cutscene has finished.

# Game Mechanics
- Monsters should not chase the player at the start of the game.
- Chasing should only be enabled globally after the specific cutscene `Meet_white` concludes.

# Key Asset & Context
- `Assets/Scripts/Managers/CutsceneManager.cs`: Holds global cutscene state.
- `Assets/Scripts/MonsterChase.cs`: Monster AI logic.
- `Assets/Scripts/Audio/CutsceneMusicTrigger.cs`: Script attached to `Meet_white` cutscene.

# Implementation Steps
## 1. Update `CutsceneManager.cs`
- Add `public bool CanMonstersChase { get; set; } = false;` to track if monsters are allowed to chase.

## 2. Update `MonsterChase.cs`
- Modify the `Update` early return condition to check `!CutsceneManager.Instance.CanMonstersChase`.

## 3. Update `CutsceneMusicTrigger.cs`
- Add a serialized boolean `isMeetWhiteCutscene`.
- When the cutscene stops, if `isMeetWhiteCutscene` is true, set `CutsceneManager.Instance.CanMonstersChase = true`.

## 4. Scene Configuration
- Find the `Meet_white` object in `scene_hospital`.
- Set `isMeetWhiteCutscene` to `true` on its `CutsceneMusicTrigger` component.

# Verification & Testing
- Start the game and ensure monsters do not chase the player initially.
- Trigger the `Meet_white` cutscene.
- Verify that after the cutscene ends, monsters start chasing the player when in range.
