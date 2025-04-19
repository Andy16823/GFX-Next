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
    /// Represents the state of the instance container.
    /// </summary>
    public enum InstanceContainerState
    {
        None,
        Initialized,
        Bound,
        Disposed
    }

    /// <summary>
    /// Represents a container for render instances.
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
        /// The state of the instance container.
        /// </summary>
        public InstanceContainerState State { get; set; } = InstanceContainerState.None;

        /// <summary>
        /// The mesh associated with the instance container.
        /// </summary>
        public Mesh Mesh { get; set; }

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
        public (Matrix4[], float[], Vector4[]) GetInstancesBuffers()
        {
            List<Matrix4> matrices = new List<Matrix4>();
            List<float> extras = new List<float>();
            List<Vector4> uvTransforms = new List<Vector4>();

            foreach (var instance in this.Instances)
            {
                matrices.Add(instance.GetMatrix());
                var extra = instance.GetExtras();
                extras.Add(extra.X);
                extras.Add(extra.Y);
                extras.Add(extra.Z);
                extras.Add(extra.W);

                uvTransforms.Add(instance.UVTransofrom);
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
    }
}
