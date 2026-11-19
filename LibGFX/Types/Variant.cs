using System;
using System.Collections.Generic;
using System.Text;

namespace LibGFX.Types
{
    /// <summary>
    /// Type-safe variant type that can hold a value of any type.
    /// </summary>
    public readonly struct Variant
    {
        private readonly object? _value;

        public Variant(object? value)
        {
            _value = value;
        }

        public bool Is<T>() => _value is T;
        public T? As<T>() => _value is T value ? value : default;
        public object? Value => _value;

        public bool TryGetValue<T>(out T? value)
        {
            if (_value is T v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }
    }
}
