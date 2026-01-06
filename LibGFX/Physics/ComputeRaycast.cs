using LibGFX.Compute;
using LibGFX.Graphics;
using LibGFX.Graphics.Renderer.OpenGL;
using LibGFX.Math;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Physics
{
    /// <summary>
    /// Represents the result of a compute-based hit test, including the intersected triangle index, surface normal, and
    /// hit position.
    /// </summary>
    /// <remarks>This structure is typically used to convey the outcome of a ray or collision query performed
    /// on a mesh or geometry using compute shaders. The fields provide information about the specific triangle that was
    /// hit, as well as the normal and position at the intersection point. The structure is laid out explicitly for
    /// interoperability with native or GPU code. The distance to the hit point is stored in the normal's w component.</remarks>
    [StructLayout(LayoutKind.Explicit, Size = 48)]
    struct ComputeHitResult
    {
        [FieldOffset(0)]
        public int TriangleIndex;
        [FieldOffset(16)]
        public Vector4 Normal;
        [FieldOffset(32)]
        public Vector4 Position;
    }

    /// <summary>
    /// Provides functionality to perform GPU-accelerated raycasting operations against mesh geometry using compute
    /// shaders.
    /// </summary>
    /// <remarks>Use this class to initialize compute-based raycasting with a specific render device and to
    /// perform intersection tests between rays and meshes. The class manages the necessary GPU resources and compute
    /// shader setup for efficient raycast queries. For one-off or stateless raycasts, the static method can be used
    /// without instantiating the class.</remarks>
    public class ComputeRaycast
    {
        private Compute.Shader.RaytestShader _raytestShader;
        private IRenderDevice _renderer;
        private int _resultBuffer;

        /// <summary>
        /// Creates a new instance of the ComputeRaycast class, initializing the internal raytest compute shader.
        /// </summary>
        public ComputeRaycast()
        {
            _raytestShader = new Compute.Shader.RaytestShader();
        }

        /// <summary>
        /// Initializes the ComputeRaycast with the specified render device, setting up necessary GPU resources.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Init(IRenderDevice renderDevice)
        {
            Debug.WriteLine("Initializing ComputeRaycast...");
            _renderer = renderDevice;
            _raytestShader.Init(renderDevice);

            Debug.WriteLine("Creating result buffer...");
            _resultBuffer = renderDevice.CreateBuffer();
            renderDevice.SetBufferData<ComputeHitResult>(_resultBuffer, new ComputeHitResult[1], RenderFlags.GFXBufferTarget.ShaderStorageBuffer, RenderFlags.GFXBufferUsageHint.DynamicRead);
            Debug.WriteLine($"Compute Raycast initialized. Result Buffer ID: {_resultBuffer} Error: {renderDevice.GetError()}");
        }

        /// <summary>
        /// Releases all resources used by the ComputeRaycast instance and associated rendering resources.
        /// </summary>
        /// <remarks>Call this method when the ComputeRaycast instance is no longer needed to free GPU and
        /// shader resources. After calling Dispose, the instance should not be used.</remarks>
        /// <param name="renderer">The render device used to release GPU buffers and related resources. Cannot be null.</param>
        public void Dispose(IRenderDevice renderer)
        {
            Debug.WriteLine("Disposing ComputeRaycast...");
            _renderer.DisposeBuffer(_resultBuffer);
            _raytestShader.Dispose(_renderer);
            _renderer = null; // Clear reference to renderer
            Debug.WriteLine("ComputeRaycast disposed.");
        }

        /// <summary>
        /// Performs a raycast against the specified mesh using a compute shader and returns the result of the
        /// intersection, if any.
        /// </summary>
        /// <remarks>This method uses GPU compute shaders to perform the raycast operation, which can
        /// efficiently handle complex meshes. The mesh's local transform is combined with the provided transform before
        /// testing. The result includes the hit position, normal, and triangle index if an intersection
        /// occurs.</remarks>
        /// <param name="ray">The ray, in world space, to test for intersection with the mesh.</param>
        /// <param name="transform">The transformation to apply to the mesh before performing the raycast. Typically represents the mesh's world
        /// transform.</param>
        /// <param name="mesh">The mesh to test for intersection with the ray.</param>
        /// <returns>A ComputeHitResult structure containing information about the intersection. If no intersection is found, the
        /// TriangleIndex property of the result is set to -1.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the raycast system has not been initialized with a render device.</exception>
        public HitResult PerformRaycast(Ray ray, Transform transform, Mesh mesh)
        {
            if (_renderer == null)
                throw new InvalidOperationException("ComputeRaycast not initialized with a render device.");

            // Combine mesh local transform with provided transform
            Matrix4 modelMatrix = mesh.GetTransform() * transform.GetMatrix();

            // Bind and set up compute shader
            _renderer.BindShaderProgram(_raytestShader);
            _renderer.PrepareShader("rayOrigin", ray.Origin);
            _renderer.PrepareShader("rayDir", ray.Direction);
            _renderer.PrepareShader("modelMatrix", true, modelMatrix);

            // Bind buffers
            _renderer.BindShaderStorageBuffer(0, mesh.RenderData.PositionBuffer);
            _renderer.BindShaderStorageBuffer(1, mesh.RenderData.IndexBuffer);
            _renderer.BindShaderStorageBuffer(2, _resultBuffer);

            // Dispatch compute shader and set memory barrier to ensure completion
            _renderer.DispatchCompute(1, 1, 1);
            _renderer.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

            // Retrieve hit result
            ComputeHitResult hit = _renderer.GetBufferData<ComputeHitResult>(_resultBuffer, 1, RenderFlags.GFXBufferTarget.ShaderStorageBuffer)[0];
            if (hit.TriangleIndex == -1)
            {
                // No hit detected
                return new HitResult { hit = false };
            }

            // Return positive hit result
            return new HitResult
            {
                hit = true,
                rayStart = ray.Origin,
                rayEnd = ray.Origin + ray.Direction * hit.Normal.W,
                hitTriangleIndex = hit.TriangleIndex,
                hitNormal = new Vector3(hit.Normal.X, hit.Normal.Y, hit.Normal.Z),
                hitLocation = new Vector3(hit.Position.X, hit.Position.Y, hit.Position.Z),
                hitDistance = hit.Normal.W
            };
        }

        /// <summary>
        /// Performs a raycast against the specified mesh using a compute shader and returns the result of the
        /// intersection, if any.
        /// </summary>
        /// <remarks>This method uses GPU compute shaders to efficiently test for ray-mesh intersections.
        /// The mesh's local transform is combined with the provided world transform before the raycast. The caller is
        /// responsible for ensuring that the mesh and shader are compatible and properly initialized.</remarks>
        /// <param name="ray">The ray, in world space, to test for intersection with the mesh.</param>
        /// <param name="transform">The world transform to apply to the mesh before performing the raycast.</param>
        /// <param name="mesh">The mesh to test for intersection with the ray.</param>
        /// <param name="renderer">The render device used to manage GPU resources and dispatch the compute shader.</param>
        /// <param name="shader">The compute shader used to perform the ray-mesh intersection test.</param>
        /// <returns>A ComputeHitResult structure containing information about the intersection. If no intersection is found, the
        /// TriangleIndex field will be -1.</returns>
        public static HitResult PerformRaycast(Ray ray, Transform transform, Mesh mesh, IRenderDevice renderer, ComputeShader shader)
        {
            // Combine mesh local transform with provided transform
            Matrix4 modelMatrix = mesh.GetTransform() * transform.GetMatrix();

            // Create temporary buffer for hit result
            int hitBuffer = renderer.CreateBuffer();
            renderer.SetBufferData<ComputeHitResult>(hitBuffer, new ComputeHitResult[1], RenderFlags.GFXBufferTarget.ShaderStorageBuffer, RenderFlags.GFXBufferUsageHint.DynamicRead);

            // Bind and set up compute shader
            renderer.BindShaderProgram(shader);
            renderer.PrepareShader("rayOrigin", ray.Origin);
            renderer.PrepareShader("rayDir", ray.Direction);
            renderer.PrepareShader("modelMatrix", true, modelMatrix);

            // Bind buffers
            renderer.BindShaderStorageBuffer(0, mesh.RenderData.PositionBuffer);
            renderer.BindShaderStorageBuffer(1, mesh.RenderData.IndexBuffer);
            renderer.BindShaderStorageBuffer(2, hitBuffer);

            // Dispatch compute shader and set memory barrier to ensure completion
            renderer.DispatchCompute(1, 1, 1);
            renderer.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

            // Retrieve hit result
            ComputeHitResult hit = renderer.GetBufferData<ComputeHitResult>(hitBuffer, 1, RenderFlags.GFXBufferTarget.ShaderStorageBuffer)[0];

            // Clean up temporary buffer
            renderer.DisposeBuffer(hitBuffer);

            // Check for no hit
            if (hit.TriangleIndex == -1)
            {
                return new HitResult { hit = false };
            }

            // Else , return positive hit result
            return new HitResult
            {
                hit = true,
                rayStart = ray.Origin,
                rayEnd = ray.Origin + ray.Direction * hit.Normal.W,
                hitTriangleIndex = hit.TriangleIndex,
                hitNormal = new Vector3(hit.Normal.X, hit.Normal.Y, hit.Normal.Z),
                hitLocation = new Vector3(hit.Position.X, hit.Position.Y, hit.Position.Z),
                hitDistance = hit.Normal.W
            };
        }
    }
}
