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
    /// An enum with the states of the mesh
    /// </summary>
    public enum MeshState
    {
        None,
        Initialized,
        Disposed
    }

    /// <summary>
    /// Represents a vertex for the rendering pipeline
    /// </summary>
    public struct Vertex
    {
        public Vector3 Position;
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector4 Tangent;
        public Vector4i BoneIDs;
        public Vector4 BoneWeights;
    }

    /// <summary>
    /// Represents a bone information for the rendering pipeline
    /// </summary>
    public struct BoneInfo
    {
        public int id;
        public Matrix4 offset;
    }

    /// <summary>
    /// Represents a mesh for the rendering pipeline
    /// </summary>
    public class Mesh
    {
        /// <summary>
        /// The name of the mesh.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The unique identifier of the mesh.
        /// </summary>
        public Guid ID { get; } = Guid.NewGuid();

        /// <summary>
        /// The vertices of the mesh.
        /// </summary>
        public List<Vertex> Vertices { get; set; }

        /// <summary>
        /// The indices of the mesh.
        /// </summary>
        public List<int> Indices { get; set; }

        /// <summary>
        /// The local translation of the mesh.
        /// </summary>
        public Vector3 LocalTranslation { get; set; }

        /// <summary>
        /// the local rotation of the mesh.
        /// </summary>
        public Quaternion LocalRotation { get; set; }

        /// <summary>
        /// The local scale of the mesh.
        /// </summary>
        public Vector3 LocalScale { get; set; }

        /// <summary>
        /// The render data associated with the mesh.
        /// </summary>
        public RenderData RenderData { get; set; }

        /// <summary>
        /// The state of the mesh.
        /// </summary>
        public MeshState State { get; set; } = MeshState.None;

        public Mesh()
        {
            Vertices = new List<Vertex>();
            Indices = new List<int>();
            LocalTranslation = Vector3.Zero;
            LocalRotation = Quaternion.Identity;
            LocalScale = Vector3.One;
            RenderData = new RenderData();
        }

        public Matrix4 GetTransform()
        {
            Matrix4 translation = Matrix4.CreateTranslation(LocalTranslation);
            Matrix4 rotation = Matrix4.CreateFromQuaternion(LocalRotation);
            Matrix4 scale = Matrix4.CreateScale(LocalScale);
            return scale * rotation * translation;
        }
    }
}
