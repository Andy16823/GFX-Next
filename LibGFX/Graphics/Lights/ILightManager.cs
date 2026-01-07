using LibGFX.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    /// <summary>
    /// Interface for managing lights in a scene.
    /// </summary>
    public interface ILightManager : ISerialization, IGraphicsResource
    {
        /// <summary>
        /// Initializes the light manager with the given render device.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Init(IRenderDevice renderDevice);

        /// <summary>
        /// Culls the lights based on the camera's view and the viewport.
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public void CullLights(Viewport viewport, IRenderDevice renderer, Camera camera);

        /// <summary>
        /// Binds the lights to the shader program for rendering.
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public void BindLights(Viewport viewport, IRenderDevice renderer, Camera camera);

        /// <summary>
        /// Disposes of the light manager and releases any resources.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Dispose(IRenderDevice renderDevice);

        /// <summary>
        /// Gets the ligt count of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int GetLightCount<T>() where T : Light;

        /// <summary>
        /// Gets the total light count across all types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetLight<T>() where T : Light;

        /// <summary>
        /// Performs an action on each light of the specified type in the scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public void ForEachLight<T>(Action<T> action) where T : Light;

        /// <summary>
        /// Performs an action on each light in the scene, regardless of type.
        /// </summary>
        /// <param name="action"></param>
        public void ForEachLight(Action<Light> action);

        /// <summary>
        /// Sets the light view matrix for the light manager, which is used to transform the light's perspective in the scene.
        /// </summary>
        /// <param name="lightViewMatrix"></param>
        public void SetLightSpaceMatrix(Matrix4 lightViewMatrix);

        /// <summary>
        /// Releases all light resources associated with the specified render device.
        /// </summary>
        /// <param name="renderDevice">The render device whose light resources will be disposed. Cannot be null.</param>
        public void DisposeLights(IRenderDevice renderDevice);

        /// <summary>
        /// Determines whether the specified light is contained within the collection.
        /// </summary>
        /// <param name="light">The light to locate in the collection. Cannot be null.</param>
        /// <returns>true if the specified light is found in the collection; otherwise, false.</returns>
        public bool ContainsLight(Light light);

        /// <summary>
        /// Removes all lights from the collection.
        /// </summary>
        /// <remarks>Call this method to reset the collection to an empty state. After calling this
        /// method, no lights will remain until new ones are added.</remarks>
        public void ClearLights();

        /// <summary>
        /// Removes the specified light from the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="light"></param>
        public void RemoveLight<T>(T light) where T : Light;
    }
}
