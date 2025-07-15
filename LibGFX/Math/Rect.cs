using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Math
{
    /// <summary>
    /// Represents a rectangle defined by its position and size.
    /// </summary>
    public struct Rect
    {
        /// <summary>
        /// The X coordinate of the rectangle's position.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// The Y coordinate of the rectangle's position.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// The width of the rectangle.
        /// </summary>
        public float Width { get; set; }
        
        /// <summary>
        /// The height of the rectangle.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rect"/> struct with default values.
        /// </summary>
        public Rect()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rect"/> struct with specified position and size.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rect(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Checks if the rectangle contains a point defined by its X and Y coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Contains(float x, float y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
        }

        /// <summary>
        /// Checks if the rectangle contains a point defined by a Vector2.
        /// </summary>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public bool Contains(Vector2 vector2)
        {
            return Contains(vector2.X, vector2.Y);
        }

        /// <summary>
        /// Checks if the rectangle intersects with another rectangle.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Intersects(Rect other)
        {
            return X < other.X + other.Width &&
                   X + Width > other.X &&
                   Y < other.Y + other.Height &&
                   Y + Height > other.Y;
        }
    }
}
