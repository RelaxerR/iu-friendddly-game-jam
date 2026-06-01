# Not So Friendly Bee

Multiplayer arena shooter built for a game jam, set in a honey-themed glade. Players fight with a bee-themed blaster while the match alternates between **GreenTime** and **RedTime** phases that change combat rules, collect honey power-ups, and use jump pads to move around the map.

|                  | |
|------------------|---|
| **Unity**        | `6000.2.6f2` (Unity 6) |
| **Product name** | Not So Friendly Bee |
| **Team**           | Glorious Urki |
| **Max players**  | 4 |

---

## Game overview

**Not So Friendly Bee** is a **third-person multiplayer combat game** on a single outdoor level (**Honey Glade**). Up to four players spawn at colored spawn points, move with WASD, look with the mouse, jump, and shoot raycast-based attacks. Health, kills, and deaths are tracked per player.

The USP is a globally synchronized **RedTime / GreenTime** cycle (`GameModeManager`) that changes whether attacks hurt enemies or yourself, backed by different music tracks per phase.

---

## Gameplay mechanics

### Movement & camera

- **Controller:** `CharacterController` driven in Fusion’s `FixedUpdateNetwork` (`PlayerMovement`).
- **Defaults (player prefab):** move speed `15`, jump height `3`, gravity `-30`, slope slide on steep surfaces.
- **Camera:** Local first-person style—mouse pitch on camera, yaw on body (`PlayerCamera`). Cursor locked for the input authority.
- **Network input:** `FusionInputHandler` sends `NetworkInputData` (WASD direction, jump, camera yaw) each tick.

### Combat

- **Weapon:** Screen-center raycast (`RaycastAttack`), ~`17` damage, `0.5s` fire rate, optional line renderer visual (`LRHoneyOrange` prefab).
- **Health:** `100` HP, networked; death sets `IsDead`, increments `DeathCount`, awards `KillCount` to attacker.
- **Respawn:** Server picks a random `SpawnManager` spawn point, restores HP, resets camera via RPC.

### RedTime / GreenTime (core rule)

| Mode | Duration (HoneyGlade scene) | Combat behavior |
|------|----------------------------|-----------------|
| **GreenTime** | 6s (serialized on scene object; code default 8s) | Hitting another player’s `Health` applies damage to **yourself** |
| **RedTime** | 16s | Hitting another player applies damage to **them** |

Mode is server-authoritative, replicated with `[Networked] CurrentMode` on `GameModeManager`. `MediaManager` cross-fades music when the mode changes.

### Pickups & world interactables

| Object | Script | Effect |
|--------|--------|--------|
| **HoneyHealBoost** (orange honey) | `HealthBoostController` | Heals **30** HP (`Health Boost.asset`). If speed boost is active, heals **0** and instead deals damage equal to current HP (eliminates overheal exploit). Despawns and respawns after **17s** (`BoostRespawnManager`). |
| **HoneyPurpleBoost** | `SpeedBoostController` | **1.5×** speed for **15s** (`Speed Boost.asset`). Respawn after **22s**. |
| **FlowerJumpBooster** | `JumpBoostPlatform` | Trigger applies upward impulse (~`48` jump height on prefab). Uses `PlayerForces` (not networked). |
| **HoneyCover** | (environment prefab) | Cover / prop; no gameplay script in `Assets/Project/Scripts`. |

### UI

- **PlayerHUD:** HP slider/text, kill/death counters for local player.
- **CrosshairUI:** Runtime-built centered crosshair image.

### Controls (legacy Input Manager)

Gameplay code uses **`Input.GetAxisRaw` / `GetButtonDown`** (not `.inputactions` assets):

| Action | Binding |
|--------|---------|
| Move | WASD / arrows |
| Jump | Space (`Jump`) |
| Shoot | Left mouse (`GetMouseButtonDown(0)`) |
| Look | Mouse X / Y |

`ProjectSettings` uses **Input System Package** for UI (`InputSystemUIInputModule` on EventSystem) with **Both** handlers enabled (`activeInputHandler: 2`).

---

## Key assets & game objects

### Production scenes

| Scene | Path | Role |
|-------|------|------|
| **HoneyGlade** | `Assets/Project/Levels/Production/HoneyGlade.unity` | **Main game** — in Build Settings |
| **Bootstrap** | `Assets/Project/Levels/Production/Bootstrap.unity` | Minimal scene with `GameModeManager` + lighting; **not** in build list |
| **Test scenes** | `Assets/Project/Levels/Test/*TestScene.unity` | Per-developer sandboxes (Fusion bootstrap, ProBuilder, experiments) |


### Art & rendering

- **URP** pipeline assets under `Assets/Project/Settings/` and `Assets/Settings/` (PC/Mobile/quality tiers).
- **Terrain** layers: grass, rocks, trail, snow (`Art/Texture/Environment/Terrain/`).
- **Animations:** `honey_pot_ArmatureAction`, `HoneyController` animator.

