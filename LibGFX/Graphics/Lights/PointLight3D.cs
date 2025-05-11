using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    /// <summary>
    /// Represents the data structure for a point light for the shader.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PointLight3DData
    {
        public Vector4 Position;
        public Vector4 ConstantLinearQuadratic;
        public Vector4 Ambient;
        public Vector4 Diffuse;
        public Vector4 Specular;
    }

    /// <summary>
    /// Represents a point light in the scene.
    /// </summary>
    public class PointLight3D : Light
    {
        /// <summary>
        /// The range of the light.
        /// </summary>
        public float Range { get => _range; set => SetRange(value); }

        /// <summary>
        /// The constant of the light. It get calculated based on the range and intensity.
        /// </summary>
        public float Constant { get; internal set; }

        /// <summary>
        /// The linear attenuation of the light. It get calculated based on the range and intensity.
        /// </summary>
        public float Linear { get; internal set; }

        /// <summary>
        /// The quadratic attenuation of the light. It get calculated based on the range and intensity.
        /// </summary>
        public float Quadratic { get; internal set; }

        /// <summary>
        /// The ambient color of the light.
        /// </summary>
        public Vector4 Ambient { get; set; }

        /// <summary>
        /// The specular color of the light.
        /// </summary>
        public Vector4 Specular { get; set; }

        /// <summary>
        /// The intensity of the light.
        /// </summary>
        public override float Intensity { get => _intensity; set => SetIntensity(value); }

        // light intensity
        private float _intensity;

        // light range
        private float _range;

        /// <summary>
        /// Creates a new instance of the <see cref="PointLight3D"/> class.
        /// </summary>
        /// <param name="postion"></param>
        /// <param name="color"></param>
        public PointLight3D(Vector3 position, Vector4 color, float range = 10f, float intesity = 1.0f)
        {
            Position = position;
            Color = color;

            Ambient = new Vector4(0.05f);
            Specular = new Vector4(1.0f);


            this.SetRange(range);
            this.SetIntensity(intesity);
        }

        /// <summary>
        /// Sets the intensity of the light and calculates the attenuation coefficients.
        /// </summary>
        /// <param name="intensity"></param>
        private void SetIntensity(float intensity)
        {
            _intensity = intensity;
            CalculateLightIntensitiy();
        }

        /// <summary>
        /// Sets the range of the light and calculates the attenuation coefficients.
        /// </summary>
        /// <param name="range"></param>
        private void SetRange(float range)
        {
            _range = range;
            CalculateLightIntensitiy();
        }

        /// <summary>
        /// Sets the attenuation coefficients for the light based on a given range.
        /// </summary>
        /// <param name="range"></param>
        public void CalculateLightIntensitiy()
        {
            Constant = 1.0f;
            Linear = 4.5f / Intensity;
            Quadratic = 75.0f / (Range * Range);
        }

        /// <summary>
        /// Converts the light data to a structure for use in the shader.
        /// </summary>
        /// <returns></returns>
        public PointLight3DData ToStruct()
        {
            return new PointLight3DData()
            {
                Position = new Vector4(Position, 1.0f),
                ConstantLinearQuadratic = new Vector4(Constant, Linear, Quadratic, 0.0f),
                Ambient = Ambient,
                Diffuse = Color * Intensity,
                Specular = Specular
            };
        }

        /// <summary>
        /// Calculates the axis-aligned bounding box (AABB) of the light based on its range.
        /// </summary>
        /// <param name="minThreshold"></param>
        /// <returns></returns>
        public (Vector3 min, Vector3 max) GetAABB(float minThreshold = 0.01f)
        {
            float range = this.Range;
            Vector3 center = Position;

            Vector3 min = center - new Vector3(range);
            Vector3 max = center + new Vector3(range);

            return (min, max);
        }
    }
}

