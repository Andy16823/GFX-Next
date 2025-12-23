using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class OverlayFXShader : ShaderProgram
    {
        public OverlayFXShader()
        {
            this.VertexShader = new Shader(@"
                #version 410 core
                layout(location = 0) in vec3 inPosition;

                out vec2 uvPosition;
                
                void main() {
                    gl_Position = vec4(inPosition, 1.0);
                    uvPosition = inPosition.xy * 0.5 + 0.5;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 410 core

                in vec2 uvPosition;

                uniform sampler2D sourceTexture;
                uniform vec4 overlayColor;
                
                
                out vec4 fragColor;

                void main() {
                                        
                    vec4 srcColor = texture(sourceTexture, uvPosition);
                    fragColor = mix(srcColor, overlayColor, overlayColor.a);
                }
            ");
        }
    }
}
