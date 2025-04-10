﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class ScreenShader : ShaderProgram
    {
        public ScreenShader()
        {
            this.VertexShader = new Shader(@"
                #version 330 core
                layout (location = 0) in vec3 aPos;
                layout (location = 1) in vec2 aTexCoords;

                out vec2 TexCoords;

                void main()
                {
                    gl_Position = vec4(aPos, 1.0); 
                    TexCoords = aTexCoords;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 330 core
                out vec4 FragColor;
  
                in vec2 TexCoords;

                uniform sampler2D screenTexture;

                void main()
                { 
                    FragColor = texture(screenTexture, TexCoords);
                }
            ");
        }
    }
}
