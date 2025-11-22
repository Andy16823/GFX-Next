using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    /// <summary>
    /// An directional light in 3D space.
    /// TODO: Create an uniform for the bias
    /// </summary>
    public class DirectionalLight3D : Light
    {
        /// <summary>
        /// The direction of the light.
        /// </summary>
        public Vector3 Direction { get; set; }

        /// <summary>
        /// The ambient color of the light.
        /// </summary>
        public Vector3 Ambient { get; set; }

        /// <summary>
        /// The specular color of the light.
        /// </summary>
        public Vector3 Specular { get; set; }

        /// <summary>
        /// The Bias of the light shadow map.
        /// </summary>
        public float Bias { get; set; } = 0.005f;

        public override bool HasShadowMap => true;

        /// <summary>
        /// Creates a new instance of the <see cref="DirectionalLight3D"/> class.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        public DirectionalLight3D(Vector3 direction, Vector4 color, float intensity)
        {
            Position = Vector3.PositiveInfinity;
            Direction = direction;
            Color = color;
            Intensity = intensity;
            Ambient = new Vector3(0.2f, 0.2f, 0.2f);
            Specular = new Vector3(1.0f, 1.0f, 1.0f);
        }

        public override void Init(IRenderDevice renderer)
        {
            Debug.WriteLine($"Creating Shadow Map for Directional Light: {this.GetType().Name} at {Position} with size {ShadowMapSize}");
            this.ShadowMap = renderer.CreateDepthRenderTarget2D(ShadowMapSize.X, ShadowMapSize.Y);
        }

        public override void Dispose(IRenderDevice renderer)
        {
            Debug.WriteLine($"Disposing Shadow Map for Directional Light: {this.GetType().Name} at {Position}");
            this.ShadowMap.Dispose(renderer);
        }
    }
}
