using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class GrayscaleFXShader : RenderShader
    {
        public GrayscaleFXShader()
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

            // Fragment Shader (Grayscale Effect)
            FragmentShader = new Graphics.Shader.Shader(@"
                #version 330 core
                
                in vec2 TexCoords;
                out vec4 FragColor;
                
                uniform sampler2D sourceTexture;
                uniform float intensity;  // 0 = full color, 1 = full grayscale
                
                void main()
                {
                    // Original Farbe
                    vec4 color = texture(sourceTexture, TexCoords);
                    
                    // Luminance berechnen (gewichtete RGB-Summe)
                    // Diese Gewichte entsprechen der menschlichen Wahrnehmung
                    float gray = dot(color.rgb, vec3(0.299, 0.587, 0.114));
                    
                    // Grayscale anwenden
                    vec3 grayscale = vec3(gray);
                    
                    // Mix zwischen original und grayscale
                    color.rgb = mix(color.rgb, grayscale, intensity);
                    
                    FragColor = color;
                }
            ");
        }
    }
}
