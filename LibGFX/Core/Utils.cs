using LibGFX.Graphics;
using LibGFX.Math;
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
        /// Creates an empty texture with the given width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap CreateEmptyTexture(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            Color color = Color.FromArgb(255, 255, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
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
                return Texture.LoadTexture(texturePath);
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
    }
}
