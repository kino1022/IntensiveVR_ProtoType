# VR Cockpit Camera Projection System - Setup Guide

## Overview

This system implements a VR cockpit projection mechanism for robot control games in Unity. The camera view from the robot is projected onto the inside of a sphere object (representing a cockpit) placed in an isolated coordinate space.

## Quick Start

### Prerequisites
- Unity 2022.3 or later
- XR Interaction Toolkit package installed
- Universal Render Pipeline (recommended)

### Basic Setup Steps

1. **Create the Cockpit Sphere**
   - Create an empty GameObject named "CockpitSphere"
   - Add the `InvertedSphereMesh` component
   - Add a `MeshRenderer` component
   - Assign the `CockpitProjectionMaterial` to the renderer

2. **Create the Projection Camera**
   - Create a new Camera named "ProjectionCamera"
   - Position it on your robot's head/viewpoint
   - Add the `CockpitCameraProjection` component
   - In the Inspector, assign the CockpitSphere's renderer to the "Cockpit Sphere Renderer" field

3. **Setup the Cockpit Manager**
   - Create an empty GameObject named "CockpitSystem"
   - Add the `CockpitManager` component
   - Configure in the Inspector:
     - Projection Camera Object: Drag the ProjectionCamera
     - Cockpit Sphere Object: Drag the CockpitSphere
     - VR Camera: Drag your XR Origin's Main Camera
     - Isolated Position: Set to (1000, 1000, 1000) or similar

4. **Test in Play Mode**
   - Enter Play mode
   - The player should be inside the cockpit sphere
   - The sphere should display the robot's view

## Architecture

```
Robot's View (ProjectionCamera) 
    ↓
RenderTexture (2048x2048)
    ↓
Cockpit Sphere Material
    ↓
Player sees inside sphere
```

## Components

### CockpitCameraProjection
Main component that handles camera-to-texture projection.

**Key Settings:**
- Field of View: 60-179° (default: 90°)
- Near Clip Plane: 0.1m
- Far Clip Plane: 1000m
- Cockpit Radius: 2m

### InvertedSphereMesh
Generates a sphere mesh that can be viewed from the inside.

**Key Settings:**
- Segments: 48 (higher = smoother, but more expensive)
- Radius: 1m

### CockpitManager
Manages the entire cockpit system.

**Key Settings:**
- Isolated Position: Where to place the cockpit (e.g., 1000, 1000, 1000)
- Place VR Camera In Cockpit: Whether to automatically move the VR camera

## Advanced Usage

### Attaching to Robot
```csharp
public class RobotController : MonoBehaviour
{
    [SerializeField] private CockpitManager cockpitManager;
    [SerializeField] private Transform robotHead;
    
    void Start()
    {
        // Attach projection camera to robot's head
        cockpitManager.AttachProjectionCameraToRobot(robotHead);
    }
}
```

### Dynamic Activation
```csharp
// Enable cockpit mode
cockpitManager.SetCockpitActive(true);

// Disable cockpit mode
cockpitManager.SetCockpitActive(false);
```

### Adjusting Resolution
```csharp
CockpitCameraProjection projection = projectionCamera.GetComponent<CockpitCameraProjection>();

// Lower resolution for better performance
projection.SetRenderTextureResolution(1024, 1024);

// Higher resolution for better quality
projection.SetRenderTextureResolution(4096, 4096);
```

## Performance Optimization

### For Mobile VR (Quest, etc.)
- Set RenderTexture resolution to 1024x1024 or 1536x1536
- Use segments: 32 for the inverted sphere
- Enable Fixed Foveated Rendering
- Optimize ProjectionCamera culling mask

### For PC VR
- RenderTexture can be 2048x2048 or higher
- Use segments: 48-64 for the inverted sphere
- Enable Single Pass Instanced rendering

## Troubleshooting

### Problem: Can't see inside the sphere
**Solution:** 
- Verify `InvertedSphereMesh` component is attached
- Check that the material's Cull mode is set to Off or Front
- Ensure the sphere mesh has been generated (check in Play mode)

### Problem: No projection visible
**Solution:**
- Check that ProjectionCamera is active
- Verify RenderTexture is created (check in Inspector during Play mode)
- Ensure material's _MainTex is assigned the RenderTexture
- Check Cockpit Sphere Renderer reference in CockpitCameraProjection

### Problem: VR camera not positioned correctly
**Solution:**
- Verify XR Origin exists in scene
- Check CockpitManager's VR Camera reference
- Ensure "Place VR Camera In Cockpit" is enabled
- Verify Isolated Position coordinates

### Problem: Performance issues
**Solution:**
- Reduce RenderTexture resolution
- Lower sphere segments count
- Optimize ProjectionCamera's culling mask
- Disable anti-aliasing on RenderTexture
- Check that only necessary layers are rendered

## Technical Notes

### Coordinate Isolation
The cockpit is placed at coordinates far from the main game world (e.g., 1000, 1000, 1000). This prevents:
- Collision with game objects
- Lighting interference
- Physics interactions
- Level geometry clipping

### Shader Requirements
The cockpit material should use:
- Unlit shader (for best performance)
- Cull mode: Off or Front (to see inside)
- No lighting calculations needed

### VR Considerations
- Works with all major VR platforms (Quest, PCVR, PSVR)
- Compatible with XR Interaction Toolkit
- Supports roomscale VR tracking
- No additional SDK requirements

## Example Scene Hierarchy

```
Scene
├── XR Origin (VR Player)
│   └── Main Camera (VR Camera)
├── Robot
│   └── Head
│       └── ProjectionCamera (with CockpitCameraProjection)
├── CockpitSystem (with CockpitManager)
│   └── CockpitSphere (with InvertedSphereMesh, at isolated coordinates)
└── Game World
    └── ... (your game objects)
```

## FAQ

**Q: Can I use this with non-VR cameras?**
A: Yes, but it's designed for VR. For non-VR, a simple camera attachment might be more appropriate.

**Q: Does this work with multiplayer?**
A: Yes, but each player needs their own cockpit system with unique isolated coordinates.

**Q: Can I customize the cockpit appearance?**
A: Yes, modify the material on the CockpitSphere or add additional visual elements inside the sphere.

**Q: What about motion sickness?**
A: The isolated cockpit can actually help reduce motion sickness by providing a stable reference frame.

**Q: Can I add UI inside the cockpit?**
A: Yes, place UI elements as children of the CockpitSphere at appropriate positions.

## License

This implementation is part of the IntensiveVR_ProtoType project.
