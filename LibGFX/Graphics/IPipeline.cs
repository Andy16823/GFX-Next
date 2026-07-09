using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public class PipelineScope : IDisposable
    {
        private IPipeline _pipeline;
        public PipelineScope(IPipeline pipeline)
        {
            _pipeline = pipeline;
        }
        public void Dispose()
        {
            _pipeline.End();
        }
    }

    public interface IPipeline
    {
        public PipelineScope Begin();
        public void End();
    }
}
