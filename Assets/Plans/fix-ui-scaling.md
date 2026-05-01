# Project Overview
- Game Title: vanz_ai_hackathon1
- High-Level Concept: Opening scene UI fix for high-resolution monitors.
- Players: Single player
- Render Pipeline: URP (PC_RPAsset)
- Target Platform: StandaloneOSX
- Screen Orientation / Resolution: Landscape (Reference 1920x1080)

# Game Mechanics
## UI Scaling
The UI currently uses `ConstantPixelSize`, which makes it appear very small on high-resolution monitors. The plan is to switch to `ScaleWithScreenSize` and adjust the dimensions of the title and button elements to look proportional on a 1080p reference resolution.

# UI
- Canvas: The root UI container.
- GameTitle: A large image element at the top center.
- StartButton: A button element below the title.

# Key Asset & Context
- `Assets/Scenes/OpeningScene.unity`: The scene containing the UI.
- `CanvasScaler` (Component ID: 419204): Needs mode change and reference resolution setup.
- `GameTitle` (RectTransform ID: 419242): Needs size increase.
- `StartButton` (RectTransform ID: 419188): Needs size increase.
- `Text` (Text component ID: 419218): Needs font size increase.

# Implementation Steps
1. **Modify CanvasScaler**:
   - Change `m_UiScaleMode` to `1` (`ScaleWithScreenSize`).
   - Set `m_ReferenceResolution` to `(1920, 1080)`.
   - Set `m_ScreenMatchMode` to `0` (`MatchWidthOrHeight`).
   - Set `m_MatchWidthOrHeight` to `0.5`.
2. **Resize GameTitle**:
   - Update `GameTitle` `RectTransform.m_SizeDelta` to `(1200, 600)`.
3. **Resize StartButton**:
   - Update `StartButton` `RectTransform.m_SizeDelta` to `(400, 120)`.
4. **Update Button Text**:
   - Update `StartButton/Text` `Text.m_FontData.m_FontSize` to `48`.

# Verification & Testing
1. **Editor Play Mode**: Test the scene in different Game View resolutions (e.g., 1920x1080, 2560x1440, 3840x2160).
2. **Visual Check**: Ensure the title and button are clearly visible and centered.
3. **Functionality Check**: Ensure the "START GAME" button is still clickable and text is centered within the button.
