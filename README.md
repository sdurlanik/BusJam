Project Architecture Overview
================================

1. Core Architecture Principles

-------------------------------

### Dependency Injection (Zenject)

All managers and controllers receive their dependencies via constructor injection. No usage of `new` or `GetComponent`. Zenject handles lifecycle and bindings, ensuring modular and isolated components.

### Signal-Based Communication

Inter-system communication uses **Zenject's `SignalBus`** instead of direct method calls. For example, when a bus is full, `BusController` fires a `BusFullSignal`. Listeners like `GameStateManager` respond accordingly.

This results in:

* Loosely coupled systems

* Easier testing and refactoring

* Scalable architecture

* * *

2. Key Design Patterns Used

---------------------------

### Singleton (via Zenject)

Managers like `GameStateManager`, `BusSystemManager`, etc., are registered as singletons using `.AsSingle()` in installers.

### Observer (via SignalBus)

A modern take on the observer pattern using Zenject's signal system:

* **Publishers** fire signals (e.g., `GameOverSignal`)

* **Subscribers** react accordingly

### State Pattern

Used in:

* `UIStateMachine` (e.g., `UIStartState`, `UIGameplayState`)

* Flags like `IsAcceptingPassengers` inside `BusController`

### Factory Pattern

Factories like `CharacterFactory`, `BusFactory`, and `ObstacleFactory` encapsulate object creation logic, keeping instantiation separate from game flow logic.

### MVC (Model-View-Controller)

| Role       | Example Classes                                |
| ---------- | ---------------------------------------------- |
| Model      | `BusModel`, `LevelSO`                          |
| View       | `CharacterView`, `BusView`                     |
| Controller | `BusController`, `CharacterMovementController` |

* * *

3. System Responsibilities (Single Responsibility Principle)

------------------------------------------------------------

### Core Managers

* **`GameStateManager`** – Handles win/lose/game over logic.

* **`BusSystemManager`** – Manages bus spawning and dispatching.

* **`GridSystemManager`** – Handles main grid and waiting area creation/reset.

* **`LevelProgressionManager`** – Manages level transitions and progression.

* **`CameraController`** – Dynamically positions the camera.

### Object Controllers

* **`CharacterMovementController`** – Handles clicks, pathfinding, movement orchestration.

* **`BusController`** – Manages a single bus’s behavior.

### Utility Services

* **`IPathfindingService`** – Handles grid-based pathfinding.

* **`IMovementTracker`** – Tracks active movements for state checks.

* **`HapticsManager`** – Centralized vibration/feedback handler.

* * *

4. Data Management – `ScriptableObject`-Based

---------------------------------------------

* `LevelSO`, `LevelProgressionSO`, grid/bus configs are all stored as **ScriptableObjects**.

* Benefits:
  
  * Designers can create/edit levels directly in Unity Editor.
  
  * Clear separation between data and logic.
  
  * Efficient memory usage in builds.

* * *

5. Custom Editor Tools

----------------------

### Level Editor (`LevelEditor`, `LevelEditor_CustomEditor`)

Custom in-editor tool for visual level creation:

* Grid, characters, and obstacles can be placed directly in the Scene view.

* Real-time camera framing mimics gameplay perspective.

* Designers can build and preview levels without running the game.
