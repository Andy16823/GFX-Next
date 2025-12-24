using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class FilmGrainFXShader : ShaderProgram
    {
        public FilmGrainFXShader()
        {
            // Vertex Shader (Standard Fullscreen Quad)
            VertexShader = new Graphics.Shader.Shader(@"
                #version 330 core
                layout (location = 0) in vec3 aPos;
                layout (location = 1) in vec2 aTexCoords;
                
                out vec2 TexCoords;
                
                void main()
                {
                    TexCoords = aTexCoords;
                    gl_Position = vec4(aPos, 1.0);
                }
            ");

            // Fragment Shader (Film Grain Effect - from Shadertoy)
            FragmentShader = new Graphics.Shader.Shader(@"
                #version 330 core
                
                in vec2 TexCoords;
                out vec4 FragColor;
                
                uniform sampler2D sourceTexture;
                uniform float intensity;      // Grain intensity (0-1)
                uniform float time;           // For animated grain
                uniform vec2 resolution;      // Screen resolution
                uniform vec2 filmResolution;  // Film grain resolution (default: 1280x720)
                
                // Film grain function from Shadertoy
                vec3 filmPixel(vec2 uv, float iTime)
                {
                    mat2x3 uvs = mat2x3(uv.xxx, uv.yyy) + mat2x3(vec3(0, 0.1, 0.2), vec3(0, 0.3, 0.4));
                    return fract(sin(uvs * vec2(12.9898, 78.233) * iTime) * 43758.5453);
                }
                
                void main()
                {
                    vec2 uv = TexCoords;
                    
                    // Simulate fixed resolution (ex: 720p)
                    vec2 filmRes = filmResolution;
                    vec2 coord = floor(uv * filmRes);
                    vec2 rest = fract(uv * filmRes);
                    
                    // Calculate noise at 4 corners for bilinear interpolation
                    vec3 noise00 = filmPixel(coord / filmRes, time);
                    vec3 noise01 = filmPixel((coord + vec2(0, 1)) / filmRes, time);
                    vec3 noise10 = filmPixel((coord + vec2(1, 0)) / filmRes, time);
                    vec3 noise11 = filmPixel((coord + vec2(1, 1)) / filmRes, time);
                    
                    // Bilinear interpolation
                    vec3 noise = mix(
                        mix(noise00, noise01, rest.y), 
                        mix(noise10, noise11, rest.y), 
                        rest.x
                    ) * vec3(0.7, 0.6, 0.8);
                    
                    // Get original texture color
                    vec3 tex = texture(sourceTexture, uv).rgb;
                    
                    // Apply grain with intensity
                    vec3 col = tex + noise * intensity;
                    
                    // Output to screen
                    FragColor = vec4(col, 1.0);
                }
            ");
        }
    }
}