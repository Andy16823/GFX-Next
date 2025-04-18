using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public enum InstanceContainerState
    {
        None,
        Initialized,
        Bound,
        Disposed
    }

    public class RenderInstanceContainer
    {
        public int InstanceVAO { get; set; }
        public int TransformInstanceBuffer { get; set; }
        public int ExtraInstanceBuffer { get; set; }
        public List<RenderInstance> Instances { get; set; }
        public InstanceContainerState State { get; set; } = InstanceContainerState.None;
        public Mesh Mesh { get; set; }

        public RenderInstanceContainer()
        {
            Instances = new List<RenderInstance>();
        }

        public (Matrix4[], float[]) GetInstancesBuffers()
        {
            List<Matrix4> matrices = new List<Matrix4>();
            List<float> extras = new List<float>();

            foreach (var instance in this.Instances)
            {
                matrices.Add(instance.GetMatrix());
                var extra = instance.GetExtras();
                extras.AddRange(extras.ToArray());
            }

            return (matrices.ToArray(), extras.ToArray());
        }
    }
}
