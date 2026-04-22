# Project Overview
- Game Title: Hospital Escape
- High-Level Concept: Horror/Escape game.
- Render Pipeline: URP.
- UI System: uGUI (Consistent with existing OpeningUI and EndingUI).

# Game Mechanics
- Players interact with items (Keys) and triggers (Exit).
- Notification system provides feedback to the player.

# UI
- **HUD Canvas**: A screen-space overlay canvas for in-game UI.
- **Notification Panel**: Located at the bottom center of the screen.
- **Notification Text**: Modular text output component.

# Key Asset & Context
- **NotificationManager.cs**: A singleton script to handle showing/hiding messages.
- **HUD_Canvas Prefab**: Containing the UI structure.
- **KeyPickup.cs**: Will be updated to call the notification.

# Implementation Steps
1. **Create NotificationManager Script**:
    - Implement a `ShowNotification(string message, float duration)` method.
    - Include simple fade-in/out or timer-based visibility.
2. **Setup UI in Scene**:
    - Create a **Canvas** named `HUD_Canvas`.
    - Add a **Panel** (transparent) and a **Text** element at the bottom center.
    - Attach `NotificationManager` to the Canvas or a child.
3. **Update KeyPickup.cs**:
    - Add a call to `NotificationManager.Instance.ShowNotification("Key obtained")` inside `CollectKey()`.
4. **Generalize for other Triggers**:
    - (Optional/Future-proof) The same system can be used for "Door is locked", "Need a key", etc.

# Verification & Testing
- Pickup the key.
- Verify the "Key obtained" text appears at the bottom.
- Verify it disappears after the specified duration.
