using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class SepiaFXShader : RenderShader
    {
        public SepiaFXShader()
        {
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

            FragmentShader = new Graphics.Shader.Shader(@"
                #version 330 core
                
                in vec2 TexCoords;
                out vec4 FragColor;
                
                uniform sampler2D sourceTexture;
                uniform float intensity;  // 0 = color, 1 = full sepia
                
                void main()
                {
                    vec4 color = texture(sourceTexture, TexCoords);
                    
                    // Sepia transformation matrix
                    // Diese Werte sind die Standard-Sepia-Konvertierung
                    vec3 sepia;
                    sepia.r = dot(color.rgb, vec3(0.393, 0.769, 0.189));
                    sepia.g = dot(color.rgb, vec3(0.349, 0.686, 0.168));
                    sepia.b = dot(color.rgb, vec3(0.272, 0.534, 0.131));
                    
                    // Mix zwischen original und sepia
                    color.rgb = mix(color.rgb, sepia, intensity);
                    
                    FragColor = color;
                }
            ");
        }
    }
}
