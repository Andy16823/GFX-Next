using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Types
{
    /// <summary>
    /// MetaValue is a wrapper class that encaps a value of a specific 
    /// type (string, int, float, double, bool, or decimal) for use in metadata storage. 
    /// It ensures that only allowed types are stored and provides a 
    /// consistent interface for accessing the value.
    /// </summary>
    public class MetaValue
    {
        /// <summary>
        /// The encapsulated value of the MetaValue instance. It can be of type string, int, float, double, bool, or decimal.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Creates a new instance of MetaValue with the specified value. The constructor validates that the value is of an allowed type
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        public MetaValue(object value)
        {
            if (!IsAllowedType(value))
                throw new ArgumentException("Value must be of type string, int, float, double, bool, or decimal.", nameof(value));

            Value = value;
        }

        /// <summary>
        /// Checks if the provided value is of an allowed type (string, int, float, double, bool, or decimal). This method is used to validate
        /// the value before it is assigned to the MetaValue instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsAllowedType(object value)
        {
            return value is string
                || value is int
                || value is float
                || value is double
                || value is bool
                || value is decimal;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public float ToFloat()
        {
            if (Value is float f)
                return f;
            if (Value is double d)
                return (float)d;
            if (Value is decimal m)
                return (float)m;
            if (Value is int i)
                return i;
            if (Value is bool b)
                return b ? 1f : 0f;
            throw new InvalidCastException("Cannot convert value to float.");
        }

        public int ToInt()
        {
            if (Value is int i)
                return i;
            if (Value is float f)
                return (int)f;
            if (Value is double d)
                return (int)d;
            if (Value is decimal m)
                return (int)m;
            if (Value is bool b)
                return b ? 1 : 0;
            throw new InvalidCastException("Cannot convert value to int.");
        }

        public bool ToBool()
        {
            if (Value is bool b)
                return b;
            if (Value is int i)
                return i != 0;
            if (Value is float f)
                return f != 0f;
            if (Value is double d)
                return d != 0.0;
            if (Value is decimal m)
                return m != 0m;
            throw new InvalidCastException("Cannot convert value to bool.");
        }

        public override bool Equals(object? obj)
        {
            if (obj is MetaValue other)
                return Value.Equals(other.Value);
            return false;
        }
    }
}
