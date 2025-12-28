using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class InfiniteGridShader : ShaderProgram
    {
        public InfiniteGridShader()
        {
            this.VertexShader = new Shader(@"
                #version 330 core
                layout(location = 0) in vec3 aPos;

                uniform mat4 p_mat; // projection
                uniform mat4 v_mat; // view
                uniform mat4 m_mat; // model (world)

                out vec3 vWorldPos;

                void main()
                {
                    // model * vec -> world position
                    vec4 worldPos = vec4(aPos, 1.0) * m_mat;
                    vWorldPos = worldPos.xyz;

                    // projection * view * worldPos
                    gl_Position = worldPos * v_mat * p_mat;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 330 core
                in vec3 vWorldPos;
                out vec4 FragColor;

                uniform vec3 u_CameraPos;
                uniform float u_GridSize;    // z.B. 1.0
                uniform float u_MainStep;    // z.B. 5.0
                uniform vec4 u_GridColor;    // base/minor color
                uniform vec4 u_AxisColorX;   // e.g. vec4(1,0.3,0.3,1)
                uniform vec4 u_AxisColorZ;   // e.g. vec4(0.3,0.3,1,1)
                uniform float u_FadeStart;   // z.B. 40.0
                uniform float u_FadeEnd;     // z.B. 200.0
                uniform float u_LineWidth;   // world-space approx line thickness (try 0.02..0.06)

                // New: how far from zero the special axis coloring should extend (world units)
                uniform float u_AxisFadeDistance; // e.g. 1.0

                float mask_for_line(float coord, float gridSize, float targetThickness)
                {
                    // coord in world units. scaled in cell units
                    float scaled = coord / gridSize;

                    // distance in cell-space (0..0.5)
                    float dist = abs(fract(scaled) - 0.5);

                    // derivative in cell-space -> use fwidth(scaled)
                    float deriv = fwidth(scaled);
                    deriv = max(deriv, 1e-6);

                    // convert target thickness (world units) -> cell-space half-thickness
                    float halfThickness = (targetThickness / gridSize) * 0.5;

                    // smooth edge using deriv (AA)
                    float edge0 = halfThickness - deriv;
                    float edge1 = halfThickness + deriv;
                    float m = smoothstep(edge1, edge0, dist);
                    return clamp(m, 0.0, 1.0);
                }

                void main()
                {
                    vec2 wxz = vWorldPos.xz;

                    // fade by camera distance (world-space)
                    float distToCam = distance(vec3(vWorldPos.x, 0.0, vWorldPos.z), vec3(u_CameraPos.x, 0.0, u_CameraPos.z));
                    float fade = clamp((u_FadeEnd - distToCam) / max(0.0001, (u_FadeEnd - u_FadeStart)), 0.0, 1.0);

                    // minor masks
                    float minorMaskX = mask_for_line(wxz.x, u_GridSize, u_LineWidth);
                    float minorMaskZ = mask_for_line(wxz.y, u_GridSize, u_LineWidth);
                    float minorMask = max(minorMaskX, minorMaskZ);

                    // major masks (every N cells)
                    float majorInterval = u_GridSize * max(1.0, u_MainStep);
                    float majorMaskX = mask_for_line(wxz.x, majorInterval, u_LineWidth * 1.2); // slightly thicker
                    float majorMaskZ = mask_for_line(wxz.y, majorInterval, u_LineWidth * 1.2);
                    float majorMask = max(majorMaskX, majorMaskZ);

                    // axis masks (x==0 or z==0) - make a stronger line around zero only
                    // mask_for_line with gridSize=1.0 gives lines at all integers; we additionally fade out everything
                    // except the region near zero using a proper smoothstep (non-inverted edges).
                    float axisBaseX = mask_for_line(wxz.x, 1.0, u_LineWidth * 1.6);
                    float axisBaseZ = mask_for_line(wxz.y, 1.0, u_LineWidth * 1.6);

                    // create a fade factor that is 1.0 at 0.0 and goes to 0.0 at u_AxisFadeDistance
                    float axisFadeX = 1.0 - smoothstep(0.0, u_AxisFadeDistance, abs(wxz.x));
                    float axisFadeZ = 1.0 - smoothstep(0.0, u_AxisFadeDistance, abs(wxz.y));

                    float axisMaskX = axisBaseX * axisFadeX;
                    float axisMaskZ = axisBaseZ * axisFadeZ;
                    float axisMask = max(axisMaskX, axisMaskZ);

                    // combine: axis > major > minor
                    vec4 baseColor = u_GridColor;
                    vec4 majorColor = min(u_GridColor * 1.25, vec4(1.0));
                    vec4 c = baseColor;
                    c = mix(c, majorColor, majorMask);

                    // axisColor: combine per-axis colors (they only contribute where their respective mask > 0)
                    vec4 axisColor = u_AxisColorX * axisMaskX + u_AxisColorZ * axisMaskZ;
                    c = mix(c, axisColor, axisMask);

                    float intensity = max(max(minorMask, majorMask), axisMask);
                    float alpha = intensity * fade * c.a;

                    // if alpha tiny, early-out but don't discard (lets MSAA sample)
                    if (alpha <= 1e-4)
                    {
                        FragColor = vec4(0.0, 0.0, 0.0, 0.0);
                        return;
                    }

                    vec3 rgb = c.rgb;
                    FragColor = vec4(rgb, alpha);
                }
            ");
        }
    }
}