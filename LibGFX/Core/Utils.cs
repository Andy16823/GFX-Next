using LibGFX.Assets;
using LibGFX.Audio;
using LibGFX.Compute;
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
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static BulletSharp.Dbvt;
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

        public static float ParseFloat(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0f;

            // Schnelle Sonderfälle
            if (string.Equals(value, "Infinity", StringComparison.OrdinalIgnoreCase))
                return float.PositiveInfinity;

            if (string.Equals(value, "-Infinity", StringComparison.OrdinalIgnoreCase))
                return float.NegativeInfinity;

            if (string.Equals(value, "NaN", StringComparison.OrdinalIgnoreCase))
                return float.NaN;

            // Normale Zahl (immer invariant!)
            if (float.TryParse(
                value,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out float result))
            {
                return result;
            }

            // Optional: letzte Rettung (Debug)
            throw new FormatException($"Invalid float value: '{value}'");
        }

        public static float ReadFloat(JsonReader reader)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Float:
                case JsonToken.Integer:
                    return Convert.ToSingle(reader.Value);

                case JsonToken.String:
                    string s = (string)reader.Value;
                    if (string.Equals(s, "Infinity", StringComparison.OrdinalIgnoreCase))
                        return float.PositiveInfinity;
                    if (string.Equals(s, "-Infinity", StringComparison.OrdinalIgnoreCase))
                        return float.NegativeInfinity;
                    if (string.Equals(s, "NaN", StringComparison.OrdinalIgnoreCase))
                        return float.NaN;
                    break;
            }

            throw new JsonException($"Invalid float value: {reader.Value} ({reader.TokenType})");
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

        public static Vector4 DeserializeVec4(JObject value)
        {
            float x = value["X"].Value<float>();
            float y = value["Y"].Value<float>();
            float z = value["Z"].Value<float>();
            float w = value["W"].Value<float>();

            return new Vector4(x, y, z, w);
        }

        public static Vector4i DeserializeVec4i(JObject value)
        {
            int x = value["X"].Value<int>();
            int y = value["Y"].Value<int>();
            int z = value["Z"].Value<int>();
            int w = value["W"].Value<int>();

            return new Vector4i(x, y, z, w);
        }

        public static Quaternion DeserializeQuat(JObject value)
        {
            float x = value["X"].Value<float>();
            float y = value["Y"].Value<float>();
            float z = value["Z"].Value<float>();
            float w = value["W"].Value<float>();

            return new Quaternion(x, y, z, w);
        }

        public static Vector3 DeserializeVec3(JObject value)
        {
            float x = value["X"].Value<float>();
            float y = value["Y"].Value<float>();
            float z = value["Z"].Value<float>();

            return new Vector3(x, y, z);
        }

        public static Vector3i DeserializeVec3i(JObject value)
        {
            int x = value["X"].Value<int>();
            int y = value["Y"].Value<int>();
            int z = value["Z"].Value<int>();

            return new Vector3i(x, y, z);
        }

        public static Vector2 DeserializeVec2(JObject value)
        {
            float x = value["X"].Value<float>();
            float y = value["Y"].Value<float>();

            return new Vector2(x, y);
        }

        public static Vector2i DeserializeVec2i(JObject value)
        {
            int x = value["X"].Value<int>();
            int y = value["Y"].Value<int>();

            return new Vector2i(x, y);
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

        public static Matrix4 DeserializeMatrix4(JObject reader)
        {
            Matrix4 matrix = new Matrix4();

            matrix.Row0 = DeserializeVec4(reader["Row0"] as JObject);
            matrix.Row1 = DeserializeVec4(reader["Row1"] as JObject);
            matrix.Row2 = DeserializeVec4(reader["Row2"] as JObject);
            matrix.Row3 = DeserializeVec4(reader["Row3"] as JObject);

            return matrix;
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

        public static AABB DeserializeAABB(JObject value)
        {
            AABB aabb = new AABB();
            aabb.Min = DeserializeVec3(value["Min"] as JObject);
            aabb.Max = DeserializeVec3(value["Max"] as JObject);
            return aabb;
        }

        public static GameElement DeserializeGameElement(JObject obj, SerializationContext serializationContext)
        {
            string typeName = obj["Type"]?.Value<string>()
                ?? throw new JsonException("Missing Type");

            Type type = Type.GetType(typeName)
                ?? throw new Exception($"Type '{typeName}' not found.");

            var element = (GameElement)Activator.CreateInstance(type);


            element.Deserialize(obj, serializationContext);

            return element;
        }

        /// <summary>
        /// Serializes the specified vertex to JSON using the provided writer.
        /// </summary>
        /// <param name="writer">The JsonWriter instance used to write the serialized JSON output. Must not be null.</param>
        /// <param name="vertex">The vertex to serialize. Must not be null.</param>
        public static void SerializeVertex(Vertex vertex, JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("TexCoord");
            SerializeVec2(vertex.TexCoord, writer);
            writer.WritePropertyName("Normal");
            SerializeVec3(vertex.Normal, writer);
            writer.WritePropertyName("Tangent");
            SerializeVec4(vertex.Tangent, writer);
            writer.WritePropertyName("BoneIDs");
            SerializeVec4i(vertex.BoneIDs, writer);
            writer.WritePropertyName("BoneWeights");
            SerializeVec4(vertex.BoneWeights, writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Deserializes a vertex from the specified JSON object.
        /// </summary>
        /// <remarks>The JSON object is expected to contain the fields "TexCoord", "Normal", "Tangent",
        /// "BoneIDs", and "BoneWeights" as nested objects. Missing or incorrectly typed fields may result in
        /// exceptions.</remarks>
        /// <param name="value">A <see cref="JObject"/> containing the serialized vertex data. Must not be <see langword="null"/> and must
        /// include the required fields.</param>
        /// <returns>A <see cref="Vertex"/> instance populated with data from the JSON object.</returns>
        public static Vertex DeserializeVertex(JObject value)
        {
            Vertex vertex = new Vertex();
            vertex.TexCoord = DeserializeVec2(value.Value<JObject>("TexCoord")!);
            vertex.Normal = DeserializeVec3(value.Value<JObject>("Normal")!);
            vertex.Tangent = DeserializeVec4(value.Value<JObject>("Tangent")!);
            vertex.BoneIDs = DeserializeVec4i(value.Value<JObject>("BoneIDs")!);
            vertex.BoneWeights = DeserializeVec4(value.Value<JObject>("BoneWeights")!);
            return vertex;
        }
    }
}
