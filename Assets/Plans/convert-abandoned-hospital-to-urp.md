# Project Overview
- Game Title: N/A (Project transition task)
- High-Level Concept: Convert the "Abandoned Hospital" environment from HDRP to URP following specific package instructions.
- Players: N/A
- Inspiration / Reference Games: N/A
- Tone / Art Direction: Horror Environment
- Target Platform: StandaloneOSX (as per current project settings)
- Render Pipeline: Universal Render Pipeline (URP) - Project is already configured for URP.

# Game Mechanics
N/A - This is an asset management/pipeline conversion task.

# UI
N/A

# Key Asset & Context
- **Source Package**: `Assets/HE_HDRP_AbandonedHospital/URP/URP_HE_AbandonedHospital.unitypackage`
- **Current HDRP Directory**: `Assets/HE_HDRP_AbandonedHospital`
- **Target URP Directory**: `Assets/HE_URP_AbandonedHospital` (will be created upon import)
- **Target Package Location**: `Assets/URP_HE_AbandonedHospital.unitypackage` (temporary)

# Implementation Steps
1. **Automated Conversion Script**: I will create an Editor script `Assets/Editor/AbandonedHospitalURPConverter.cs` to automate the file operations.
2. **Move Package**: The script will move `URP_HE_AbandonedHospital.unitypackage` to the root `Assets` folder to prevent it from being deleted in the next step.
3. **Delete HDRP Assets**: The script will recursively delete the `Assets/HE_HDRP_AbandonedHospital` directory.
4. **Trigger Import**: The script will trigger `AssetDatabase.ImportPackage` for the URP package. This will open the Unity Import Package dialog.
5. **Final Cleanup**: Once the user has imported the assets, they should delete the `.unitypackage` file from `Assets/` to optimize project size.

# Verification & Testing
1. **Script Validation**: Ensure the script correctly identifies the source package path.
2. **Path Verification**: Verify the package is moved outside before the directory is deleted.
3. **Import Check**: Confirm the `HE_URP_AbandonedHospital` directory is created and assets (materials/shaders) are URP-compatible (not pink).
4. **Manual Check**: Open the example scene in `HE_URP_AbandonedHospital` to verify visual correctness.
