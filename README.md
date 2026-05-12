# Thermal Disposal - Game Design & Technical Documentation

**Genre:** First-Person Psychological Horror  
**Platform:** PC (Windows)  
**Engine:** Unity 6.3 LTS  
**Mode:** Single Player  

---

## 1. Game Overview & High Concept
*Thermal Disposal* is a first-person simulation where the player acts as a waste disposal operator in a brutalist, underground industrial facility. What begins as a repetitive, mundane job of processing trash bags on a conveyor belt slowly descends into psychological and cosmic horror as the "trash" becomes increasingly disturbing.

The core unique selling point is **Moral Complicity**. The game forces the player to actively participate in horrific acts (incinerating organic matter and human remains) simply to fulfill their daily corporate quota and avoid termination. A stylized retro low-poly aesthetic is used to obscure gruesome details, forcing the player's imagination to fill in the terrifying gaps.

---

## 2. Story & Narrative
Set in a dystopian future, the nameless protagonist is employed by the mysterious *The Company*. 
- **The Setup:** The player is given a simple task: weigh trash, document it, and burn it. 
- **The Escalation:** Slowly, anomalies appear. Trash bags twitch, scanners detect organic matter, and the furnace emits human-like screams. The Company strictly orders the player to ignore these "escaping trapped gases."
- **The Climax:** Once the player has served their purpose and processed all the horrifying evidence, The Company has no more use for them. The final task requires the player to willingly step into the furnace themselves to complete their contract.

---

## 3. Core Gameplay Loop
The player must survive a 7-day work contract. A single mistake, skipped step, or falsified record results in a fatal "Rule Break" flagged by the HR System, leading to execution.

**The Loop:**
1. **Receive:** A trash bag arrives via the Conveyor Belt.
2. **Weigh:** The bag is placed on the Weight Scale to determine if it is OPTIMAL or OVER_CAPACITY.
3. **Document (Check):** The player uses the Paper & Clipboard to check off that the bag has been weighed.
4. **Incinerate:** The bag is thrown into the massive Furnace.
5. **Document (Cross Out):** The player crosses the task off the clipboard to finalize the disposal.
6. **Clock Out:** Once the daily quota is met, the player interacts with the Exit Door to end the shift.

**Diegetic UI & Physical Inventory:**
The game features zero traditional HUD elements. All UI is diegetic—meaning it exists physically in the game world. The player can only hold one item at a time (Physical Inventory). To know their tasks, they must physically hold the Clipboard. To know a bag's contents, they must hold the Scanner.

---

## 4. Day-by-Day Progression
The game utilizes a Day-by-day progression system, introducing new mechanics and narrative horrors each shift:

| Day | Quota | Narrative & Mechanical Escalation |
| :--- | :--- | :--- |
| **1** | 10 Bags | Introduction to the core loop. Bags feel heavy and sound "wet." |
| **2** | 10 Bags | **Scanner Unlocked.** Bags show "no metal", but bag #9 twitches and scans as "organic matter detected." |
| **3** | 10 Bags | **Conveyor Broken.** Player must manually retrieve bags from a dark corner where monster/human screams can be faintly heard. |
| **4** | 10 Bags | Directive: "Ignore acoustics, they are trapped gases." 3 bags scan as organic and scream horrifically when incinerated. |
| **5** | 10 Bags | **Bag Opening Mechanic.** 5 bags are "OVER_CAPACITY". Player must open them (revealing raw meat mixed with household items like shoes and toothbrushes) and repackage them. |
| **6** | 10 Bags | **Iron Poker Unlocked.** Incomplete combustion occurs. Player must use the poker to manually push charred debris (including human limbs) deep into the flames. |
| **7** | 1 Task | **The Final Shift.** Only 1 task on the clipboard. The conveyor is empty. The only object in the room that reads "OPTIMAL" on the scale is the Player themselves. |

---

## 5. Technical Architecture & Component Map

The codebase has been heavily optimized for performance, scalability, and clean architecture, utilizing modern C# design patterns to ensure decoupling and efficiency.

