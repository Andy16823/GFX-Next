using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a container for render instances.#
    /// TODO: Why is this an IAsset? It doesn't have any asset-like properties or behaviors. Maybe it should be a regular class instead of implementing IAsset?
    /// </summary>
    public class RenderInstanceContainer
    {
        /// <summary>
        /// The Vertex Array Object (VAO) for the instance container.
        /// </summary>
        public int InstanceVAO { get; set; }

        /// <summary>
        /// The buffer for the model matrix of the instances.
        /// </summary>
        public int TransformInstanceBuffer { get; set; }

        /// <summary>
        /// The buffer for the extra instance data.
        /// </summary>
        public int ExtraInstanceBuffer { get; set; }

        /// <summary>
        /// The buffer for the UV transform data of the instances.
        /// </summary>
        public int UVInstanceBuffer { get; set; }

        /// <summary>
        /// The list of render instances in the container.
        /// </summary>
        public List<RenderInstance> Instances { get; set; }

        /// <summary>
        /// The mesh associated with the instance container.
        /// </summary>
        public Mesh Mesh { get; set; }

        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Creates a new instance of the RenderInstanceContainer class.
        /// </summary>
        public RenderInstanceContainer()
        {
            Instances = new List<RenderInstance>();
        }

        /// <summary>
        /// Gets the model matrices and extra data for the instances in the container.
        /// </summary>
        /// <returns></returns>
        public (Matrix4[], Vector4[], Vector4[]) GetInstancesBuffers()
        {
            List<Matrix4> matrices = new List<Matrix4>();
            List<Vector4> extras = new List<Vector4>();
            List<Vector4> uvTransforms = new List<Vector4>();

            foreach (var instance in this.Instances)
            {
                matrices.Add(instance.GetMatrix());
                var extra = instance.GetExtras();
                extras.Add(extra);
                uvTransforms.Add(instance.UVTransform);
            }

            return (matrices.ToArray(), extras.ToArray(), uvTransforms.ToArray());
        }

        /// <summary>
        /// Adds a new instance to the container.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="visibility"></param>
        /// <returns></returns>
        public int AddInstance(Transform transform, bool visibility)
        {
            var instance = new RenderInstance();
            instance.Transform = transform;
            instance.Visible = visibility;
            this.Instances.Add(instance);
            return this.Instances.Count - 1;
        }

        /// <summary>
        /// Initializes the instance container by loading it into the render device. This typically involves creating the necessary buffers and setting up the vertex array object (VAO) for rendering the instances.
        /// </summary>
        /// <param name="renderer"></param>
        public void Init(IRenderDevice renderer)
        {
            renderer.LoadInstanceContainer(this);
            this.IsInitialized = true;
        }

        /// <summary>
        /// Disposes of the instance container by releasing any resources associated with it in the render device. This typically involves deleting the buffers and vertex array object (VAO) used for rendering the instances.
        /// </summary>
        /// <param name="renderer"></param>
        public void Dispose(IRenderDevice renderer)
        {
            renderer.DisposeInstanceContainer(this);
            this.IsInitialized = false;
        }
    }
}
