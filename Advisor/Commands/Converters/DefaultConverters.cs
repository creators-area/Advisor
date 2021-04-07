﻿using System;
using System.Globalization;

namespace Advisor.Commands.Converters
{
    /// <summary>
    /// Converts a string argument into a boolean.
    /// </summary>
    public class BoolConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(bool);
        
        public ArgumentConverterResult ConvertArgument(string input)
        {
            if (input == null)
            {
                return ArgumentConverterResult.Failed();
            }
            
            if (bool.TryParse(input, out bool b))
            {
                return ArgumentConverterResult.FromSuccess(b);
            }
            
            if (byte.TryParse(input, out byte by))
            {
                switch (by)
                {
                    case 0: return ArgumentConverterResult.FromSuccess(false);
                    case 1: return ArgumentConverterResult.FromSuccess(true);
                }
            }

            // Let's catch a little more cases, but you really shouldn't be looking for these. 
            input = input.Trim();
            if (input.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
            {
                return ArgumentConverterResult.FromSuccess(true);
            }

            if (input.Equals("no", StringComparison.InvariantCultureIgnoreCase))
            {
                return ArgumentConverterResult.FromSuccess(false);
            }
            
            return ArgumentConverterResult.Failed();
        }
    }
    
    /// <summary>
    /// Converts a string argument into an unsigned byte.
    /// </summary>
    public class ByteConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(byte);

        public ArgumentConverterResult ConvertArgument(string input)
        {
            if (byte.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out byte b))
            {
                return ArgumentConverterResult.FromSuccess(b);
            }

            return ArgumentConverterResult.Failed();
        }
    }
    
    /// <summary>
    /// Converts a string argument into a signed byte.
    /// </summary>
    public class SByteConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(sbyte);

        public ArgumentConverterResult ConvertArgument(string input)
        {
            if (sbyte.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out sbyte sb))
            {
                return ArgumentConverterResult.FromSuccess(sb);
            }

            return ArgumentConverterResult.Failed();
        }
    }
    
    /// <summary>
    /// Converts a string argument into a signed short.
    /// </summary>
    public class ShortConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(short);

        public ArgumentConverterResult ConvertArgument(string input)
        {
            if (short.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out short s))
            {
                return ArgumentConverterResult.FromSuccess(s);
            }

            return ArgumentConverterResult.Failed();
        }
    }
    
    /// <summary>
    /// Converts a string argument into an unsigned short.
    /// </summary>
    public class UShortConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(ushort);

        public ArgumentConverterResult ConvertArgument(string input)
        {
            if (ushort.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out ushort us))
            {
                return ArgumentConverterResult.FromSuccess(us);
            }

            return ArgumentConverterResult.Failed();
        }
    }
    
    /// <summary>
    /// Converts a string argument into a signed integer.
    /// </summary>
    public class IntConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(int);

        public ArgumentConverterResult ConvertArgument(string input)
        {
            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
            {
                return ArgumentConverterResult.FromSuccess(i);
            }

            return ArgumentConverterResult.Failed();
        }
    }

    /// <summary>
    /// Converts a string argument into an unsigned integer.
    /// </summary>
    public class UIntConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(uint);

        public ArgumentConverterResult ConvertArgument(string input)
        {
            if (uint.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint ui))
            {
                return ArgumentConverterResult.FromSuccess(ui);
            }

            return ArgumentConverterResult.Failed();
        }
    }
    
    /// <summary>
    /// Converts a string argument into a signed long.
    /// </summary>
    public class LongConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(long);

        public ArgumentConverterResult ConvertArgument(string input)
        {
            if (long.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out long l))
            {
                return ArgumentConverterResult.FromSuccess(l);
            }

            return ArgumentConverterResult.Failed();
        }
    }

    /// <summary>
    /// Converts a string argument into an unsigned long.
    /// </summary>
    public class ULongConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(ulong);

        public ArgumentConverterResult ConvertArgument(string input)
        {
            if (ulong.TryParse(input, NumberStyles.Integer , CultureInfo.InvariantCulture, out ulong ul))
            {
                return ArgumentConverterResult.FromSuccess(ul);
            }

            return ArgumentConverterResult.Failed();
        }
    }
    
    /// <summary>
    /// Converts a string argument into a float.
    /// </summary>
    public class FloatConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(float);

        public ArgumentConverterResult ConvertArgument(string input)
        {
            if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float f))
            {
                return ArgumentConverterResult.FromSuccess(f);
            }

            return ArgumentConverterResult.Failed();
        }
    }
    
    /// <summary>
    /// Converts a string argument into a double.
    /// </summary>
    public class DoubleConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(double);

        public ArgumentConverterResult ConvertArgument(string input)
        {
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
            {
                return ArgumentConverterResult.FromSuccess(d);
            }

            return ArgumentConverterResult.Failed();
        }
    }
    
    /// <summary>
    /// Converts a string argument into a decimal.
    /// </summary>
    public class DecimalConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(decimal);

        public ArgumentConverterResult ConvertArgument(string input)
        {
            if (decimal.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal d))
            {
                return ArgumentConverterResult.FromSuccess(d);
            }

            return ArgumentConverterResult.Failed();
        }
    }

    /// <summary>
    /// For simplicity's sake, so I don't have to check parameter types all the time.
    /// </summary>
    public class StringConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(string);

        public ArgumentConverterResult ConvertArgument(string input)
        {
            return string.IsNullOrWhiteSpace(input)
                ? ArgumentConverterResult.FromSuccess(input)
                : ArgumentConverterResult.Failed();
        }
    }

    /// <summary>
    /// Converts a string argument intto a char. 
    /// </summary>
    public class CharConverter : IArgumentConverter
    {
        public Type GetConvertedType() => typeof(char);

        public ArgumentConverterResult ConvertArgument(string input)
        {
            if (char.TryParse(input, out char c))
            {
                return ArgumentConverterResult.FromSuccess(c);
            }

            return ArgumentConverterResult.Failed();
        }
    }
}