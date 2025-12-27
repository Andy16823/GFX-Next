using LibGFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGFXEditor.Exporter
{
    public interface IExporter
    {
        string Name { get; }
        string FileExtension { get; }
        void Export(string filePath, Scene3D scene);
    }
}
