using Assimp.Configs;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics.Materials;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Animation2D
{
    /// <summary>
    /// Represents a 2D animation. Based on a sprite sheet.
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// The name of the animation.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The material used for the animation.
        /// </summary>
        public SpriteMaterial Material { get; set; }

        /// <summary>
        /// The cell width of the animation sheet.
        /// </summary>
        public int CellWidth { get; set; }

        /// <summary>
        /// The cell height of the animation sheet.
        /// </summary>
        public int CellHeight { get; set; }

        /// <summary>
        /// The starting column of the animation.
        /// </summary>
        public int StartColumn { get; set; }

        /// <summary>
        /// The starting row of the animation.
        /// </summary>
        public int StartRow { get; set; }

        /// <summary>
        /// The total number of columns in the animation.
        /// </summary>
        public int TotalColumns { get; set; }

        /// <summary>
        /// The total number of rows in the animation.
        /// </summary>
        public int TotalRows { get; set; }

        /// <summary>
        /// The total number of frames in the animation.
        /// </summary>
        public int TotalFrames { get; set; }

        /// <summary>
        /// The current frame of the animation.
        /// </summary>
        public int CurrentFrame { get; set; }

        /// <summary>
        /// The framerate of the animation in frames per second.
        /// </summary>
        public float Framerate { get; set; }

        // The last frame update time
        private long _lastFrame = 0;

        /// <summary>
        /// Creates a new animation.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cellWidth"></param>
        /// <param name="cellHeight"></param>
        /// <param name="startColumn"></param>
        /// <param name="startRow"></param>
        /// <param name="frames"></param>
        /// <param name="framerate"></param>
        /// <param name="material"></param>
        public Animation(String name, int cellWidth, int cellHeight, int startColumn, int startRow, int frames, float framerate, SpriteMaterial material)
        {
            this.Name = name;
            this.Material = material;
            this.CellWidth = cellWidth;
            this.CellHeight = cellHeight;
            this.StartColumn = startColumn;
            this.StartRow = startRow;
            this.TotalFrames = frames;
            this.Framerate = framerate;

            this.TotalColumns = material.Texture.Width / cellWidth;
            this.TotalRows = material.Texture.Height / cellHeight;
        }

        /// <summary>
        /// Updates the animation. It updates the current frame based on the framerate and the time passed since the last frame update.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime, out bool finished)
        {
            finished = false;
            var now = Utils.GetCurrentTimeMillis();
            var delay = (long)(1000.0f / Framerate);

            // Check if enough time has passed since the last frame update
            if (now - _lastFrame < delay)
                return;

            // Update the current frame
            this.CurrentFrame++;
            if (this.CurrentFrame >= TotalFrames)
            {
                this.CurrentFrame = 0;
                finished = true;
            }
            _lastFrame = now;
        }

        /// <summary>
        /// Gets the uv transform of the current frame.
        /// </summary>
        /// <returns></returns>
        public Vector4 GetUVTransform()
        {
            int column = (CurrentFrame % TotalColumns) + StartColumn;
            return Material.Texture.GetSubImage(new LibGFX.Math.Rect(column * CellWidth, StartRow * CellHeight, CellWidth, CellHeight));
        }
    }
}
