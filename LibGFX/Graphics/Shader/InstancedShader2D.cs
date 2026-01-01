using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class InstancedShader2D : RenderShader
    {
        public InstancedShader2D()
        {
            this.VertexShader = new Shader(@"
                #version 430 core

                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec2 inTexCoord;
                layout(location = 2) in vec3 inNormal;
                layout(location = 3) in vec4 inTangent;

                layout(binding = 0, std430, row_major) buffer matrixBuffer {
                    mat4 modelMatrices[];
                };

                layout(std430, binding = 1) buffer extrasBuffer {
                    vec4 extraBuffer[];
                };

                layout(std430, binding = 2) buffer uvTransformBuffer {
                    vec4 uvTransforms[];
                };

                out vec3 position;
                out vec3 normal;
                out vec2 texCoord;
                out vec4 tangent; 
                out vec4 extras;
                out vec4 uvTransform;

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 mesh_matrix;

                void main() {
                    mat4 m_mat = mesh_matrix * modelMatrices[gl_InstanceID]; 
                    mat4 mvp = m_mat * v_mat * p_mat;
                    position = vec3(vec4(inPosition, 1.0) * m_mat);
                    normal = inNormal * transpose(inverse(mat3(m_mat)));
                    texCoord = inTexCoord;
                    tangent = inTangent;
                    extras = extraBuffer[gl_InstanceID];
                    uvTransform = uvTransforms[gl_InstanceID];
                    gl_Position = vec4(inPosition, 1.0) * mvp;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 430 core

                in vec3 position;
                in vec3 normal;
                in vec2 texCoord;
                in vec4 tangent;
                in vec4 extras;
                in vec4 uvTransform;

                out vec4 fragColor;
                uniform vec3 viewPos;
                uniform sampler2D textureSampler;

                void main() {
                    vec2 transformedTexCoord = texCoord * uvTransform.xy + uvTransform.zw;
                    fragColor = texture(textureSampler, transformedTexCoord);
                    //fragColor = vec4(1.0, 1.0, 1.0, 1.0);
                }
            ");
        }
    }
}