using BulletSharp;
using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics
{
    /// <summary>
    /// Represents a debug drawer
    /// </summary>
    public class DebugDrawer : DebugDraw
    {
        /// <summary>
        /// Gets or sets the debug color
        /// </summary>
        public OpenTK.Mathematics.Vector4 DebugColor  { get; set; }

        private IRenderDevice _renderer;
        private DebugDrawModes _debugMode = DebugDrawModes.All;

        /// <summary>
        /// Creates a new debug drawer
        /// </summary>
        /// <param name="RenderDevice"></param>
        public DebugDrawer(IRenderDevice RenderDevice)
        {
            _renderer = RenderDevice;
            DebugColor = new OpenTK.Mathematics.Vector4(0.0f, 1.0f, 0.0f, 1.0f);
        }

        /// <summary>
        /// Gets or sets the debug mode
        /// </summary>
        public override DebugDrawModes DebugMode
        {
            get => _debugMode; set => _debugMode = value;
        }

        /// <summary>
        /// Draws a 3D text
        /// </summary>
        /// <param name="location"></param>
        /// <param name="textString"></param>
        public override void Draw3DText(ref Vector3 location, string textString)
        {

        }

        /// <summary>
        /// Draws an line
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        public override void DrawLine(ref Vector3 from, ref Vector3 to, ref Vector3 color)
        {
            var shader = _renderer.GetShaderProgram("LineShader");
            _renderer.BindShaderProgram(shader);
            _renderer.DrawLine((OpenTK.Mathematics.Vector3) from, (OpenTK.Mathematics.Vector3) to, DebugColor);
            _renderer.UnbindShaderProgram();
        }

        /// <summary>
        /// Draws a contact point
        /// </summary>
        /// <param name="warningString"></param>
        public override void ReportErrorWarning(string warningString)
        {
            Debug.WriteLine(warningString);
        }
    }
}
