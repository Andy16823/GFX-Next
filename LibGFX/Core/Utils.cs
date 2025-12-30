using LibGFX.Graphics;
using LibGFX.Graphics.Animation3D;
using LibGFX.Graphics.Materials;
using LibGFX.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static LibGFX.Graphics.RenderFlags;

namespace LibGFX.Core
{
    /// <summary>
    /// Utility class for various helper functions
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Creates an empty normal map with the given width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap CreateEmptyNormalMap(int width, int height)
        {
            Bitmap normalMap = new Bitmap(width, height);

            Color normalColor = Color.FromArgb(128, 128, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    normalMap.SetPixel(x, y, normalColor);
                }
            }

            return normalMap;
        }

        /// <summary>
        /// Creates image data (byte array) filled with the given color in RGBA format.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static byte[] CreateImageData(int width, int height, Vector4i color)
        {
            byte[] pixels = new byte[width * height * 4];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width + x) * 4;
                    pixels[index + 0] = (byte)color.X; // R
                    pixels[index + 1] = (byte)color.Y; // G
                    pixels[index + 2] = (byte)color.Z; // B
                    pixels[index + 3] = (byte)color.W; // A
                }
            }
            return pixels;
        }

        /// <summary>
        /// Gets the bullet transform matrix for the given element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="offsetLocation"></param>
        /// <returns></returns>
        public static System.Numerics.Matrix4x4 GetBtTransform(GameElement element, Vector3 offsetLocation = default)
        {
            var location = (System.Numerics.Vector3)element.Transform.Position + (System.Numerics.Vector3)offsetLocation;
            var rotation = (System.Numerics.Quaternion)element.Transform.Rotation;

            var btTranslation = System.Numerics.Matrix4x4.CreateTranslation(location);
            var btRotation = System.Numerics.Matrix4x4.CreateFromQuaternion(rotation);

            return btRotation * btTranslation;
        }

        /// <summary>
        /// Gets the bullet transform matrix for the given location and rotation.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="rotation"></param>
        /// <param name="offsetLocation"></param>
        /// <returns></returns>
        public static System.Numerics.Matrix4x4 GetBtTransform(System.Numerics.Vector3 location, System.Numerics.Quaternion rotation, Vector3 offsetLocation = default)
        {
            var btTranslation = System.Numerics.Matrix4x4.CreateTranslation(location + (System.Numerics.Vector3)offsetLocation);
            var btRotation = System.Numerics.Matrix4x4.CreateFromQuaternion(rotation);
            return btRotation * btTranslation;
        }

        /// <summary>
        /// Applies a scale to the given transform matrix for the phyiscs.
        /// You should not use this method outside of compound shapes. On single shapes, the scale is applied directly to the shape.
        /// This funtion is used to apply the correct non uniform scale to the transform matrix.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static System.Numerics.Matrix4x4 ApplyScale(System.Numerics.Matrix4x4 transform, System.Numerics.Vector3 scale)
        {
            // Erstelle eine Skalierungsmatrix
            var scaleMatrix = System.Numerics.Matrix4x4.CreateScale(scale);
            // Wende die Skalierung auf die bestehende Transformationsmatrix an
            return transform * scaleMatrix;
        }

        /// <summary>
        /// Converts a Vector3 to radians.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Vector3 ToRadians(Vector3 input)
        {
            return new Vector3(MathHelper.DegreesToRadians(input.X), MathHelper.DegreesToRadians(input.Y), MathHelper.DegreesToRadians(input.Z));
        }

        public static float CalculateYaw(Vector3 point1, Vector3 point2)
        {
            // Berechne die Differenzen zwischen den Koordinaten
            double deltaX = point2.X - point1.X;
            double deltaZ = point2.Z - point1.Z;

            // Verwende Atan2, um den Winkel zu berechnen (in Radian)
            float radians = (float)System.Math.Atan2(deltaZ, deltaX);

            // Konvertiere den Winkel von Radian nach Grad
            float angle = (float)(radians * (180 / System.Math.PI));

            return angle;
        }

        /// <summary>
        /// Calculates the pitch angle from point1 to point2.
        /// </summary>
        /// <param name="point1">The starting point.</param>
        /// <param name="point2">The target point.</param>
        /// <returns>The pitch angle in degrees.</returns>
        public static float CalculatePitch(Vector3 point1, Vector3 point2)
        {
            // Berechne die Differenzen zwischen den Koordinaten
            double deltaY = point2.Y - point1.Y;
            double horizontalDistance = System.Math.Sqrt((point2.X - point1.X) * (point2.X - point1.X) + (point2.Z - point1.Z) * (point2.Z - point1.Z));

            // Verwende Atan2, um den Pitch-Winkel zu berechnen (in Radian)
            float radians = (float)System.Math.Atan2(deltaY, horizontalDistance);

            // Konvertiere den Winkel von Radian nach Grad
            float pitch = (float)(radians * (180 / System.Math.PI));

            return pitch;
        }

        public static Quaternion LookAt(Vector3 campos, Vector3 targetpos)
        {
            // Berechne den Vektor von der Kamera zum Ziel
            var direction = targetpos - campos;

            // Erzeuge eine Matrix, die die Kamera in die richtige Richtung ausrichtet
            var matrix = Matrix4.LookAt(campos, campos + direction, -Vector3.UnitY);

            // Extrahiere die Rotation der Matrix und gebe sie als Quaternion zurück
            return matrix.ExtractRotation();
        }

        public static Texture LoadTextureIfExists(JObject jsonObject, string propertyName, string basePath)
        {
            if (jsonObject[propertyName].Value<String>() != "null")
            {
                var texturePath = Path.Combine(basePath, jsonObject[propertyName].Value<String>());
                return new Texture(texturePath);
            }
            return null;
        }

        public static Vector4 GetUVTransformFromTilemap(int width, int height, int cellWidth, int cellHeight, int tileX, int tileY)
        {
            float cellsX = (float)width / cellWidth;
            float cellsY = (float)height / cellHeight;

            float cellX = (float)tileX * cellWidth;
            float flippedTileY = cellsY - tileY - 1;
            float cellY = flippedTileY * cellHeight;

            float scaleX = (float)cellWidth / width;
            float scaleY = (float)cellHeight / height;
            float offsetX = (float)cellX / width;
            float offsetY = (float)cellY / height;

            return new Vector4(scaleX, scaleY, offsetX, offsetY);
        }

        /// <summary>
        /// Creates a new Bitmap from a byte array containing pixel data in BGRA format with 32 bits per pixel.
        /// </summary>
        /// <remarks>The resulting bitmap is vertically flipped to match the coordinate origin used by
        /// OpenGL, where the origin is at the bottom-left corner. The input pixel data must be in row-major order, with
        /// each pixel represented by four bytes (blue, green, red, alpha).</remarks>
        /// <param name="pixels">An array of bytes representing the pixel data in BGRA order. The array length must be at least width ×
        /// height × 4.</param>
        /// <param name="width">The width, in pixels, of the resulting bitmap. Must be greater than 0.</param>
        /// <param name="height">The height, in pixels, of the resulting bitmap. Must be greater than 0.</param>
        /// <returns>A Bitmap object containing the image represented by the specified BGRA pixel data.</returns>
        public static System.Drawing.Bitmap ByteBGRAToBitmap(byte[] pixels, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            // Daten kopieren
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            bitmap.UnlockBits(data);

            // Vertikal spiegeln – OpenGLs Ursprung ist unten links
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return bitmap;
        }

        /// <summary>
        /// Gets the current time in milliseconds since the Unix epoch.
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentTimeMillis()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Converts a local position to a world position using the given transform.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="localPosition"></param>
        /// <returns></returns>
        public static Vector3 LocalToWorldPositon(Transform transform, Vector3 localPosition)
        {
            var modelMatrix = transform.GetMatrix();
            var worldPosition = Vector3.TransformPosition(localPosition, modelMatrix);
            return worldPosition;
        }

        /// <summary>
        /// Converts a world position to a local position using the given transform.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static Vector3 WorldToLocalPosition(Transform transform, Vector3 worldPosition)
        {
            var modelMatrix = transform.GetMatrix();
            var inverseModelMatrix = Matrix4.Invert(modelMatrix);
            var localPosition = Vector3.TransformPosition(worldPosition, inverseModelMatrix);
            return localPosition;
        }

        /// <summary>
        /// Selects the most appropriate depth and/or stencil renderbuffer storage format based on the specified
        /// requirements.
        /// </summary>
        /// <param name="depth">true to request a format that supports depth buffering; otherwise, false.</param>
        /// <param name="stencil">true to request a format that supports stencil buffering; otherwise, false.</param>
        /// <returns>A GFXRenderbufferStorage value representing the best matching format for the requested depth and stencil
        /// support.</returns>
        /// <exception cref="ArgumentException">Thrown if both depth and stencil are false, as at least one must be requested.</exception>
        public static GFXRenderbufferStorage GetBestDepthStencilFormat(bool depth, bool stencil)
        {
            return (depth, stencil) switch
            {
                (true, true) => GFXRenderbufferStorage.Depth24Stencil8,
                (true, false) => GFXRenderbufferStorage.DepthComponent24,
                (false, true) => GFXRenderbufferStorage.StencilIndex8,
                _ => throw new ArgumentException("At least one of depth or stencil must be true.")
            };
        }


        /// <summary>
        /// Recursively searches the scene graph starting from the specified node for a node with the given name.
        /// </summary>
        /// <remarks>The search is performed in a depth-first manner. Only the first matching node
        /// encountered will be returned in foundNode.</remarks>
        /// <param name="currentNode">The node from which to begin the search. This node and its descendants will be examined.</param>
        /// <param name="name">The name of the node to search for. The comparison is case-sensitive.</param>
        /// <param name="foundNode">When this method returns, contains the first node with the specified name if found; otherwise, a default
        /// value.</param>
        /// <returns>true if a node with the specified name is found; otherwise, false.</returns>
        public static bool FindNodeByNameRecursive(SceneNodeData currentNode, string name, out SceneNodeData foundNode)
        {
            if (currentNode.name == name)
            {
                foundNode = currentNode;
                return true;
            }
            foreach (var child in currentNode.children)
            {
                if (FindNodeByNameRecursive(child, name, out foundNode))
                {
                    return true;
                }
            }
            foundNode = new SceneNodeData();
            return false;
        }

        public static void SerializeVec2(Vector2 vec, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("X");
            writer.WriteValue(vec.X);
            writer.WritePropertyName("Y");
            writer.WriteValue(vec.Y);
            writer.WriteEndObject();
        }

        public static void SerializeVec2i(Vector2i vec, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("X");
            writer.WriteValue(vec.X);
            writer.WritePropertyName("Y");
            writer.WriteValue(vec.Y);
            writer.WriteEndObject();
        }

        public static void SerializeVec3(Vector3 vec, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("X");
            writer.WriteValue(vec.X);
            writer.WritePropertyName("Y");
            writer.WriteValue(vec.Y);
            writer.WritePropertyName("Z");
            writer.WriteValue(vec.Z);
            writer.WriteEndObject();
        }

        public static void SerializeVec3i(Vector3i vec, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("X");
            writer.WriteValue(vec.X);
            writer.WritePropertyName("Y");
            writer.WriteValue(vec.Y);
            writer.WritePropertyName("Z");
            writer.WriteValue(vec.Z);
            writer.WriteEndObject();
        }

        public static void SerializeVec4(Vector4 vec, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("X");
            writer.WriteValue(vec.X);
            writer.WritePropertyName("Y");
            writer.WriteValue(vec.Y);
            writer.WritePropertyName("Z");
            writer.WriteValue(vec.Z);
            writer.WritePropertyName("W");
            writer.WriteValue(vec.W);
            writer.WriteEndObject();
        }

        public static void SerializeVec4i(Vector4i vec, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("X");
            writer.WriteValue(vec.X);
            writer.WritePropertyName("Y");
            writer.WriteValue(vec.Y);
            writer.WritePropertyName("Z");
            writer.WriteValue(vec.Z);
            writer.WritePropertyName("W");
            writer.WriteValue(vec.W);
            writer.WriteEndObject();
        }

        public static void SerializeQuat(Quaternion quat, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("X");
            writer.WriteValue(quat.X);
            writer.WritePropertyName("Y");
            writer.WriteValue(quat.Y);
            writer.WritePropertyName("Z");
            writer.WriteValue(quat.Z);
            writer.WritePropertyName("W");
            writer.WriteValue(quat.W);
            writer.WriteEndObject();
        }

        public static Vector4 DeserializeVec4(JObject obj)
        {
            return new Vector4(
                obj["X"].Value<float>(),
                obj["Y"].Value<float>(),
                obj["Z"].Value<float>(),
                obj["W"].Value<float>()
            );
        }

        public static Vector4i DeserializeVec4i(JObject obj)
        {
            return new Vector4i(
                obj["X"].Value<int>(),
                obj["Y"].Value<int>(),
                obj["Z"].Value<int>(),
                obj["W"].Value<int>()
            );
        }

        public static Quaternion DeserializeQuat(JObject obj)
        {
            return new Quaternion(
                obj["X"].Value<float>(),
                obj["Y"].Value<float>(),
                obj["Z"].Value<float>(),
                obj["W"].Value<float>()
            );
        }

        public static Vector3 DeserializeVec3(JObject obj)
        {
            return new Vector3(
                obj["X"].Value<float>(),
                obj["Y"].Value<float>(),
                obj["Z"].Value<float>()
            );
        }

        public static Vector3i DeserializeVec3i(JObject obj)
        {
            return new Vector3i(
                obj["X"].Value<int>(),
                obj["Y"].Value<int>(),
                obj["Z"].Value<int>()
            );
        }

        public static Vector2 DeserializeVec2(JObject obj)
        {
            return new Vector2(
                obj["X"].Value<float>(),
                obj["Y"].Value<float>()
            );
        }

        public static Vector2i DeserializeVec2i(JObject obj)
        {
            return new Vector2i(
                obj["X"].Value<int>(),
                obj["Y"].Value<int>()
            );
        }

        public static void SerializeVertex(Vertex vertex, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Position");
            SerializeVec3(vertex.Position, writer);
            writer.WritePropertyName("Normal");
            SerializeVec3(vertex.Normal, writer);
            writer.WritePropertyName("TexCoord");
            SerializeVec2(vertex.TexCoord, writer);
            writer.WritePropertyName("Tangent");
            SerializeVec4(vertex.Tangent, writer);
            writer.WritePropertyName("BoneWeights");
            SerializeVec4(vertex.BoneWeights, writer);
            writer.WritePropertyName("BoneIDs");
            SerializeVec4i(vertex.BoneIDs, writer);
            writer.WriteEndObject();
        }

        public static Vertex DeserializeVertex(JObject obj)
        {
            Vertex vertex = new Vertex();
            vertex.Position = Utils.DeserializeVec3((JObject)obj["Position"]);
            vertex.Normal = Utils.DeserializeVec3((JObject)obj["Normal"]);
            vertex.TexCoord = Utils.DeserializeVec2((JObject)obj["TexCoord"]);
            vertex.Tangent = Utils.DeserializeVec4((JObject)obj["Tangent"]);
            vertex.BoneWeights = Utils.DeserializeVec4((JObject)obj["BoneWeights"]);
            vertex.BoneIDs = Utils.DeserializeVec4i((JObject)obj["BoneIDs"]);
            return vertex;
        }

        public static void SerializeMatrix4(Matrix4 mat, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Row0");
            SerializeVec4(mat.Row0, writer);
            writer.WritePropertyName("Row1");
            SerializeVec4(mat.Row1, writer);
            writer.WritePropertyName("Row2");
            SerializeVec4(mat.Row2, writer);
            writer.WritePropertyName("Row3");
            SerializeVec4(mat.Row3, writer);
            writer.WriteEndObject();
        }

        public static Matrix4 DeserializeMatrix4(JObject obj)
        {
            Matrix4 mat = new Matrix4();
            mat.Row0 = Utils.DeserializeVec4((JObject)obj["Row0"]);
            mat.Row1 = Utils.DeserializeVec4((JObject)obj["Row1"]);
            mat.Row2 = Utils.DeserializeVec4((JObject)obj["Row2"]);
            mat.Row3 = Utils.DeserializeVec4((JObject)obj["Row3"]);
            return mat;
        }

        public static void SerializeSceneNodeData(SceneNodeData nodedata, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Transformation");
            SerializeMatrix4(nodedata.transformation, writer);
            writer.WritePropertyName("Name");
            writer.WriteValue(nodedata.name);
            writer.WritePropertyName("ChildrenCount");
            writer.WriteValue(nodedata.childrenCount);
            writer.WritePropertyName("Children");
            writer.WriteStartArray();
            foreach (var child in nodedata.children)
            {
                SerializeSceneNodeData(child, writer);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public static SceneNodeData DeserializeSceneNodeData(JObject obj)
        {
            SceneNodeData nodedata = new SceneNodeData();
            nodedata.transformation = Utils.DeserializeMatrix4((JObject)obj["Transformation"]);
            nodedata.name = obj["Name"].Value<string>();
            nodedata.childrenCount = obj["ChildrenCount"].Value<int>();
            nodedata.children = new List<SceneNodeData>();
            JArray childrenArray = (JArray)obj["Children"];
            foreach (var childObj in childrenArray)
            {
                nodedata.children.Add(DeserializeSceneNodeData((JObject)childObj));
            }
            return nodedata;
        }

        public static void SerializeBoneInfo(BoneInfo boneInfo, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("ID");
            writer.WriteValue(boneInfo.id);
            writer.WritePropertyName("Offset");
            SerializeMatrix4(boneInfo.offset, writer);
            writer.WriteEndObject();
        }

        public static BoneInfo DeserializeBoneInfo(JObject obj)
        {
            BoneInfo boneInfo = new BoneInfo();
            boneInfo.id = obj["ID"].Value<int>();
            boneInfo.offset = Utils.DeserializeMatrix4(obj["Offset"] as JObject);
            return boneInfo;
        }

        public static void SerializeKeyPosition(KeyPosition keyPosition, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Position");
            SerializeVec3(keyPosition.position, writer);
            writer.WritePropertyName("TimeStamp");
            writer.WriteValue(keyPosition.timeStamp);
            writer.WriteEndObject();
        }

        public static KeyPosition DeserializeKeyPosition(JObject obj)
        {
            KeyPosition keyPosition = new KeyPosition();
            keyPosition.position = Utils.DeserializeVec3(obj["Position"] as JObject);
            keyPosition.timeStamp = obj["TimeStamp"].Value<float>();
            return keyPosition;
        }

        public static void SerializeKeyRotation(KeyRotation keyRotation, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Orientation");
            SerializeQuat(keyRotation.orientation, writer);
            writer.WritePropertyName("TimeStamp");
            writer.WriteValue(keyRotation.timeStamp);
            writer.WriteEndObject();
        }

        public static KeyRotation DeserializeKeyRotation(JObject obj)
        {
            KeyRotation keyRotation = new KeyRotation();
            keyRotation.orientation = Utils.DeserializeQuat(obj["Orientation"] as JObject);
            keyRotation.timeStamp = obj["TimeStamp"].Value<float>();
            return keyRotation;
        }

        public static void SerializeKeyScale(KeyScale keyScale, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Scale");
            SerializeVec3(keyScale.scale, writer);
            writer.WritePropertyName("TimeStamp");
            writer.WriteValue(keyScale.timeStamp);
            writer.WriteEndObject();
        }

        public static KeyScale DeserializeKeyScale(JObject obj)
        {
            KeyScale keyScale = new KeyScale();
            keyScale.scale = Utils.DeserializeVec3(obj["Scale"] as JObject);
            keyScale.timeStamp = obj["TimeStamp"].Value<float>();
            return keyScale;
        }

        public static void SerializeAABB(AABB aabb, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Min");
            SerializeVec3(aabb.Min, writer);
            writer.WritePropertyName("Max");
            SerializeVec3(aabb.Max, writer);
            writer.WriteEndObject();
        }

        public static AABB DeserializeAABB(JObject obj)
        {
            AABB aabb = new AABB();
            aabb.Min = Utils.DeserializeVec3(obj["Min"] as JObject);
            aabb.Max = Utils.DeserializeVec3(obj["Max"] as JObject);
            return aabb;
        }
    }
}
