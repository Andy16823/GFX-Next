using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Interface for cloneable objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICloneable<T>
    {
        public T Clone();
    }
}
