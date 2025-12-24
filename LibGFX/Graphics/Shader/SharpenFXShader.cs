using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class SharpenFXShader : ShaderProgram
    {
        public SharpenFXShader()
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

            // Fragment Shader (Sharpen Effect)
            FragmentShader = new Graphics.Shader.Shader(@"
                #version 330 core
                
                in vec2 TexCoords;
                out vec4 FragColor;
                
                uniform sampler2D sourceTexture;
                uniform vec2 texelSize;      // 1.0 / resolution
                uniform float intensity;     // Sharpen strength (0-1+)
                
                void main()
                {
                    // Sharpen kernel (3x3):
                    //  0  -1   0
                    // -1   5  -1
                    //  0  -1   0
                    //
                    // Or stronger version:
                    // -1  -1  -1
                    // -1   9  -1
                    // -1  -1  -1
                    
                    vec3 center = texture(sourceTexture, TexCoords).rgb;
                    
                    // Sample 4 neighbors (cross pattern)
                    vec3 top    = texture(sourceTexture, TexCoords + vec2(0.0, texelSize.y)).rgb;
                    vec3 bottom = texture(sourceTexture, TexCoords - vec2(0.0, texelSize.y)).rgb;
                    vec3 left   = texture(sourceTexture, TexCoords - vec2(texelSize. x, 0.0)).rgb;
                    vec3 right  = texture(sourceTexture, TexCoords + vec2(texelSize.x, 0.0)).rgb;
                    
                    // Apply sharpen kernel
                    vec3 sharpened = center * 5.0 - (top + bottom + left + right);
                    
                    // Mix with original based on intensity
                    vec3 result = mix(center, sharpened, intensity);
                    
                    FragColor = vec4(result, 1.0);
                }
            ");
        }
    }
}
