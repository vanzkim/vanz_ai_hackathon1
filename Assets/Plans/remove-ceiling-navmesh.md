# Project Overview
- **Game Title**: Hospital Project
- **Render Pipeline**: URP (Universal Render Pipeline)
- **Navigation System**: Unity AI Navigation (NavMeshSurface)
- **Lighting System**: Likely using APV (Adaptive Probe Volumes) or Baked GI.

# Problem
Ceiling objects are being included in the NavMesh bake because `NavMeshSurface` components are set to collect all objects. The user is concerned that disabling "Static" flags will affect GI (Global Illumination).

# Solution
Use the `NavMeshModifier` component to exclude ceilings from NavMesh **without** touching the Static flags. This ensures that the objects still contribute to Global Illumination (GI) and APV while being ignored by the Navigation system.

# Key Assets & Context
- **Ceiling Prefabs**:
  - `Assets/HE_URP_AbandonedHospital/Prefabs/Brushes/roundCeiling.prefab`
  - `Assets/HE_URP_AbandonedHospital/Prefabs/Brushes/roundCeiling_cap.prefab`
  - `Assets/HE_URP_AbandonedHospital/Prefabs/Brushes/roundCeiling_cap_2s.prefab`
  - `Assets/HE_URP_AbandonedHospital/Prefabs/Brushes/roundCeiling_cross.prefab`
  - `Assets/HE_URP_AbandonedHospital/Prefabs/Brushes/roundCeiling_skylight.prefab`
- **Constraint**: Must keep `Contribute GI` static flag enabled for proper APV/Lighting.

# Implementation Steps

## Step 1: Add NavMeshModifier to Prefabs
1. Open the following ceiling prefabs:
   - `roundCeiling.prefab`
   - `roundCeiling_cap.prefab`
   - `roundCeiling_cap_2s.prefab`
   - `roundCeiling_cross.prefab`
   - `roundCeiling_skylight.prefab`
2. Add the `NavMeshModifier` component.
3. Enable the **Ignore From Build** option.
4. **IMPORTANT**: Do not modify any `Static` flags (ensure `Contribute GI` remains checked).

## Step 2: Handle Non-Prefab Ceilings (if any)
1. Search the scene for any other objects named "ceiling" that might not be instances of the above prefabs.
2. Apply the same `NavMeshModifier` with **Ignore From Build** enabled.

## Step 3: Re-bake NavMesh
1. Select a `NavMeshSurface` in the scene.
2. Click **Bake**.
3. Since many surfaces exist, if they are redundant, they should all be updated. (Note: Modifying prefabs will automatically affect all instances when their respective surfaces are baked).

# Verification & Testing
- **Navigation Check**: Verify the blue NavMesh is gone from the ceiling in the Scene View.
- **Lighting Check**: Verify the ceiling still receives/contributes to lighting (GI/APV) as before.
- **AI Check**: Ensure agents can still walk on the floors correctly.
