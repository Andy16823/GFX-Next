using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class PixelationFXShader : ShaderProgram
    {
        public PixelationFXShader()
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

            // Fragment Shader (Pixelation Effect)
            FragmentShader = new Graphics.Shader.Shader(@"
                #version 330 core
                
                in vec2 TexCoords;
                out vec4 FragColor;
                
                uniform sampler2D sourceTexture;
                uniform vec2 resolution;     // Screen resolution
                uniform float pixelSize;     // Pixel block size (e.g.  4 = 4x4 blocks)
                
                void main()
                {
                    // Calculate pixel block size in UV space
                    vec2 pixelBlockSize = vec2(pixelSize) / resolution;
                    
                    // Snap UV coordinates to pixel grid
                    vec2 pixelatedUV = floor(TexCoords / pixelBlockSize) * pixelBlockSize;
                    
                    // Sample texture at pixelated position
                    vec3 color = texture(sourceTexture, pixelatedUV).rgb;
                    
                    FragColor = vec4(color, 1.0);
                }
            ");
        }
    }
}
