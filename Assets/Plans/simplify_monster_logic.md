# Project Overview
- Game Title: Hospital Horror
- High-Level Concept: Simplified monster AI that prioritizes visual aggression over physics-based animation matching.

# Problem Diagnosis
Physics-based animation checks (velocity) cause the monster to flicker into 'Idle' during minor NavMesh glitches or path recalculations, creating a "getting caught" or "stuttering" feeling.

# Implementation Steps
1. **Simplify `MonsterChase.cs`**:
    - Remove velocity-based `isMoving` checks.
    - Set animation states based solely on `distance <= detectionRange`.
    - Within detection range, toggle between `Walk` and `Dash` based on `dashRange`.
    - Keep `isResetting` logic for the "return to base" functionality, but ensure it also uses state-based animation.
2. **Configure NavMeshAgent**:
    - Ensure very high acceleration to eliminate the "coming slowly" feeling.

# Verification & Testing
1. Monster should never play Idle animation while the player is within 15m (detectionRange).
2. Transition from Walk to Dash should be purely distance-based.
3. Returning to base should play Walk animation until arrived.
