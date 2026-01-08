# VR Cockpit System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                         VR COCKPIT SYSTEM                            │
└─────────────────────────────────────────────────────────────────────┘

GAME WORLD (Normal Coordinates: 0, 0, 0)
┌────────────────────────────────────────┐
│                                        │
│  ┌──────────┐                         │
│  │  Robot   │                         │
│  │  ┌────┐  │                         │
│  │  │Head│←─┼─────────────────────┐  │
│  │  └────┘  │                     │  │
│  │    ↑     │                     │  │
│  │ Moves/   │                     │  │
│  │ Rotates  │                     │  │
│  └──────────┘                     │  │
│                                   │  │
│  [Game Objects]                   │  │
│  [Environment]                    │  │
│  [NPCs]                           │  │
│                                   │  │
└───────────────────────────────────┼───┘
                                    │
                   ┌────────────────┴─────────────────┐
                   │ ProjectionCamera                 │
                   │ - Follows robot head             │
                   │ - Renders to RenderTexture       │
                   │ - FOV: 90°                       │
                   └────────────┬─────────────────────┘
                                │
                                ↓
                   ┌────────────────────────────┐
                   │   RenderTexture (2048²)    │
                   │   [Camera View Data]       │
                   └────────────┬───────────────┘
                                │
                                ↓
================================│================================
                                │
ISOLATED COCKPIT SPACE (Coordinates: 1000, 1000, 1000)
┌───────────────────────────────┼──────────────────────────────┐
│                               │                              │
│                               ↓                              │
│              ┌────────────────────────────────┐              │
│              │  CockpitSphere (Inverted)     │              │
│              │  - Radius: 2m                 │              │
│              │  - Normals: Facing inward     │              │
│              │  - Material: Shows RT texture │              │
│              │         ╱─────────╲           │              │
│              │       ╱             ╲         │              │
│              │     ╱                 ╲       │              │
│              │    │    ┌─────────┐   │      │              │
│              │    │    │VR Player│   │      │              │
│              │    │    │ (Camera)│   │      │              │
│              │    │    └─────────┘   │      │              │
│              │     ╲                 ╱       │              │
│              │       ╲             ╱         │              │
│              │         ╲─────────╱           │              │
│              │                               │              │
│              │   Player sees robot's view    │              │
│              │   projected on sphere inside  │              │
│              └───────────────────────────────┘              │
│                                                              │
│  No collision with game world                               │
│  No lighting interference                                   │
│  Player is "inside" the cockpit                             │
│                                                              │
└──────────────────────────────────────────────────────────────┘


COMPONENT INTERACTION FLOW:
════════════════════════════════════════════════════════════════

1. Robot Moves in Game World
   ↓
2. ProjectionCamera (attached to robot head) moves with robot
   ↓
3. ProjectionCamera renders view to RenderTexture
   ↓
4. RenderTexture is applied to CockpitSphere material
   ↓
5. VR Player (inside sphere at isolated coords) sees the texture
   ↓
6. Player experiences being inside robot cockpit


KEY COMPONENTS:
════════════════════════════════════════════════════════════════

┌──────────────────────────────────────────────────────────────┐
│ CockpitCameraProjection                                      │
├──────────────────────────────────────────────────────────────┤
│ - Manages ProjectionCamera                                   │
│ - Creates/manages RenderTexture                              │
│ - Applies texture to sphere material                         │
│ - Configurable FOV, resolution                               │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│ InvertedSphereMesh                                           │
├──────────────────────────────────────────────────────────────┤
│ - Generates sphere mesh procedurally                         │
│ - Inverts normals (point inward)                             │
│ - Reverses face winding                                      │
│ - Configurable segments & radius                             │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│ CockpitManager                                               │
├──────────────────────────────────────────────────────────────┤
│ - Integrates entire system                                   │
│ - Positions cockpit at isolated coords                       │
│ - Moves VR player into cockpit                               │
│ - Attaches camera to robot                                   │
└──────────────────────────────────────────────────────────────┘


COORDINATE ISOLATION BENEFITS:
════════════════════════════════════════════════════════════════

Game World (0,0,0)          Cockpit Space (1000,1000,1000)
┌─────────────────┐         ┌─────────────────┐
│ Robot           │         │ Sphere          │
│ Environment     │   VS    │ VR Player       │
│ Physics         │         │ UI (optional)   │
│ NPCs            │         │ Indicators      │
└─────────────────┘         └─────────────────┘

✓ No collision between sphere and game objects
✓ No lighting interference
✓ Separate rendering layers possible
✓ Easy to add cockpit-specific UI
✓ Player can't accidentally leave cockpit
✓ Clean separation of concerns


PERFORMANCE PROFILE:
════════════════════════════════════════════════════════════════

┌─────────────────────┬──────────┬─────────────────┐
│ Component           │ Cost     │ Notes           │
├─────────────────────┼──────────┼─────────────────┤
│ RenderTexture       │ Medium   │ ~16MB @ 2048²   │
│ Additional Camera   │ Medium   │ Second viewpoint│
│ Sphere Mesh         │ Low      │ <1MB memory     │
│ Scripts             │ Minimal  │ Negligible      │
└─────────────────────┴──────────┴─────────────────┘

Optimization Options:
• Lower RT resolution on mobile (1024² vs 2048²)
• Reduce sphere segments (32 vs 48)
• Optimize camera culling mask
• Use Fixed Foveated Rendering (Quest)


PLATFORM COMPATIBILITY:
════════════════════════════════════════════════════════════════

✓ Meta Quest 1/2/3/Pro
✓ PC VR (SteamVR, Oculus Link)
✓ PSVR 2
✓ Any OpenXR-compatible device

Requirements:
• Unity 2022.3+
• XR Interaction Toolkit 3.3.0+
• Universal Render Pipeline (recommended)
```
