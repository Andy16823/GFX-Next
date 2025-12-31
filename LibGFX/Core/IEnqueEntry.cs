using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LibGFX.Core.BaseScene;

namespace LibGFX.Core
{
    public interface IEnqueEntry
    {
        /// <summary>
        /// Gets or sets the game element associated with this instance.
        /// </summary>
        public GameElement Element { get; set; }

        public Action<BaseScene, GameElement, Dictionary<string, object>>? EnqueAction { get; set; }
        public Dictionary<string, object>? ExtraData { get; set; }
    }
}
