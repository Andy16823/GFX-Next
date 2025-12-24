using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class ColorTintFXShader : ShaderProgram
    {
        public ColorTintFXShader()
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

            // Fragment Shader (Color Tint)
            FragmentShader = new Graphics.Shader.Shader(@"
                #version 330 core
                
                in vec2 TexCoords;
                out vec4 FragColor;
                
                uniform sampler2D sourceTexture;
                uniform vec4 tintColor;  // RGB = Farbe, A = Stärke
                
                void main()
                {
                    vec4 color = texture(sourceTexture, TexCoords);
                    
                    // Mix original color with tint color
                    color.rgb = mix(color.rgb, tintColor.rgb, tintColor.a);
                    
                    FragColor = color;
                }
            ");
        }
    }
}
