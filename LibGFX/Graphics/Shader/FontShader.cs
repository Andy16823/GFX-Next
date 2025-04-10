﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class FontShader : ShaderProgram
    {
        public FontShader()
        {
            this.VertexShader = new Shader(@"
                #version 330 core
                layout (location = 0) in vec4 vertex;
                layout (location = 1) in int glyphLayer;

                out vec2 TexCoords;
                flat out int layer;

                uniform mat4 p_mat;

                void main()
                {
                    gl_Position = p_mat * vec4(vertex.xy, 0.0, 1.0);
                    TexCoords = vertex.zw;
                    layer = glyphLayer;
                }  
            ");

            this.FragmentShader = new Shader(@"
                #version 330 core
                in vec2 TexCoords;
                flat in int layer; 

                out vec4 color;

                uniform sampler2DArray fontTexture;
                uniform vec4 vertexColor;

                void main()
                {    
                    float alpha = texture(fontTexture, vec3(TexCoords, float(layer))).r;
                    color = vertexColor * vec4(1.0, 1.0, 1.0, alpha);
                }
            ");
        }
    }
}
