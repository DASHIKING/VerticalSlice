# GDIM33 Vertical Slice
## Milestone 1 Devlog

1. The player movement graph is one of the core Visual Scripting graphs in my game. It runs on the On Update event and handles both movement and animation every frame. Two Get Axis nodes read the Horizontal and Vertical inputs, which are combined into a Vector3 and passed through Transform Direction so that movement always follows the direction the player is facing. The resulting direction vector is then fed into a Multiply node, where the speed value is determined by a Select node: if the player is holding Left Shift, the speed is set to 10 (running); otherwise it defaults to 5 (walking). This multiplied vector is passed into Character Controller Simple Move to physically move the player. At the same time, the movement vector is sent through Camera Get Main and Vector3 Magnitude to calculate how fast the player is actually moving, and that value is passed into Animator Set Float with the name "Speed" to drive the animation system, ensuring the correct animation plays depending on whether the player is walking, running, or idle.

2. My break-down has been updated to include two state machine systems. The first is the player movement state machine, implemented in the Visual Scripting movement graph, which switches between two states — Walking and Running — based on whether the player is holding Left Shift. This directly affects the Character Controller speed and the Animator float parameter, linking the movement system to both the physics and animation systems. The second is the flashlight state machine, implemented in C# code, which toggles between an On state and an Off state each time the player presses F. In the On state, the Light component is enabled, illuminating the environment; in the Off state, it is disabled, plunging the area into darkness.

   These two state machines are closely related to other systems in the game. The movement state machine connects to the animation system through the Animator Set Float node, ensuring that walking and running animations match the player's actual speed, and it also indirectly relates to the audio system since footstep sounds are triggered based on the same axis input values. The flashlight state machine is related to the monster detection system, as monsters are designed to sense the flashlight's light and begin chasing the player when it is on — meaning the player must actively decide when to toggle the flashlight, balancing visibility against the risk of being detected.

   <img width="1700" height="1080" alt="image" src="https://github.com/user-attachments/assets/e770e445-747d-4a40-8227-5a0b09561d81" />
   I use AI to help me draw this new breakdown
## Milestone 2 Devlog

1.The feature I am building is the handcart and collection point system, which allows the player to drag a cart around the museum, place collected items into it, and submit those items at a collection point to progress toward escaping.
Step 1: Setting Up the Handcart (Least Complex)
The goal is to make the handcart a physical object that the player can drag around the scene.

Substep 1: Add a Rigidbody and Box Collider to the handcart GameObject, and set the Tag to "Handcart".
Substep 2: Create HandcartController.cs and attach it to the handcart to store a list of items and track total value.
Substep 3: In HandcartController.cs, freeze the Y position and X/Z rotation in the Rigidbody constraints so the cart stays on the ground.
Substep 4: In PlayerInteraction.cs, add right-click logic to grab and drag the cart using MovePosition toward a target point in front of the player.
Test: Run the game and right-click the cart — if it smoothly follows the player around the scene without falling through the floor, this step is working.

Step 2: Placing Items Into the Cart (Medium Complex)
The goal is to allow the player to drop a held item into the cart when close enough, and have the cart track the total value.

Substep 1: In PlayerInteraction.cs, add a DetectNearbyCart() method that uses Physics.OverlapSphere to check if a cart is within range.
Substep 2: Modify the drop logic so that if a cart is nearby when the player releases an item, AddItem() is called on the HandcartController instead of just dropping it.
Substep 3: In HandcartController.AddItem(), set the item as a child of the cart, disable its Rigidbody gravity, and reset its velocity so it does not push the cart.
Substep 4: Add a World Space Canvas to the cart with a TextMeshPro object that displays the current total value of all items inside.
Test: Pick up an item, walk near the cart, and release it — if the item snaps onto the cart and the cart UI updates to show the correct total value, this step is working.

Step 3: Collection Point Submission (Most Complex)
The goal is to create a collection point that detects when the cart is pushed into it, checks if the total value meets the requirement, and removes the items if successful.

Substep 1: Create a CollectionPoint GameObject with a trigger Box Collider and attach CollectionPoint.cs to it.
Substep 2: Use OnTriggerEnter and OnTriggerExit in CollectionPoint.cs to detect when the cart enters or leaves the collection zone.
Substep 3: Display the required value on a World Space Canvas above the collection point, and update a status text in real time to show how much more value is needed.
Substep 4: When the player presses E inside the zone, call SubmitItems() on the HandcartController to destroy items up to the required value, and notify GameManager that one collection point is completed.
Substep 5: In GameManager.cs, track how many collection points are completed and show the escape button on the HUD when all points are finished.
Test: Push the cart into the collection zone with enough items and press E — if the items disappear, the zone marks itself as complete, and the GameManager updates the progress counter, this step is working.

2.The task break-down activity was helpful because it forced me to think about the feature in smaller, manageable pieces before writing any code, which made it easier to know what to work on next without feeling overwhelmed. However, my break-downs could be improved by being more specific about which scripts interact with each other and what the expected output of each step should be, so it would be easier to test whether each step was actually working before moving on.

3. In my game, I bridge Visual Scripting and C# code in the player movement and footstep audio system. The Visual Scripting graph on the Player handles the movement logic — it reads Horizontal and Vertical axis inputs using Get Axis nodes, combines them into a Vector3, passes it through Transform Direction so movement follows the camera, and feeds the result into Character Controller Simple Move. On the C# side, the FlashlightToggle.cs script exposes a static boolean variable IsFlashlightOn that is updated every time the player presses F to toggle the flashlight. This variable is then read directly by MonsterAI.cs and SanitySystem.cs in their Update loops — MonsterAI uses it to increase the monster's detection range when the flashlight is on, and SanitySystem uses it to determine whether the player's sanity should recover or continue decreasing. This hybrid approach works well because the movement input logic is simple and readable as a Visual Scripting graph, while the flashlight state needs to be shared across multiple C# systems simultaneously, which is much cleaner to handle as a static variable in code than to pass through graph nodes. The C# scripts involved are FlashlightToggle.cs, MonsterAI.cs, and SanitySystem.cs.


5. I would like to be graded on my NavMesh-based monster AI system, which can be found on any of the monster GameObjects in the scene — select a monster, and you will see the NavMesh Agent component along with the MonsterAI and MonsterStats scripts that drive the patrol, chase, attack, and lost states.
## Milestone 3 Devlog
Milestone 3 Devlog goes here.
## Milestone 4 Devlog
Milestone 4 Devlog goes here.
## Final Devlog
Final Devlog goes here.
## Open-source assets
- Cite any external assets used here!
