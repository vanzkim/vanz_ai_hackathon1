# Project Overview
- Game Title: Hospital Escape (Inferred from scene name)
- High-Level Concept: Horror/Escape game where the player collects keys to exit.
- Players: Single player.
- Render Pipeline: URP.
- Target Platform: Standalone.

# Game Mechanics
## Core Gameplay Loop
- Explore the hospital, find keys, avoid monsters, and escape through the exit trigger.
## Controls and Input Methods
- Third-person movement, interaction with triggers.

# UI
- HUD for inventory/keys (implied by PlayerInventory).

# Key Asset & Context
- **Keys (GameObject)**: The item player needs to collect. Currently missing Rigidbody, preventing `OnTriggerEnter`.
- **KeyPickup.cs**: Script responsible for handling key collection, sound, and destruction.
- **Trigger_WhiteExit (GameObject)**: The exit trigger. Also missing Rigidbody.
- **EndingTrigger.cs**: Script for ending the game.

# Implementation Steps
1. **Fix Physics Triggers**:
    - Add **Rigidbody** component to `Keys` GameObject.
    - Set Rigidbody to **Is Kinematic = true** and **Use Gravity = false**.
    - Add **Rigidbody** component to `Trigger_WhiteExit` GameObject (proactive fix for the same issue).
    - Set Rigidbody to **Is Kinematic = true** and **Use Gravity = false**.
2. **Refine KeyPickup.cs**:
    - Ensure the script correctly disables the visual mesh and collider immediately upon pickup.
    - Ensure `Destroy(gameObject, delay)` is called to remove the object after the sound plays.
    - *Note*: Current script already has this, but I will verify the `Collider` is also disabled to prevent multiple triggers.

# Verification & Testing
- Move player to `Keys` position.
- Verify "keys_rattling" sound plays.
- Verify `Keys` object disappears from the scene.
- Verify `PlayerInventory.Instance.hasKey` becomes true.
- Move player to `Trigger_WhiteExit`.
- Verify ending transition works.
