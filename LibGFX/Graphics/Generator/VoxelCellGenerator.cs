using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Generator
{
    public class VoxelCellGenerator
    {
        [Flags]
        public enum VoxelCellFace
        {
            Front = 1 << 0,
            Back = 1 << 1,
            Left = 1 << 2,
            Right = 1 << 3,
            Top = 1 << 4,
            Bottom = 1 << 5
        }

        public static int[] CreateVisibility(Vector3i cells)
        {
            int totalCells = cells.X * cells.Y * cells.Z;
            return Enumerable.Repeat(1, totalCells).ToArray();
        }

        public static float[] CreateVoxelVertices(Vector3i cells, int[] visibility, float cellSize = 1.0f)
        {
            var vertices = new List<float>();
            for (int x = 0; x < cells.X; x++)
            {
                for (int y = 0; y < cells.Y; y++)
                {
                    for (int z = 0; z < cells.Z; z++)
                    {
                        // Current cell index
                        int index = (int)(x + y * cells.X + z * cells.X * cells.Y);
                        if (visibility[index] == 0)
                            continue;

                        // Determine which faces are visible
                        VoxelCellFace faces = 0;
                        if(IsNeighborHidden(visibility, cells, x, y, z + 1))
                            faces |= VoxelCellFace.Front;
                        if(IsNeighborHidden(visibility, cells, x, y, z - 1))
                            faces |= VoxelCellFace.Back;
                        if(IsNeighborHidden(visibility, cells, x - 1, y, z))
                            faces |= VoxelCellFace.Left;
                        if(IsNeighborHidden(visibility, cells, x + 1, y, z))
                            faces |= VoxelCellFace.Right;
                        if(IsNeighborHidden(visibility, cells, x, y + 1, z))
                            faces |= VoxelCellFace.Top;
                        if(IsNeighborHidden(visibility, cells, x, y - 1, z))
                            faces |= VoxelCellFace.Bottom;

                        // Generate vertices for visible faces
                        if (faces != 0)
                        {
                            Vector3 center = new Vector3(
                                x * cellSize + cellSize / 2,
                                y * cellSize + cellSize / 2,
                                z * cellSize + cellSize / 2
                            );
                            vertices.AddRange(CellVertices(center, faces, cellSize / 2));
                        }
                    }
                }
            }
            return vertices.ToArray();
        }

        private static bool IsNeighborHidden(int[] visibility, Vector3i cells, int x, int y, int z)
        {
            // Check bounds
            if (x < 0 || x >= cells.X || 
                y < 0 || y >= cells.Y || 
                z < 0 || z >= cells.Z)
                return true; 

            int index = (int)(x + y * cells.X + z * cells.X * cells.Y);
            return visibility[index] == 0;
        }

        public static float[] CellVertices(Vector3 center, VoxelCellFace faces, float halfSize = 0.5f)
        {
            var vertices = new List<float>();

            // Generate vertices for each face
            if (faces.HasFlag(VoxelCellFace.Front))
            {
                float[] verts = [
                    // First triangle
                    center.X - halfSize, center.Y - halfSize, center.Z + halfSize, // Front left bottom
                    center.X + halfSize, center.Y - halfSize, center.Z + halfSize, // Front right bottom
                    center.X + halfSize, center.Y + halfSize, center.Z + halfSize, // Front right top

                    // Second triangle
                    center.X + halfSize, center.Y + halfSize, center.Z + halfSize, // Front right top
                    center.X - halfSize, center.Y + halfSize, center.Z + halfSize,  // Front left top
                    center.X - halfSize, center.Y - halfSize, center.Z + halfSize  // Front left bottom
                ];
                vertices.AddRange(verts);
            }
            if (faces.HasFlag(VoxelCellFace.Back))
            {
                float[] verts = [
                    // First triangle
                    center.X - halfSize, center.Y - halfSize, center.Z - halfSize, // Back left bottom
                    center.X + halfSize, center.Y - halfSize, center.Z - halfSize, // Back right bottom
                    center.X + halfSize, center.Y + halfSize, center.Z - halfSize, // Back right top

                    // Second triangle
                    center.X + halfSize, center.Y + halfSize, center.Z - halfSize, // Back right top
                    center.X - halfSize, center.Y + halfSize, center.Z - halfSize, // Back left top
                    center.X - halfSize, center.Y - halfSize, center.Z - halfSize  // Back left bottom
                ];
                vertices.AddRange(verts);
            }
            if (faces.HasFlag(VoxelCellFace.Left))
            {
                float[] verts = [
                    // First triangle
                    center.X - halfSize, center.Y - halfSize, center.Z - halfSize, // Left back bottom
                    center.X - halfSize, center.Y - halfSize, center.Z + halfSize, // Left front bottom
                    center.X - halfSize, center.Y + halfSize, center.Z + halfSize, // Left front top

                    // Second triangle
                    center.X - halfSize, center.Y + halfSize, center.Z + halfSize, // Left front top
                    center.X - halfSize, center.Y + halfSize, center.Z - halfSize,  // Left back top
                    center.X - halfSize, center.Y - halfSize, center.Z - halfSize  // Left back bottom
                ];
                vertices.AddRange(verts);
            }
            if (faces.HasFlag(VoxelCellFace.Right))
            {
                float[] verts = [
                    // First triangle
                    center.X + halfSize, center.Y - halfSize, center.Z + halfSize, // Right front bottom
                    center.X + halfSize, center.Y - halfSize, center.Z - halfSize, // Right back bottom
                    center.X + halfSize, center.Y + halfSize, center.Z - halfSize, // Right back top

                    // Second triangle
                    center.X + halfSize, center.Y + halfSize, center.Z - halfSize, // Right back top
                    center.X + halfSize, center.Y + halfSize, center.Z + halfSize, // Right front top
                    center.X + halfSize, center.Y - halfSize, center.Z + halfSize  // Right front bottom
                ];
                vertices.AddRange(verts);
            }
            if (faces.HasFlag(VoxelCellFace.Top))
            {
                float[] verts = [
                    // First triangle
                    center.X - halfSize, center.Y + halfSize, center.Z + halfSize, // Top front left
                    center.X + halfSize, center.Y + halfSize, center.Z + halfSize, // Top front right
                    center.X + halfSize, center.Y + halfSize, center.Z - halfSize, // Top back right

                    // Second triangle
                    center.X + halfSize, center.Y + halfSize, center.Z - halfSize, // Top back right
                    center.X - halfSize, center.Y + halfSize, center.Z - halfSize, // Top back left
                    center.X - halfSize, center.Y + halfSize, center.Z + halfSize, // Top front left
                ];
                vertices.AddRange(verts);
            }
            if (faces.HasFlag(VoxelCellFace.Bottom))
            {
                float[] verts = [
                    // First triangle
                    center.X - halfSize, center.Y - halfSize, center.Z - halfSize, // Bottom back left
                    center.X - halfSize, center.Y - halfSize, center.Z + halfSize, // Bottom front left
                    center.X + halfSize, center.Y - halfSize, center.Z + halfSize, // Bottom front right

                    // Second triangle
                    center.X + halfSize, center.Y - halfSize, center.Z + halfSize, // Bottom front right
                    center.X + halfSize, center. Y - halfSize, center.Z - halfSize, // Bottom back right
                    center.X - halfSize, center.Y - halfSize, center.Z - halfSize  // Bottom back left
                ];
                vertices.AddRange(verts);
            }

            return vertices.ToArray();
        }
    }
}
