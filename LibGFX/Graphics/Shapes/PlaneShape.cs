using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shapes
{
    /// <summary>
    /// Represents a geometric plane shape defined by four vertices, suitable for rendering or geometric computations in
    /// 3D space.
    /// </summary>
    /// <remarks>The plane is oriented along the XZ axis at Y = 0, with vertices arranged in a
    /// counter-clockwise order. This class provides mesh data such as vertices, normals, tangents, texture coordinates,
    /// and indices for use in graphics or physics applications. All mesh data is returned in arrays corresponding to
    /// the four corners of the plane.</remarks>
    public class PlaneShape : Shape
    {
        /// <summary>
        /// Returns the number of indices used by this geometry.
        /// </summary>
        /// <returns>An integer representing the total number of indices. Always returns 6.</returns>
        public override int GetIndexCount()
        {
            return 6;
        }

        /// <summary>
        /// Returns the array of vertex indices that define the triangles composing the mesh.
        /// </summary>
        /// <remarks>The returned indices are ordered to form two triangles, suitable for rendering a quad
        /// as two triangles in graphics APIs. The order of indices determines the winding and orientation of the
        /// triangles.</remarks>
        /// <returns>An array of unsigned integers representing the indices of vertices for each triangle in the mesh. The array
        /// contains six elements, corresponding to two triangles.</returns>
        public override uint[] GetIndices()
        {
            return new uint[]
            {
                0, 1, 2,    // First triangle (Back-left, Back-right, Front-right)
                2, 3, 0     // Second triangle (Front-right, Front-left, Back-left)
            };
        }

        /// <summary>
        /// Returns an array of normal vectors for the surface, with each normal corresponding to a vertex.
        /// </summary>
        /// <returns>An array of four single-precision floating-point values representing the normal vectors for the back-left,
        /// back-right, front-right, and front-left vertices. Each normal points upward along the Y-axis.</returns>
        public override float[] GetNormals()
        {
            return new float[]
            {
                0.0f, 1.0f, 0.0f,    // Back-left
                0.0f, 1.0f, 0.0f,    // Back-right
                0.0f, 1.0f, 0.0f,    // Front-right
                0.0f, 1.0f, 0.0f     // Front-left
            };
        }

        /// <summary>
        /// Returns the name of the shape represented by this instance.
        /// </summary>
        /// <returns>A string that contains the name "PlaneShape".</returns>
        public override string GetShapeName()
        {
            return "PlaneShape";
        }

        /// <summary>
        /// Returns an array of tangent vectors for the mesh geometry.
        /// </summary>
        /// <remarks>The returned array contains tangent data for four vertices, with each tangent
        /// represented by four consecutive float values (x, y, z, w). The handedness (w component) is used for normal
        /// mapping calculations.</remarks>
        /// <returns>An array of four-element floats representing the tangent vectors for each vertex. Each group of four floats
        /// corresponds to the tangent vector and handedness for a vertex.</returns>
        public override float[] GetTangents()
        {
            return new float[]
            {
                1.0f, 0.0f, 0.0f, 1.0f, // Back-left
                1.0f, 0.0f, 0.0f, 1.0f, // Back-right
                1.0f, 0.0f, 0.0f, 1.0f, // Front-right
                1.0f, 0.0f, 0.0f, 1.0f  // Front-left
            };
        }

        /// <summary>
        /// Returns the normalized UV coordinates for the four corners of a quadrilateral surface.
        /// </summary>
        /// <remarks>The returned coordinates are suitable for mapping a texture onto a rectangular
        /// surface, with each pair corresponding to a corner in normalized (0.0 to 1.0) UV space.</remarks>
        /// <returns>An array of four pairs of floating-point values representing the UV coordinates for the back-left,
        /// back-right, front-right, and front-left corners, in that order.</returns>
        public override float[] GetUVCoords()
        {
            return new float[]
            {
                0.0f, 0.0f,    // Back-left
                1.0f, 0.0f,    // Back-right
                1.0f, 1.0f,    // Front-right
                0.0f, 1.0f     // Front-left
            };
        }

        /// <summary>
        /// Returns the vertex positions for a flat quadrilateral in 3D space.
        /// </summary>
        /// <remarks>The returned vertices define a quadrilateral lying in the XZ plane at Y = 0, with
        /// each corner at a distance of 0.5 units from the origin along the X and Z axes. This method is typically used
        /// for rendering or geometry calculations where a flat surface is required.</remarks>
        /// <returns>An array of four 3D vertex positions, each represented by three consecutive float values (X, Y, Z). The
        /// vertices are ordered as back-left, back-right, front-right, and front-left.</returns>
        public override float[] GetVertices()
        {
            return new float[]
            {
                -0.5f, 0.0f, -0.5f,     // Back-left
                 0.5f, 0.0f, -0.5f,     // Back-right
                 0.5f, 0.0f,  0.5f,     // Front-right
                -0.5f, 0.0f,  0.5f      // Front-left
            };
        }
    }
}
