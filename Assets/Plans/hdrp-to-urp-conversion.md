# Project Overview
- Game Title: N/A (General Conversion Task)
- High-Level Concept: Convert an existing HDRP scene or project to the Universal Render Pipeline (URP) in Unity 6.
- Players: Single-player (General context)
- Inspiration / Reference Games: N/A
- Tone / Art Direction: Depends on original HDRP scene, goal is to match it as closely as possible within URP's capabilities.
- Target Platform: Standalone (MacOS/PC)
- Screen Orientation / Resolution: Landscape
- Render Pipeline: URP (Converted from HDRP)

# Game Mechanics
(N/A for conversion task)

# UI
(N/A for conversion task)

# Key Asset & Context
- Current Project Active Pipeline: URP (`PC_RPAsset`)
- Source Assets: HDRP Scenes, Materials, and Shaders.
- Unity Version: 6000.4.1f1

# Implementation Steps
(Note: Converting HDRP to URP is a manual process as there is no official "HDRP to URP" automatic converter. It's considered a "downgrade" in terms of feature set complexity.)

1. **Backup and Prerequisites**
    - Ensure the project is backed up.
    - Confirm the Universal RP package is installed in the Package Manager.

2. **Render Pipeline Setup**
    - Ensure a `Universal Render Pipeline Asset` exists (currently `Assets/Settings/PC_RPAsset.asset`).
    - Verify `Project Settings > Graphics > Scriptable Render Pipeline Settings` is set to this URP asset.
    - Verify `Project Settings > Quality > Render Pipeline Asset` is set to this URP asset.

3. **Material Conversion**
    - Open `Window > Rendering > Render Pipeline Converter`.
    - Select **Built-in to URP** (Note: Since there is no HDRP-specific converter, you may need to manually switch HDRP Shaders to URP Shaders).
    - **Manual Shader Switch**: For HDRP Lit materials, change the shader to `Universal Render Pipeline/Lit`. You may need to re-assign textures if naming conventions differ.
    - Check for "Pink Materials" and resolve them by assigning appropriate URP shaders.

4. **Lighting and Volume Adjustments**
    - **Volume System**: HDRP's Volume overrides (like `HD Shadow Settings`, `Physical Camera`, etc.) will not work in URP. Remove HDRP-specific overrides and add URP Volume overrides (`Bloom`, `Tonemapping`, `Vignette`, etc.).
    - **Lights**: HDRP uses physical light units (Lux, Candela). URP uses different units or scaled versions. Adjust Light intensities.
    - **Shadows**: URP has different shadow settings in the URP Asset. Re-tune shadow distance and resolution.

5. **Camera Conversion**
    - Unity should automatically attach `Universal Additional Camera Data` to cameras when URP is active, but check for any missing references or errors on existing cameras that previously had HDRP Camera components.

6. **Post-Processing**
    - Enable "Post Processing" on the Camera.
    - Configure the `Global Volume` with URP-compatible overrides.

# Verification & Testing
- **Visual Check**: Walk through the scene to ensure materials look correct (no pink textures).
- **Lighting Check**: Ensure the scene isn't too dark or blown out due to unit differences.
- **Performance**: Monitor FPS to see the performance gains from switching to URP.
- **Console Errors**: Check for any missing scripts or shader compilation errors related to HDRP.
