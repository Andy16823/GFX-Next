using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public class RenderPassScope : IDisposable
    {
        private IRenderPass _renderPass;

        public RenderPassScope(IRenderPass renderPass)
        {
            _renderPass = renderPass;
        }

        public void Dispose()
        {
            _renderPass.End();
        }
    }


    public interface IRenderPass
    {
        public RenderPassScope Begin();
        public void End();
    }
}
