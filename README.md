# Baran_Taylan_CarGame

This game developed with **Unity 2019.4.8f1** for a prototype. 

Prototype built compatible to level designing.

There are APK, Windows and Linux builds under releases. 
You can use Left and Right arrow for rotation in non-mobile platforms. 

You can use **Left and Right** arrow keys for controlling the car.

## Classes

**TurnPointInfo** -> Level design focused class and prefab for designing entrance, exit point and car position in scene.

**LevelManager** -> All settings are stored and updated by LevelManager class s.t. car speed, move precision etc.

**LevelEvents** -> All game events stored in LevelEvents class since it is a small project.

**CarSoftware** -> Player and Bot car script, does all car movement, rotation, collision, record/replay.

**CarRotate classes** -> User input classes for mouse, touch and keyboard.

## Record/Replay Implementation

I implemented record/replay system based on input recording since it uses very much less ram and feels real-like movement.
For implementing record and replay system, I did not use any fps or physics based components and methods to develop a **deterministic** gameplay.
Because of that movement and rotation, even if **rotation input is is used by IEnumerator's and Coroutines** with waiting as long as **MovementPrecision** value given by LevelManager.
