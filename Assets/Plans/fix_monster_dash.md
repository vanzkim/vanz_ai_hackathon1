# Project Overview
- Game Title: Hospital Horror (Assumed)
- High-Level Concept: Survival horror where a monster (GreyLady) chases the player.
- Players: Single player.
- Inspiration / Reference Games: Amnesia, Outlast.
- Render Pipeline: URP.
- Target Platform: PC/Mac.

# Problem Diagnosis
The monster `Monster_GreyLady` is not playing the `Dash` animation even when within range. It toggles between `Walk` and `Idle` repeatedly.
Causes:
1. `SetDestination` is called every frame, causing path recalculation jitter.
2. `NavMeshAgent` acceleration is low (12), leading to slow speed ramp-up.
3. `Auto Braking` is on, causing slowdown near the player.
4. `isMoving` logic in script relies on a thin margin of `velocity.magnitude > 0.1f`, which can fail during jitter.

# Implementation Steps
1. **Update `MonsterChase.cs`**:
    - Reduce `SetDestination` frequency by checking if the player has moved.
    - Improve `isMoving` logic to be more resilient to minor velocity drops.
    - Explicitly manage `autoBraking` state.
2. **Update `NavMeshAgent` Settings**:
    - Increase `acceleration` to 40.
    - Disable `autoBraking`.
3. **Verify Animator Controller**:
    - Ensure `Idle -> Dash` exists (already added).
    - Ensure `Walk -> Dash` is prioritized (already added).

# Key Asset & Context
- `Assets/Scripts/MonsterChase.cs`: Main movement script.
- `Monster_GreyLady` (Instance ID 603888): The target GameObject.

# Verification & Testing
1. Observe 몬스터 in the scene: It should transition to Dash animation immediately when within 5m.
2. Check Console for any NavMesh-related warnings.
3. Verify that the monster returns to base correctly after the player is caught.
