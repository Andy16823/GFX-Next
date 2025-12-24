using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Materials
{
    public class SGMaterialImporter : IMaterialImporter
    {
        public IMaterial ImportAssimpMaterial(Assimp.Material asmat, String directory)
        {
            var material = new Graphics.Materials.SGMaterial();
            material.Name = asmat.Name;
            material.Opacity = asmat.Opacity;
            material.Color = new Vector4(asmat.ColorDiffuse.X, asmat.ColorDiffuse.Y, asmat.ColorDiffuse.Z, asmat.ColorDiffuse.W);

            if (asmat.Shininess > 0)
            {
                material.Shininess = asmat.Shininess;
            }

            if (asmat.HasTextureDiffuse)
            {
                material.DiffuseTexture = new Texture(Path.Combine(directory, asmat.TextureDiffuse.FilePath));
            }
            else
            {
                material.DiffuseTexture = new Texture(1, 1, new Vector4i(255, 255, 255, 255));
            }

            if (asmat.HasTextureNormal)
            {
                material.NormalTexture = new Texture(Path.Combine(directory, asmat.TextureNormal.FilePath));
            }
            else
            {
                material.NormalTexture = new Texture(1, 1, new Vector4i(128, 128, 255, 255));
            }

            if (asmat.HasTextureSpecular)
            {
                material.SpecularTexture = new Texture(Path.Combine(directory, asmat.TextureSpecular.FilePath));
            }
            else
            {
                material.SpecularTexture = new Texture(1, 1, new Vector4i(128, 128, 128, 255));
            }

            return material;
        }
    }
}
