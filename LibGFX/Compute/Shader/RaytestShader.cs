using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Compute.Shader
{
    public class RaytestShader : ComputeShader
    {
        public RaytestShader()
        {
            this.ShaderSource = @"
                #version 450
                layout(local_size_x = 1) in;

                uniform vec3 rayOrigin;
                uniform vec3 rayDir;

                uniform mat4 modelMatrix;

                struct HitResult
                {
                    int triangleIndex;
                    vec4 normal;
                    vec4 position;
                };

                struct PackedVec3 {
                    float x;
                    float y;
                    float z;
                };

                layout(std430, binding = 0, row_major) readonly buffer Positions
                {
                    PackedVec3 positions[];
                };

                layout(std430, binding = 1, row_major) readonly buffer IndexBuffer
                {
                    int indices[];
                };

                layout(std430, binding = 2, row_major) writeonly buffer Result
                {
                    HitResult result;
                };

                bool intersectTriangle(vec3 ro, vec3 rd, vec3 v0, vec3 v1, vec3 v2, out float t, out vec3 normal)
                {
                    const float EPS = 1e-5;

                    vec3 edge1 = v1 - v0;
                    vec3 edge2 = v2 - v0;
                    vec3 h = cross(rd, edge2);
                    float a = dot(edge1, h);

                    if (abs(a) < EPS) return false;

                    float f = 1.0 / a;
                    vec3 s = ro - v0;
                    float u = f * dot(s, h);

                    if (u < 0.0 || u > 1.0) return false;

                    vec3 q = cross(s, edge1);
                    float v = f * dot(rd, q);
                    if (v < 0.0 || u + v > 1.0) return false;

                    t = f * dot(edge2, q);
                    if (t > EPS)
                    {
                        normal = normalize(cross(edge1, edge2));
                        return true;
                    }
                    else {
                        return false;
                    }
                }

                void main()
                {
                    float closestT = 1e20;
                    result.triangleIndex = -1;

                    for (int i = 0; i < indices.length(); i += 3)
                    {
                        vec3 v0 = vec3(vec4(positions[indices[i]].x, positions[indices[i]].y, positions[indices[i]].z, 1.0) * modelMatrix);
                        vec3 v1 = vec3(vec4(positions[indices[i + 1]].x, positions[indices[i + 1]].y, positions[indices[i + 1]].z, 1.0) * modelMatrix);
                        vec3 v2 = vec3(vec4(positions[indices[i + 2]].x, positions[indices[i + 2]].y, positions[indices[i + 2]].z, 1.0) * modelMatrix);

                        float t;
                        vec3 normal;
                        if (intersectTriangle(rayOrigin, rayDir, v0, v1, v2, t, normal))
                        {
                            if (t < closestT)
                            {
                                closestT = t;
                                result.triangleIndex = i / 3;
                                result.normal = vec4(normal, t);
                                result.position = vec4(rayOrigin + rayDir * t, 1.0);
                            }
                        }
                    }
                }";
        }
    }
}
