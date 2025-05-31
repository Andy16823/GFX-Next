
# **GFX Game Engine**

[![NuGet Version](https://img.shields.io/nuget/v/GFX.svg?label=NuGet&color=blue)](https://www.nuget.org/packages/GFX)

Welcome to the GFX Game Engine – an open-source framework designed to make game development simple, powerful, and accessible.

If you have any questions about GFX, join our community on [Discord](https://discord.gg/qZRgRKedBs).

## **Overview**  
The GFX Game Engine is a lightweight framework for creating 2D and 3D games in C# with .NET 8. It includes modules for rendering, graphics, animation, and audio.

**Features**  

- **Simple creation of 2D and 3D scenes**  
  Manage scenes with a minimal and clear structure.

- **Custom behavior system**  
  Define game logic using a modular, component-based system.

- **Physics simulation with BulletSharp**  
  Integrated Bullet3 physics via the BulletSharp wrapper.

- **Support for multiple 3D model formats (Assimp)**  
  Import formats like FBX, OBJ, DAE, and others using Assimp.

- **Rendering with OpenGL via OpenTK**  
  Cross-platform rendering backend using OpenGL.

- **Instanced rendering support**  
  Efficient rendering of many instances of the same mesh.

- **3D audio using OpenAL**  
  Spatial and positional audio support through OpenAL.

- **2D lighting with clustered forward rendering**  
  Real-time lighting support for 2D scenes.

- **3D lighting with clustered forward rendering**  
  Efficient handling of multiple dynamic lights in 3D.

- **Skeletal animation**  
  Bone-based animations for animated 3D models.

- **Abstract and extensible architecture**  
  Designed to be easily adaptable to different workflows and needs.

- **Asset loading system**  
  Built-in loader with support for custom file formats.

- **Easy installation via NuGet**  
  Available as a NuGet package for quick integration.

- **MIT licensed and open source**  
  Free to use, modify, and distribute.

- **Active community on Discord**  
  Get support, share projects, and give feedback in an active Discord server.

## **Core Features**
### **Rendering**
GFX utilizes OpenGL 4.5 via OpenTK for high-performance rendering, with full support for custom shaders and materials. Supported 3D file formats include Wavefront (.obj), FBX, Collada, and GLTF.
Future roadmap: support for Vulkan and DirectX 12.

- **Multiple Lights in 2D & 3D Scenes** – Support for multiple light sources in both 2D and 3D environments, with realistic lighting behavior (not just blending).
- **Experimental PBR Rendering** – Initial integration of Physically Based Rendering (PBR) for primitives.
- **Specular Glossiness Shader Optimizations** – Performance and visual improvements for specular lighting.
- **Frustum Culling for Lights** – Efficiently cull light sources outside of the view frustum for better performance.
- **Instanced Rendering (2D & 3D)** – Enhanced rendering performance for multiple identical objects.
- **Support for SSBOs & ArrayBuffers** – New support for Shader Storage Buffer Objects (SSBOs) and ArrayBuffers in the rendering pipeline.

### **Physics**
Physics in GFX are powered by **BulletSharp**, a .NET wrapper for the robust Bullet Physics library. Features include:
- **PhysicHandler3D** and **PhysicHandler2D** for seamless simulations.
- New **Rigidbodies** and **Triggers** for advanced 3D physics.
- **2D Physics Integration** – Fully integrated 2D physics system.

### **Audio**
- **3D Sound via OpenAL** – Spatial audio support for more immersive soundscapes. (OpenAL must be installed or the DLL manually included.)

### **Lighting Management**
- **Light Manager** – New system for managing lights in both 2D and 3D scenes.
- **Clustered Frustum Culling (2D & 3D)** – Optimized light culling system to improve rendering performance.
  
### **Getting Started**
Installation Guide for GFX (.NET 8)

1. Install the GFX NuGet Package `dotnet add package GFX`

## Example
Check out the example project included in the GFX repository or linked on the GFX website.
It shows a working setup with libbulletc and GFX already configured.

### Contribute to GFX
GFX Game Engine thrives on community contributions! Whether it’s reporting bugs, submitting feature requests, or contributing code, your input is always welcome. Check out our contribution guidelines to get involved.

### License
The GFX Game Engine is released under the MIT License, ensuring complete freedom for commercial and personal projects. See the LICENSE folder for full terms.
