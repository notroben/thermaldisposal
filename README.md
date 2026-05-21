# Thermal Disposal

**Genre:** First-Person Simulation / Psychological Horror  
**Platform:** PC (Windows)  
**Engine:** Unity 6.3 LTS  
**Mode:** Single Player  

---

## Part 1: Game Overview

### Description

*Thermal Disposal* is a first-person simulation where you work as a waste disposal operator in a brutalist, underground industrial facility. What begins as a repetitive, mundane job of processing trash bags on a conveyor belt slowly descends into a strange and atmospheric mystery.

The core gameplay focuses on **Corporate Compliance**. You must actively complete each task precisely and in order to fulfill your daily quota and avoid termination by The Company's HR System. A stylized retro low-poly aesthetic builds the atmosphere, forcing your imagination to fill in the gaps of the facility's true purpose.

> **Note:** It is highly recommended to read the tutorial file DisposalProtocol.pdf (also available in-game) before playing to understand your daily tasks.

<!-- Add your screenshot paths here -->
<!-- ![Screenshot 1](path/to/image1.png) -->
<!-- ![Screenshot 2](path/to/image2.png) -->

### Features

- **7-Day Work Contract** — Survive a full week of shifts, each introducing new tools, mechanics, and escalating situations.
- **Fully Diegetic Interface** — Zero traditional HUD besides crosshair and prompt texts. All information exists physically in the game world: a clipboard for tasks, a scanner for analysis, and a weight scale for measurements.
- **Physical Inventory** — You can only hold one item at a time. Managing what you carry and when is part of the challenge.
- **Strict Procedural Compliance** — Every step must be completed in the exact order. Weigh, document, incinerate, document again. A single misstep triggers a fatal infraction.
- **Tool Progression** — New tools unlock across the week, each adding a layer to the gameplay loop.
- **Retro Low-Poly Aesthetic** — Stylized visuals that balance atmosphere with imagination.
- **Save/Load System** — 4 save slots with in-game screenshot thumbnails.
- **Fully Configurable Settings** — Mouse sensitivity, invert Y, volume controls, and fullscreen toggle.
- **In-Game Tutorial** — Slideshow-based tutorial accessible from the main menu and pause menu.

### Controls

| Input | Action |
| :--- | :--- |
| `WASD` | Move |
| `Mouse` | Look |
| `Space` | Jump |
| `Left Ctrl` / `C` | Toggle Crouch |
| `Left Click` | Interact / Use Tool / Pick Up |
| `Right Click` | Drop Held Item |
| `Left Click` (on clipboard) | Focus/Unfocus Clipboard |
| `ESC` | Pause Menu |

---

## Part 2: Technical Documentation (Contains Spoilers)

> **WARNING: The following sections contain heavy spoilers regarding the game's narrative, mechanics, and progression. Read at your own risk.**

---

### 1. Story & Narrative

Set in a dystopian future, the nameless protagonist is employed by the mysterious *The Company*.

- **The Setup:** The player is given a simple task: weigh trash, document it, and burn it.
- **The Escalation:** Slowly, anomalies appear. Trash bags twitch, scanners detect organic matter, and the furnace emits human-like screams. The Company strictly orders the player to ignore these "escaping trapped gases."
- **The Climax:** Once the player has served their purpose and processed all the horrifying evidence, The Company has no more use for them. The final task requires the player to willingly step into the furnace themselves to complete their contract.

The core unique selling point is **Moral Complicity**. The game forces the player to actively participate in horrific acts (incinerating organic matter and human remains) simply to fulfill their daily corporate quota and avoid termination. A stylized retro low-poly aesthetic is used to obscure gruesome details, forcing the player's imagination to fill in the terrifying gaps.

---

### 2. Core Gameplay Loop

The player must survive a 7-day work contract. A single mistake, skipped step, or falsified record results in a fatal "Rule Break" flagged by the HR System, leading to execution on Day 8.

**The Loop:**
1. **Receive** — A trash bag arrives via the Conveyor Belt.
2. **Weigh** — The bag is placed on the Weight Scale to determine if it is `OPTIMAL` or `OVER_CAPACITY`.
3. **Document (Check)** — The player uses the Paper & Clipboard to check off that the bag has been weighed.
4. **Incinerate** — The bag is thrown into the Furnace.
5. **Document (Cross Out)** — The player crosses the task off the clipboard to finalize the disposal.
6. **Clock Out** — Once the daily quota is met, the player interacts with the Exit Door to end the shift.

**Diegetic UI & Physical Inventory:**
The game features zero traditional HUD elements. All UI is diegetic — meaning it exists physically in the game world. The player can only hold one item at a time (Physical Inventory). To know their tasks, they must physically hold the Clipboard. To know a bag's contents, they must hold the Scanner.

