using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class ScanlinesFXShader : ShaderProgram
    {
        public ScanlinesFXShader()
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

            // Fragment Shader (Scanlines Effect)
            FragmentShader = new Graphics.Shader.Shader(@"
                #version 330 core
                
                in vec2 TexCoords;
                out vec4 FragColor;
                
                uniform sampler2D sourceTexture;
                uniform float intensity;      // Scanline darkness (0-1)
                uniform float lineCount;      // Number of lines (e.g.  800 for 1080p)
                uniform float speed;          // Animation speed (0 = static)
                uniform float time;           // Time for animation
                
                void main()
                {
                    vec3 color = texture(sourceTexture, TexCoords).rgb;
                    
                    // Calculate scanline position
                    float scanlinePos = TexCoords.y * lineCount;
                    
                    // Add time for scrolling effect
                    scanlinePos += time * speed;
                    
                    // Generate scanline pattern
                    float scanline = sin(scanlinePos * 3.14159265) * 0.5 + 0.5;
                    
                    // Apply scanline darkness
                    float darken = 1.0 - (scanline * intensity);
                    
                    color *= darken;
                    
                    FragColor = vec4(color, 1.0);
                }
            ");
        }
    }
}
