
# **GFX Game Engine**

[![NuGet Version](https://img.shields.io/nuget/v/GFX.svg?label=NuGet&color=blue)](https://www.nuget.org/packages/GFX)

Welcome to the GFX Game Engine – an open-source framework designed to make game development simple, powerful, and accessible.

If you have any questions about GFX, join our community on [Discord](https://discord.gg/qZRgRKedBs).

## **Overview**  
The GFX Game Engine is a lightweight framework for creating 2D and 3D games in C# with .NET 8. It includes modules for rendering, graphics, animation, and audio.

---

## **Features**

- **Built on .NET 8 (Cross-Platform)**  
  Developed using .NET 8 – no legacy .NET Framework dependencies. Runs on Windows, Linux, and macOS.

- **Simple setup for 2D and 3D scenes**  
  Create and manage scenes with a minimal and clean structure.

- **Custom behavior system**  
  Define game logic using a flexible, component-based approach.

- **Physics simulation with BulletSharp**  
  Integrates Bullet3 physics via BulletSharp for 2D and 3D.

- **Multiple collision shapes**  
  Supports various collider types like boxes, spheres, capsules, and more in 2D and 3D.

- **3D model loading via Assimp**  
  Import standard formats such as FBX, OBJ, DAE, etc.

- **OpenGL rendering via OpenTK**  
  Cross-platform rendering powered by OpenGL through OpenTK. Vulkan support is planned for the future.

- **Instanced rendering support**  
  Render large numbers of objects efficiently using hardware instancing.

- **3D audio with OpenAL**  
  Provides spatial sound and positional audio.

- **2D lighting with clustered forward rendering**  
  Real-time 2D lighting with performance-friendly rendering.

- **3D lighting with clustered forward rendering**  
  Supports many dynamic light sources efficiently in 3D scenes.

- **Skeletal animation**  
  Bone-based animations for animated 3D models.

- **Modular asset loading system**  
  Includes a built-in loader system with support for custom file formats.

- **Abstract and extensible architecture**  
  Core systems are designed to be independent and adaptable to different workflows.

- **Independent subsystems**  
  Renderer, physics, scene management, and game logic are all decoupled.  
  Only need the renderer? No problem. Want to use your own lighting system? Also possible.

- **NuGet installation**  
  Available as a NuGet package for easy integration into your project.

- **MIT licensed and open source**  
  Fully open for personal or commercial use, modification, and distribution.

- **Active Discord community**  
  Get help, share your projects, and discuss ideas with other developers.
  
A solid foundation for 2D and 3D projects using modern C#/.NET – flexible, modular, and cross-platform.

---

### Architecture Flexibility

GFX-Next provides both ready-to-use standard components (such as Light3DManager, material systems, and loaders) and the ability to develop your own implementations via clearly defined interfaces like ILightManager. The renderer can also be used independently of the other systems. This makes GFX-Next suitable for both rapid prototyping and highly customized solutions.

---
  
### **Getting Started**
Installation Guide for GFX (.NET 8)

1. Install the GFX NuGet Package `dotnet add package GFX`

---

## Example
Check out the example project included in the GFX repository or linked on the GFX website.
It shows a working setup with libbulletc and GFX already configured.

---

### Contribute to GFX
GFX Game Engine thrives on community contributions! Whether it’s reporting bugs, submitting feature requests, or contributing code, your input is always welcome. Check out our contribution guidelines to get involved.

---

### License
The GFX Game Engine is released under the MIT License, ensuring complete freedom for commercial and personal projects. See the LICENSE folder for full terms.