---

## Project architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     HoneyGlade.unity                         │
├─────────────────────────────────────────────────────────────┤
│  FusionBootstrap ──► NetworkRunner (Host / Shared mode)      │
│         │                    │                               │
│         │                    ├── FusionInputHandler          │
│         │                    ├── PlayerSpawner               │
│         │                    └── Simulated NetworkObjects    │
├─────────────────────────────────────────────────────────────┤
│  Scene services: GameModeManager, SpawnManager,              │
│                  BoostRespawnManager, MediaManager, UI       │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
              ┌───────────────────────────────┐
              │   Spawned player prefab        │
              │   NetworkObject + Transform    │
              │   PlayerMovement, Health,      │
              │   RaycastAttack, PlayerCamera, │
              │   PlayerForces, CharacterController │
              └───────────────────────────────┘
```

- **Networking model:** Photon **Fusion 2** (`Assets/Photon/Fusion/`), server/state authority for gameplay rules, `[Networked]` properties and RPCs for combat and boosts.
- **Tick rate:** 64 Hz client simulation (`NetworkProjectConfig.fusion`).
- **Region:** Photon fixed region **EU** (`PhotonAppSettings.asset`).
- **Patterns:** ScriptableObject tuning data; boost controllers as networked triggers; singleton `SpawnManager` for respawn points (not Fusion-spawned).

---

## Code structure

All gameplay code lives under **`Assets/Project/Scripts/`** (25 C# files).

```
Assets/Project/Scripts/
├── Bootstrap/
│   └── GameModeManager.cs      # RedTime / GreenTime cycle
├── Boost/
│   ├── Abstract/               # IBoost, IBoostController, IBoostDurable
│   ├── Boosts/                 # HealthBoostController, SpeedBoostController
│   ├── Settings/               # ScriptableObject configs
│   └── PlayerBoostController.cs  # Local tag-based boosts (unused in networked pickups)
├── Managers/
│   └── BoostRespawnManager.cs
├── Media/
│   ├── MediaManager.cs
│   └── MediaSettings.cs
├── Network/
│   ├── Input/                  # FusionInputHandler, NetworkInputData
│   ├── Managers/SpawnManager.cs
│   ├── Player/PlayerSpawner.cs
│   └── Animations/ObjectSpawner.cs  # Spawns animated prefab on authority (usage scene-specific)
├── Player/
│   ├── Controller/             # PlayerMovement, PlayerCamera
│   └── Combat/                 # Health, RaycastAttack
├── UI/                         # PlayerHUD, CrosshairUI
└── World/JumpBoost/            # JumpBoostPlatform (file: JumpBoost.cs), PlayerForces
```

---

## Technologies & plugins

| Technology | Version / notes |
|------------|-----------------|
| **Unity** | `6000.2.6f2` |
| **URP** | `com.unity.render-pipelines.universal` 17.2.0 |
| **Photon Fusion** | 2.0.7 (Build 1327 per `release_history.txt`) |
| **Photon Realtime** | Bundled under `Assets/Photon/` |
| **TextMesh Pro** | UI text (HUD) |
| **Input System** | 1.14.2 (package present; gameplay uses legacy axes) |
| **ProBuilder** | 6.0.7 (test scenes / level blockout) |
| **Splines** | 2.8.2 (scene splines in HoneyGlade) |
| **AI Navigation** | 2.0.9 (package installed; no custom NavMesh agents in Project scripts) |
| **Timeline / Visual Scripting** | Installed; not used by Project scripts |
| **Multiplayer Center** | 1.0.0 |

---

## Setup & build instructions

### Prerequisites

- **Unity Hub** with editor **6000.2.6f2** (see `ProjectSettings/ProjectVersion.txt`).
- **Photon account** with Fusion App Id (project already contains `Assets/Photon/Fusion/Resources/PhotonAppSettings.asset`).

### First-time setup

1. Clone the repository.
2. Open the project folder in Unity Hub and allow package resolution (URP, Fusion, etc.).
3. Open **`Assets/Project/Levels/Production/HoneyGlade.unity`**.
4. Confirm **Fusion** App Id under `Tools → Fusion → Fusion Hub` (or `PhotonAppSettings` asset) if cloud connection fails.
5. Press **Play** — Fusion Bootstrap should start a **Host** session automatically.

### Local multiplayer testing

- Use **Fusion Bootstrap** settings on **Prototype Network Start** (`AutoClients`, virtual instances).
- Alternatively run **ParrelSync** / multiple builds (not configured in repo; manual setup).

### Player build

1. **File → Build Settings** — scene `HoneyGlade` should be enabled.
2. Choose target platform (Standalone Windows is default-friendly).
3. Build; all clients need the same Fusion App Id / version and network access to Photon cloud (EU).