---

### 3. Day-by-Day Escalation

The game uses a `DayData` ScriptableObject system, allowing each day's rules, spawning behavior, tool unlocks, and narrative beats to be configured entirely through data, with zero hardcoded logic.

| Day | Quota | Narrative & Mechanical Escalation |
| :--- | :--- | :--- |
| **1** | 10 Bags | Introduction to the core loop. Bags feel heavy and sound "wet." |
| **2** | 10 Bags | **Scanner Unlocked.** Bags show "no metal", but bag #9 twitches and scans as "organic matter detected." |
| **3** | 10 Bags | **Conveyor Broken.** Player must manually retrieve bags from a dark corner where screams can be faintly heard. |
| **4** | 10 Bags | Directive: "Ignore acoustics, they are trapped gases." 3 bags scan as organic and scream when incinerated. |
| **5** | 10 Bags | **Bag Opening Mechanic.** 5 bags are `OVER_CAPACITY`. Player must open them (revealing raw meat mixed with household items) and repackage them. |
| **6** | 10 Bags | **Iron Poker Unlocked.** Incomplete combustion occurs. Player must poke charred debris (including human limbs) into the flames. |
| **7** | 1 Task | **The Final Shift.** Only 1 task on the clipboard. The conveyor is empty. The only object that reads `OPTIMAL` on the scale is the Player themselves. |
| **8** | — | **Execution.** The player is terminated for a logged infraction. Alarm lights activate, then the HR termination reason is displayed. |

---

### 4. Technical Architecture and Design Patterns

The codebase uses three core architectural patterns to achieve full decoupling and zero `FindObjectOfType` calls at runtime:

| Pattern | Implementation | Purpose |
| :--- | :--- | :--- |
| **Service Locator** | `ServiceLocator.cs` | Static registry. `GameManager`, `PlayerController`, and `AudioManager` register themselves on `Awake()`. Any script can access them at zero lookup cost via `ServiceLocator.GameManager`, etc. |
| **Event Bus** | `GameEvents.cs` | Static C# `Action` delegates (`OnFurnaceActivated`, `OnFurnaceDoorToggled`, `OnTriggerGameOver`). Enables fire-and-forget communication between `PlayerInteraction`, `FurnaceLogic`, `ClipboardTask`, and `GameManager` without any script referencing another. |
| **Data-Driven Config** | `DayData.cs` (ScriptableObject) | Each day's rules (quotas, spawning, tool unlocks, hazards, special events) are defined as Inspector-editable assets. `GameManager` and `TrashSpawner` read from the current day's `DayData` to configure the entire shift. |

---

### 5. Script Reference

The project contains **34 scripts** organized into the following functional layers:

#### 5.1 Infrastructure Layer

These scripts form the foundation that all other systems depend on.

| Script | Lines | Role |
| :--- | :--- | :--- |
| `ServiceLocator.cs` | 29 | Static dependency injection container. Provides zero-cost global access to `GameManager`, `PlayerController`, and `AudioManager` via property getters. |
| `GameEvents.cs` | 9 | Static C# `Action` event bus with 3 events: `OnFurnaceActivated`, `OnFurnaceDoorToggled(bool)`, `OnTriggerGameOver(string)`. Fully decouples all interactable systems. |
| `DayData.cs` | 30 | `ScriptableObject` data container. Defines per-day configuration: quotas, conveyor state, tool unlocks, spawning rules, hazards, and special day flags (`isFinalDay`, `isExecutionDay`). |
| `RuleBreak.cs` | 19 | Static constants class. Centralizes all 9 infraction strings used by the HR termination system, ensuring consistent dystopian tone across all game-over triggers. |

#### 5.2 Core Managers

| Script | Lines | Role |
| :--- | :--- | :--- |
| `GameManager.cs` | 190 | Central state machine. Manages day progression, quota tracking, honesty verification (`physicalWeighedCount` vs. `uiCheckedCount` vs. `uiCrossedOutCount`), rule-break logging, Day 8 execution sequence, god mode cheat flag, and true ending trigger. Uses `RuntimeInitializeOnLoadMethod` to reset static state on domain reload. |
| `AudioManager.cs` | 89 | Dictionary-based SFX system. Supports both 2D global playback and 3D spatial audio with configurable volume, pitch, and pitch randomization (±10%). Uses temporary `AudioSource` GameObjects that self-destruct after playback. |
| `SaveManager.cs` | 119 | Static persistence system. Saves/loads game state as JSON to `Application.persistentDataPath/saves/`. Stores per-slot metadata (`currentDay`, `ruleBreakReason`, `timestamp`) and `Texture2D` screenshots as PNG thumbnails. Includes a `ResizeScreenshot` utility using `RenderTexture` blitting. |

