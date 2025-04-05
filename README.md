# **GFX Game Engine**

[![NuGet Version](https://img.shields.io/nuget/v/GFX.svg?label=NuGet&color=blue)](https://www.nuget.org/packages/GFX)

Welcome to the GFX Game Engine – an open-source framework designed to make game development simple, powerful, and accessible.

If you have any questions about GFX, join our community on [Discord](https://discord.gg/qZRgRKedBs).
 
---

## **Overview**  
GFX Game Engine is a lightweight yet robust framework built in **C#** for developing 2D and 3D games. Whether you're an indie developer or a hobbyist, GFX empowers you with tools for seamless scene management, rendering, physics simulation, and game logic customization.  

**Why Choose GFX?**  
- Simplifies game development with intuitive tools and APIs.  
- Combines performance with flexibility for 2D and 3D workflows.  
- Open-source with MIT licensing for unlimited creative freedom.  

---

## **Core Features**
### **Rendering**  
GFX leverages OpenGL 4.5 via OpenTK for high-performance rendering, with support for custom shaders and materials. Supported 3D file formats include Wavefront (.obj), FBX, Collada, and GLTF. Future plans include support for Vulkan and DirectX 12.

### **Physics**  
Physics are powered by **BulletSharp**, a wrapper for the Bullet Physics library. The framework supports:  
- **PhysicHandler3D** and **PhysicHandler2D** for seamless simulations.  
- Custom physics handlers to suit advanced gameplay needs.  

### **2D & 3D Game Development**  
- Fully integrated **layer system** for managing game elements such as sprites, 3D objects, and empty game objects.  
- Support for **GameBehaviors** to simplify custom game logic implementation.  

---

## **Getting Started**
Installation Guide for GFX (.NET 8)

1. Install the GFX NuGet Package `dotnet add package GFX`
2. Download the Native Dependency [(libbulletc)](https://gfx-engine.org/downloads/)
3. Extract the file and place the libbulletc binary in your project folder (next to your .csproj).
4. Ensure the native library is copied to your build output folder

## Example
Check out the example project included in the GFX repository or linked on the GFX website.
It shows a working setup with libbulletc and GFX already configured.

### Contribute to GFX
GFX Game Engine thrives on community contributions! Whether it’s reporting bugs, submitting feature requests, or contributing code, your input is always welcome. Check out our contribution guidelines to get involved.

### License
The GFX Game Engine is released under the MIT License, ensuring complete freedom for commercial and personal projects. See the LICENSE folder for full terms.
