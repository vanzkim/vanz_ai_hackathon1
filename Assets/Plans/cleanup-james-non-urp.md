# Project Overview
- **Game Title:** James URP Cleanup
- **High-Level Concept:** Optimize the project by removing unused HDRP and Built-in assets for the "James" character while preserving URP functionality.
- **Render Pipeline:** URP (Universal Render Pipeline)

# Implementation Plan: Cleanup HDRP/Built-in Assets
James's asset pack contains materials and prefabs for all three render pipelines. To save space, we will remove the non-URP assets. **Note:** Analysis shows that URP prefabs currently have dependencies on certain HDRP assets (specifically the Body materials and base prefabs), so we must be careful not to break them.

## Key Asset & Context
- **URP Assets (Keep):**
  - `Assets/David Grette/Adventure_Pack/Characters/James/Materials/URP_Materials/`
  - `Assets/David Grette/Adventure_Pack/Characters/James/Prefabs/URP/`
  - `Assets/David Grette/Scenes/James_URP.unity`
  - All Source files: `FBX/`, `Textures/`, `Scripts/`, `Shader/SubGpaph/`
- **Dependencies to Extract/Preserve:**
  - `Assets/David Grette/Adventure_Pack/Characters/James/Materials/HDRP_Materials/Body/` (Referenced by URP James)
  - `Assets/David Grette/Adventure_Pack/Characters/James/Prefabs/HDRP/James/` (URP prefabs are variants of these)

## Implementation Steps
1. **Dependency Analysis (Verification)**
   - Confirm that `James_Player_Armature` (URP) still references `SK_James_Chan` (HDRP).
   - Confirm that the James Body (Head, Torso, Legs) only has materials in the HDRP folder.

2. **Relocate Essential HDRP Assets**
   - Create a new folder: `Assets/David Grette/Adventure_Pack/Characters/James/Materials/Shared_Materials/Body`.
   - Move all materials from `Assets/David Grette/Adventure_Pack/Characters/James/Materials/HDRP_Materials/Body/` to the new `Shared_Materials/Body` folder. This prevents them from being deleted in the next step.
   - (Optional but recommended) "Unpack" the URP prefabs or point them directly to the FBX models to remove the dependency on the HDRP prefabs. If not done, the HDRP prefabs folder must be kept.

3. **Delete Unused Pipeline Folders**
   - Delete the folder: `Assets/David Grette/Adventure_Pack/Characters/James/Materials/Built-in_Materials/`.
   - Delete the folder: `Assets/David Grette/Adventure_Pack/Characters/James/Prefabs/Built-in/`.
   - Delete the remaining subfolders in `Assets/David Grette/Adventure_Pack/Characters/James/Materials/HDRP_Materials/` (Accessories, Cloth, etc.).
   - Delete the folder: `Assets/David Grette/Adventure_Pack/Characters/James/Prefabs/HDRP/` (ONLY if prefabs have been re-parented or dependencies removed).

4. **Delete Unused Scenes and Resources**
   - Delete `Assets/David Grette/Scenes/James_Built_in.unity`.
   - Delete `Assets/David Grette/Scenes/James_HDRP.unity`.
   - Delete `Assets/David Grette/Scenes/HDRPDefaultResources/` (These are default HDRP settings not needed for URP).

5. **Texture Optimization**
   - Identify if specific skin folders in `Textures/` (e.g., `Horror`, `Snow`) are intended to be used. If the user only wants the "Original" James, the other texture folders can be removed to save significant space.

## Verification & Testing
- Open `Assets/David Grette/Scenes/James_URP.unity`.
- Select the James character in the hierarchy.
- Verify the **Mesh Renderer** materials are all loaded (not pink).
- Check the **Animator** and verify James moves correctly.
- Ensure no "Missing Prefab" errors appear in the Project window.