#### 5.3 Player Systems

| Script | Lines | Role |
| :--- | :--- | :--- |
| `PlayerController.cs` | 190 | First-person `CharacterController`-based movement with walking, jumping, crouching (smooth height interpolation), ground detection via `Physics.CheckSphere`, noclip flight mode (Space/Shift for vertical), conveyor belt velocity application, and footstep audio with interval-based timing. |
| `PlayerInteraction.cs` | 443 | The largest script. Handles **all** player-world interaction through a single optimized raycast per frame. Manages: physics-based object grabbing with `Rigidbody` velocity, tool equipping with lerped positioning, furnace door animation (hinge rotation with cooldown anti-spam), bag opening/sealing (Day 5), hold-to-interact for bag examination, scanner/clipboard/poker tool usage routing, and diegetic UI prompt updates. |

#### 5.4 Environment & Interactables

| Script | Lines | Role |
| :--- | :--- | :--- |
| `FurnaceLogic.cs` | 185 | Incineration zone with trigger-based item tracking. Processes burning via coroutine: validates door state, checks honesty with `GameManager`, burns bags (spawning charred debris on Day 6), chars `DebrisLogic` objects, detects tool destruction (infraction), detects player presence for final day ending, and enforces `OVER_CAPACITY` jam rules. |
| `ConveyorBelt.cs` | 62 | Kinematic Rigidbody trick: modifies `rb.position` backward then snaps via `rb.MovePosition` in `FixedUpdate`, creating frictionless surface velocity for resting objects. Scrolls belt texture for visual feedback. `OnTriggerStay`/`OnTriggerExit` push the player's `CharacterController` via `conveyorVelocity`. Respects `isBroken` flag. |
| `WeightScaleReader.cs` | 60 | Trigger-based weighing station. Reads `TrashBag_data.WeightCategory` and displays result on a `TextMeshPro` world-space display. Tracks physical weigh counts for honesty verification and enforces chronological sequence rules. Player stepping on the scale reads `OPTIMAL` (Day 7 mechanic). |
| `ExitDoor.cs` | 104 | Shift-end mechanic with hold-to-interact (`fillAmount` radial indicator). Validates quota completion and paperwork before advancing the day. Handles fade-in on scene start and fade-out on shift end. Routes to Day 8 execution if a rule was broken. |
| `TrashSpawner.cs` | 100 | Timer-based spawner with randomized intervals. Reads `DayData` to determine organic bag indices (specific or random), over-capacity bag indices, and empty bag spawning. Assigns random excess trash prefab variants to over-capacity bags. Pauses spawning on final/execution days. |
| `DebrisLogic.cs` | 58 | Charred remains from incomplete combustion (Day 6). Applies blackened material swap on char. Pokeable by `IronPoker` — destroys itself when poked while charred. Changes layer to `Default` after charring to remove from interactable mask. |
| `OrganicJitter.cs` | 68 | Horror effect: periodic randomized spasms on organic trash bags. Applies scale distortion and Rigidbody impulse forces on a random timer. Plays `BagJitter` audio on each spasm. Pauses when `Time.timeScale` is 0. |
| `AlarmLight.cs` | 11 | Simple rotating light for the Day 8 execution sequence. Rotates around Y-axis at 360°/s. |
| `CollisionAudio.cs` | 19 | Generic impact audio. Plays a named SFX via `AudioManager` when a collision exceeds `minImpactForce` threshold. Attached to bags and throwable objects. |

#### 5.5 Tools & Item Data

| Script | Lines | Role |
| :--- | :--- | :--- |
| `TrashBag_data.cs` | 53 | Data component on all bags. Tracks `WeightCategory` (UnderLoad/Optimal/OverCapacity), `isOrganic`, `hasBeenWeighed`, `hasBeenRecorded`. Handles Day 5 bag opening (model swap, excess trash ejection with impulse force) and sealing. |
| `ScannerTool.cs` | 72 | Handheld scanner with hold-to-scan mechanic. `RaycastAll` detects `TrashBag_data` through multiple colliders. Displays scan progress percentage, then reveals organic/metal status with color-coded `TextMeshPro` output. |
| `ClipboardTool.cs` | 32 | Focus mode controller. When focused, locks `canLook`/`canMove` on `PlayerController` and unlocks cursor for UI interaction with clipboard tasks. |
| `ClipboardTask.cs` | 48 | Per-task UI element on the clipboard. Check (✓) validates sequence against `GameManager.uiCheckedCount` and physical weigh count. Cross-out (~~strikethrough~~) validates against burn count. Both enforce chronological order and honesty — falsification triggers infractions. |
| `IronPoker.cs` | 32 | Day 6 tool. `RaycastAll` forward to find `DebrisLogic` components and calls `Poke()` to destroy charred debris. Plays distinct audio for hit vs. miss. |
| `EmptyBag.cs` | 48 | Day 5 repackaging bag. Accepts `ExcessTrash`-tagged objects via `OnCollisionEnter`. Visually inflates (`localScale.y` lerp) as trash count increases. Swaps from empty to full model at capacity and sets `WeightCategory` to `Optimal`. |
| `ToolSettings.cs` | 8 | Simple data component storing per-tool equip position and rotation vectors, read by `PlayerInteraction` during tool pickup. |

