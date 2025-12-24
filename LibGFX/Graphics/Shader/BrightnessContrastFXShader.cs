using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class BrightnessContrastFXShader : ShaderProgram
    {
        public BrightnessContrastFXShader()
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

            // Fragment Shader (Brightness/Contrast Effect)
            FragmentShader = new Graphics.Shader.Shader(@"
                #version 330 core
                
                in vec2 TexCoords;
                out vec4 FragColor;
                
                uniform sampler2D sourceTexture;
                uniform float brightness;    // Additive brightness (-1 to 1, 0 = no change)
                uniform float contrast;      // Multiplicative contrast (0 to 2+, 1 = no change)
                
                void main()
                {
                    vec3 color = texture(sourceTexture, TexCoords).rgb;
                    
                    // Apply contrast first (around middle grey 0.5)
                    // Formula: (color - 0.5) * contrast + 0.5
                    color = (color - 0.5) * contrast + 0.5;
                    
                    // Then apply brightness (additive)
                    color += brightness;
                    
                    // Clamp to valid range
                    color = clamp(color, 0.0, 1.0);
                    
                    FragColor = vec4(color, 1.0);
                }
            ");
        }
    }
}
