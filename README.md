# GDIM33 Vertical Slice
## Milestone 1 Devlog

1. The player movement graph is one of the core Visual Scripting graphs in my game. It runs on the On Update event and handles both movement and animation every frame. Two Get Axis nodes read the Horizontal and Vertical inputs, which are combined into a Vector3 and passed through Transform Direction so that movement always follows the direction the player is facing. The resulting direction vector is then fed into a Multiply node, where the speed value is determined by a Select node: if the player is holding Left Shift, the speed is set to 10 (running); otherwise it defaults to 5 (walking). This multiplied vector is passed into Character Controller Simple Move to physically move the player. At the same time, the movement vector is sent through Camera Get Main and Vector3 Magnitude to calculate how fast the player is actually moving, and that value is passed into Animator Set Float with the name "Speed" to drive the animation system, ensuring the correct animation plays depending on whether the player is walking, running, or idle.

2. My break-down has been updated to include two state machine systems. The first is the player movement state machine, implemented in the Visual Scripting movement graph, which switches between two states — Walking and Running — based on whether the player is holding Left Shift. This directly affects the Character Controller speed and the Animator float parameter, linking the movement system to both the physics and animation systems. The second is the flashlight state machine, implemented in C# code, which toggles between an On state and an Off state each time the player presses F. In the On state, the Light component is enabled, illuminating the environment; in the Off state, it is disabled, plunging the area into darkness.

These two state machines are closely related to other systems in the game. The movement state machine connects to the animation system through the Animator Set Float node, ensuring that walking and running animations match the player's actual speed, and it also indirectly relates to the audio system since footstep sounds are triggered based on the same axis input values. The flashlight state machine is related to the monster detection system, as monsters are designed to sense the flashlight's light and begin chasing the player when it is on — meaning the player must actively decide when to toggle the flashlight, balancing visibility against the risk of being detected.
   [New break down]<img width="1700" height="1080" alt="image" src="https://github.com/user-attachments/assets/e770e445-747d-4a40-8227-5a0b09561d81" />

## Milestone 2 Devlog
Milestone 2 Devlog goes here.
## Milestone 3 Devlog
Milestone 3 Devlog goes here.
## Milestone 4 Devlog
Milestone 4 Devlog goes here.
## Final Devlog
Final Devlog goes here.
## Open-source assets
- Cite any external assets used here!
