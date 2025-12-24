using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class PosterizationFXShader : ShaderProgram
    {
        public PosterizationFXShader()
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

            // Fragment Shader (Posterization Effect)
            FragmentShader = new Graphics.Shader.Shader(@"
                #version 330 core
                
                in vec2 TexCoords;
                out vec4 FragColor;
                
                uniform sampler2D sourceTexture;
                uniform float levels;        // Number of color levels per channel (e.g.  8)
                uniform float gamma;         // Gamma correction (optional, for better look)
                
                void main()
                {
                    vec3 color = texture(sourceTexture, TexCoords).rgb;
                    
                    // Optional:  Apply gamma correction before posterization
                    // Makes darker colors more visible
                    color = pow(color, vec3(gamma));
                    
                    // Posterize each channel
                    // floor(color * levels) / levels quantizes to N levels
                    color = floor(color * levels) / levels;
                    
                    // Optional: Reverse gamma correction
                    color = pow(color, vec3(1.0 / gamma));
                    
                    FragColor = vec4(color, 1.0);
                }
            ");
        }
    }
}