#### 5.6 UI & Meta Systems

| Script | Lines | Role |
| :--- | :--- | :--- |
| `MainMenuManager.cs` | 107 | Main menu controller with camera parallax (mouse-driven `Quaternion.Slerp`). Manages panel navigation (Main, Settings, Tutorial, Load Game) and ESC routing. |
| `PauseMenuManager.cs` | 167 | Pause menu with screenshot capture on pause (`WaitForEndOfFrame` → `ReadPixels` → resize to 320×180 thumbnail). Manages save panel, settings, tutorial sub-panels. Guards against `DevConsole.IsOpen` conflict. |
| `SaveLoadPanel.cs` | 106 | Dual-mode (Save/Load) panel controller. Manages 4 `SaveSlotUI` instances, swaps confirmation prompt sprites per action type (save/load/delete), and handles the full confirmation flow. |
| `SaveSlotUI.cs` | 85 | Individual save slot row. Displays slot title, timestamp, and screenshot thumbnail (`RawImage` from PNG). Toggles between populated and empty states. Delete button with conditional interactability. |
| `SettingsManager.cs` | 95 | `PlayerPrefs`-based settings persistence. Controls: Master Volume (`AudioListener.volume`), SFX Volume, Mouse Sensitivity (0.5–5.0), Invert Y, Fullscreen. Properly removes listeners on `OnDisable`. |
| `TutorialPanel.cs` | 51 | Slideshow-based tutorial viewer with Previous/Next navigation, page indicator, and sprite-swap display. Resets to first slide on `OnEnable`. |
| `DevConsole.cs` | 305 | Easter egg developer console toggled with `~`. Built-in commands: `toggle_hud`, `help`, `clear`. Secret cheat unlock via `imthefuckingdeveloper` enables: `start day X`, `spawn object X`, `god` (ignore infractions), `noclip` (fly mode), `list objects`. Uses `LayoutRebuilder.ForceRebuildLayoutImmediate` for auto-scrolling output. |
| `DisabledButtonTooltip.cs` | 19 | `IPointerEnterHandler`/`IPointerExitHandler` implementation. Shows/hides a tooltip when hovering a disabled button. |
| `UIClickSFX.cs` | 24 | Lightweight click audio component. Creates its own `AudioSource` and plays a clip via `PlayOneShot` on button press. |

---

### 6. Project Structure

```
Assets/
  Audio/              Sound effects
  DayData/            ScriptableObject assets
  Fonts/              Custom typefaces
  Materials/          Materials
  Models/             Low-poly meshes
  Prefabs/            Instantiable objects
  Scenes/             Game scenes/screens
  Scripts/            All 34 C# scripts (documented above)
  Settings/           URP render pipeline settings
  TextMesh Pro/       TMP font assets and shaders
  Textures/           UI sprites and textures
  _Blender/           Source .blend files for 3D models
```

---

### 7. Key Technical Highlights

- **Single Raycast Optimization:** `PlayerInteraction` fires exactly **one** raycast per `Update()` frame, caching the hit result for hover prompts, pickup logic, tool usage, and door interaction simultaneously.
- **Kinematic Rigidbody Conveyor:** `ConveyorBelt` uses Unity's native physics to move resting objects by manipulating `rb.position` ↔ `rb.MovePosition`, achieving frictionless belt movement without `OnCollisionStay` polling.
- **Zero-HUD Diegetic Design:** All gameplay information (tasks, scan results, weight readings, progress) is conveyed through in-world objects the player physically interacts with — no floating UI elements.
- **Honesty Verification System:** `GameManager` independently tracks three counters (`physicalWeighedCount`, `uiCheckedCount`, `uiCrossedOutCount`) and cross-validates them against each other and `bagsBurnedToday` to detect falsified paperwork in real-time.
- **Data-Driven Progression:** Adding a new day requires only creating a new `DayData` ScriptableObject asset and dragging it into the `dayConfigs` array — no code changes required.
- **Screenshot Save Thumbnails:** Pause menu captures a full-resolution screenshot via `ScreenCapture.ReadPixels`, downsamples it to 320×180 via `RenderTexture` blitting, and stores it as a lossless PNG alongside the JSON save data.
