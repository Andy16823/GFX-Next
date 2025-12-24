using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class VignetteFXShader : ShaderProgram
    {
        public VignetteFXShader()
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
                uniform float intensity;      // Stärke der Abdunkelung (0-1)
                uniform float smoothness;     // Weichheit des Übergangs (0-1)
                uniform vec3 vignetteColor;   // Farbe der Vignette
                
                void main()
                {
                    // Original Farbe holen
                    vec4 color = texture(sourceTexture, TexCoords);
                    
                    // UV von 0-1 zu -1 bis 1 (zentriert)
                    vec2 uv = TexCoords * 2.0 - 1.0;
                    
                    // Distanz vom Zentrum berechnen
                    float dist = length(uv);
                    
                    // Vignette Maske berechnen
                    // Je höher smoothness, desto weicher der Übergang
                    // Je höher intensity, desto stärker die Abdunkelung
                    float vignette = smoothstep(1.2, 1.2 - smoothness * 1.5, dist);
                    
                    // Vignette Stärke anwenden
                    // pow macht den Effekt noch stärker
                    vignette = pow(vignette, 0.8 - (intensity * 0.5));
                    
                    // Farbe mit Vignette mischen
                    color.rgb = mix(vignetteColor, color.rgb, vignette);
                    
                    FragColor = color;
                }
            ");
        }
    }
}