### Core Managers & Systems
- **`GameManager.cs`**
  - **Role**: The central state machine. Tracks quotas, execution states, and honesty stats (`physicalWeighedCount`, `uiCheckedCount`, `uiCrossedOutCount`).
  - **Relationships**: Serves as the global authority. Receives infraction reports from other scripts to trigger Game Over states.
- **`DayData.cs` (ScriptableObject)**
  - **Role**: Data container for day-specific progression.
  - **Relationships**: Queried by the `GameManager` and `TrashSpawner` to dictate daily quotas, tool unlocks, broken conveyor states, and trash spawning rules without hardcoded logic.
- **`ServiceLocator.cs`**
  - **Role**: A Dependency Injection container.
  - **Relationships**: `GameManager` and `PlayerController` register themselves here on `Awake()`, granting other scripts instant, zero-cost access to global state, eliminating expensive `FindObject` calls.
- **`GameEvents.cs`**
  - **Role**: A central C# `Action` Event Bus.
  - **Relationships**: Severs object-to-object coupling. For example, `PlayerInteraction` invokes `OnFurnaceActivated()`, and `FurnaceLogic` listens and activates itself without either script knowing about the other.
- **`RuleBreak.cs`**
  - **Role**: A static constants class centralizing all Game Over triggers and "HR System" logging.
  - **Relationships**: Used by all interactable objects to enforce a uniform, dystopian tone (e.g., `INFRACTION: Documentation logged out of chronological sequence.`).

### Player Systems
- **`PlayerController.cs`**
  - **Role**: Handles first-person movement, looking, gravity, and crouching.
  - **Relationships**: Registers to `ServiceLocator`. Exposes state locks (`canLook`, `canMove`) used by tools like `ClipboardTool`.
- **`PlayerInteraction.cs`**
  - **Role**: Handles raycast-based environment interaction (picking up physics objects, toggling switches, using tools).
  - **Relationships**: Highly optimized to fire only **one raycast per frame**, caching the hit for hover states, pickup logic, and tool usage simultaneously, saving immense CPU overhead.

### Interactable Environment
- **`FurnaceLogic.cs`**
  - **Role**: The incineration zone. Tracks objects that enter/exit its trigger and processes burning.
  - **Relationships**: Listens to `GameEvents` to open/close doors or activate. Fires `RuleBreak` events to the `GameManager` if unweighed bags or essential tools are destroyed.
- **`WeightScaleReader.cs`**
  - **Role**: A trigger zone that reads the `TrashBag_data` of whatever is placed on it and updates the diegetic UI display.
  - **Relationships**: Updates `GameManager`'s physical weigh counts and ensures correct chronological sequence.
- **`ConveyorBelt.cs`**
  - **Role**: Moves bags into the room.
  - **Relationships**: Uses a highly performant **Kinematic Rigidbody trick**—modifying `rb.position` and `rb.MovePosition` in `FixedUpdate`—forcing Unity's native physics engine to calculate frictionless movement for resting bags at virtually zero cost (abandoning heavy `OnCollisionStay` polling).
- **`ExitDoor.cs`**
  - **Role**: The shift-end mechanic.
  - **Relationships**: Verifies quotas and paperwork with `GameManager` before advancing the day or triggering the Execution scene.

### Tools & Data
- **`TrashBag_Data.cs`**: Data container on bags tracking `WeightCategory`, `isOrganic`, `hasMetal`, and interaction states (`isOpen`, `isSealed`, `hasBeenWeighed`).
- **`TrashSpawner.cs`**: Timer-based spawner that reads `DayData` to instantiate specific bags at specific intervals.
- **`ScannerTool.cs`**: Raycasts forward to reveal hidden bag data (Metal/Organic) to a UI screen.
- **`ClipboardTool.cs` & `ClipboardTask.cs`**: Manages the diegetic UI clipboard, tracking daily tasks and interacting with the `GameManager`'s honesty tracking.
- **`IronPoker.cs`**: Used to manually destroy `DebrisLogic` objects left behind during incomplete combustion.
