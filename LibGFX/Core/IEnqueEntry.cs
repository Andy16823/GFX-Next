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

        /// <summary>
        /// Action delegate that defines the behavior to be executed when this instance is processed in the scene's update loop.
        /// </summary>
        public Action<BaseScene, GameElement, Dictionary<string, object>>? EnqueAction { get; set; }

        /// <summary>
        /// Extra data that can be associated with this instance, allowing for additional information to be passed to the EnqueAction when it is executed.
        /// </summary>
        public Dictionary<string, object>? ExtraData { get; set; }
    }
}
