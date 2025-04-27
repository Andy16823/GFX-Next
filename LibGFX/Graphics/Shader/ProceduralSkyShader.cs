using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class ProceduralSkyShader : ShaderProgram
    {
        public ProceduralSkyShader()
        {
            this.VertexShader = new Shader(@"
                #version 410 core
                layout (location = 0) in vec3 inPosition;

                out vec3 position;

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;

                void main()
                {
                    position = inPosition;
                    gl_Position = vec4(inPosition, 1.0) * m_mat * v_mat * p_mat;
                }  
            ");

            this.FragmentShader = new Shader(@"
                #version 410 core
                out vec4 fragColor;

                in vec3 position;

                uniform vec3 skyTopColor;
                uniform vec3 skyBottomColor;
                uniform vec3 sunDirection;
                uniform vec3 sunColor;
                uniform float sunSize;
                uniform float sunIntensity;
                uniform float skylineOffset;
                uniform float skylineScale;

                // Optional coverage
                uniform bool coverage;
                uniform sampler2D coverageTexture;
                uniform float coverageFactor;

                void main() {
                    // Normalisiere die Richtung
                    vec3 dir = normalize(position);

                    // Himmel-Hintergrund: Interpolation zwischen Boden- und Himmelsfarbe
                    float t = dir.y*0.5+0.5;
                    t = smoothstep(skylineOffset - skylineScale, skylineOffset + skylineScale, dir.y);

                    vec3 skyColor = mix(skyBottomColor, skyTopColor, t);

                    // Optional: Coverage-Textur
                    if(coverage) {
                        // Interpolation der Wolkenbedeckung basierend auf der Höhe
                        float cloudFactor = smoothstep(0.0, 1.0, dir.y); // Interpolation von 0.0 (unten) bis 1.0 (oben)

                        // Wolkenanzeige: Lese die Noise-Textur und kombiniere mit cloudFactor
                        vec2 noiseCords = vec2(dir.x, dir.z); // Texturkoordinaten für Wolken
                        float cloudNoise = texture(coverageTexture, noiseCords).r; // Noise-Wert aus der Textur
                        float clouds = smoothstep(0.3, 0.7, cloudNoise); // Weicher Übergang für Wolken
                        clouds *= coverageFactor; // Intensität der Wolkenbedeckung

                        // Cloud-Faktor beeinflusst die Sichtbarkeit der Wolken
                        clouds *= cloudFactor;

                        clouds *= smoothstep(skylineOffset - skylineScale, skylineOffset + skylineScale, dir.y);

                        // Wolkenfarbe hinzufügen
                        vec3 cloudColor = mix(skyColor, vec3(0.8, 0.8, 0.8), clouds);
                        skyColor = cloudColor; // Wolken auf den Himmel anwenden
                    }

                    // Sonne hinzufügen
                    float sunFactor = max(dot(dir, normalize(sunDirection)), 0.0);
                    sunFactor = pow(sunFactor, sunSize);

                    vec3 finalColor = skyColor+sunColor*sunFactor*sunIntensity;

                    fragColor = vec4(finalColor, 1.0);
                }
            ");
        }
    }
}
