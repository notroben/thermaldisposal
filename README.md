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

## 5. Technical Architecture & Optimizations (Under the Hood)
The codebase has been heavily optimized for performance, scalability, and clean architecture, abandoning messy singletons and hardcoded logic in favor of modern C# design patterns.

### Data-Driven Progression (`DayData.cs`)
Instead of massive `if/else` chains checking `currentDay`, the game uses a `ScriptableObject` system (`DayData.cs`). Each day is its own asset file that dictates quotas, tool unlocks, broken conveyor states, and specific trash spawning rules. The `GameManager` simply reads the current `DayData` asset, making it incredibly easy for designers to tweak day balances without touching code.

### Service Locator Pattern (`ServiceLocator.cs`)
To eliminate the severe performance overhead of `FindFirstObjectByType<>` calls scattered across scripts, a lightweight Dependency Injection container (`ServiceLocator.cs`) is used. Core managers like the `GameManager` and `PlayerController` register themselves on `Awake()`, granting instant, zero-cost access to global state.

### Event-Driven Systems (`GameEvents.cs`)
Object-to-object coupling has been severed using a central C# `Action` Event Bus. 
- **Example:** The `PlayerInteraction` script doesn't know the `FurnaceLogic` exists. When the player clicks the furnace switch, it simply invokes `GameEvents.OnFurnaceActivated()`. The furnace listens for this event and activates itself.

### Consolidated Infractions (`RuleBreak.cs`)
All game-over triggers are centralized in a static constants class (`RuleBreak.cs`). This enforces a uniform, dystopian "HR System" tone across the entire game (e.g., `INFRACTION: Documentation logged out of chronological sequence.`). It also drastically simplifies text editing and future localization.

### Optimized Raycasting & Physics
- **Single-Raycast Interaction:** `PlayerInteraction.cs` fires only **one** raycast per frame, caching the hit. Hover states, pickup logic, and tool usage all share this single cached hit, saving immense CPU overhead.
- **Kinematic Conveyor Belts:** `ConveyorBelt.cs` avoids the notoriously heavy `OnCollisionStay` polling method. Instead, it uses a Kinematic Rigidbody trick—modifying `rb.position` and `rb.MovePosition` in `FixedUpdate`—forcing Unity's native physics engine to calculate frictionless, perfectly smooth movement for all resting bags at virtually zero cost.
