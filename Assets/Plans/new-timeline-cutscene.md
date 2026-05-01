# Project Overview
- Game Title: Hospital Scene Cutscene
- High-Level Concept: Setting up a new Timeline cutscene environment for manual camera framing.
- Players: Single player (contextual)
- Render Pipeline: URP (PC_RPAsset)
- Unity Version: 6000.4.1f1

# Game Mechanics
## Core Gameplay Loop
- Cinematic storytelling via Timeline.
## Controls and Input Methods
- Manual camera positioning in the Unity Editor.

# UI
- N/A (Cutscene setup focus)

# Key Asset & Context
- **Timeline Asset**: `Assets/Timeline/New_Cutscene.playable`
- **Manager GameObject**: `New_Cutscene_Manager` (with `PlayableDirector`)
- **Virtual Cameras**: `Vcam_Cutscene_01`, `Vcam_Cutscene_02` (Cinemachine cameras)
- **Main Camera**: Uses `CinemachineBrain` to switch between virtual cameras.

# Implementation Steps
1. **Create Timeline Asset**:
   - Create a new Timeline asset at `Assets/Timeline/New_Cutscene.playable`.
2. **Setup Scene Manager**:
   - Create a new GameObject named `New_Cutscene_Manager`.
   - Add a `PlayableDirector` component.
   - Assign the `New_Cutscene.playable` asset to the `PlayableDirector`.
3. **Create Virtual Cameras**:
   - Create a child GameObject `Cameras` under `New_Cutscene_Manager`.
   - Create two GameObjects: `Vcam_Cutscene_01` and `Vcam_Cutscene_02`.
   - Add `CinemachineCamera` components to each.
   - Add `CinemachinePositionSource` (or just rely on Transform) and `CinemachineRotationComposer` if needed, but per Cinemachine 3.1 instructions, I'll ensure they have basic components for manual control.
4. **Configure Timeline Tracks**:
   - Add a `Cinemachine Track` to the `New_Cutscene.playable`.
   - Bind the `Main Camera` (the one with `CinemachineBrain`) to this track.
   - Add two `Cinemachine Shot` clips to the track.
   - Bind `Vcam_Cutscene_01` to the first clip and `Vcam_Cutscene_02` to the second clip.
5. **Organization**:
   - Move the manager under the `CutScenes` parent object in the hierarchy to maintain project structure.

# Verification & Testing
1. **Timeline Window**: Open the Timeline window with `New_Cutscene_Manager` selected.
2. **Binding Check**: Verify the Cinemachine Track is bound to the Main Camera.
3. **Clip Check**: Verify that scrubbing the timeline switches between `Vcam_Cutscene_01` and `Vcam_Cutscene_02`.
4. **Editor Framing**: Ensure the user can select the virtual cameras and use the "Align with View" or move them in the scene to set the framing.
