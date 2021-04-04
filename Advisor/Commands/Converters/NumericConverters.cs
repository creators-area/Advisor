using System.Globalization;

namespace Advisor.Commands.Converters
{
    /// <summary>
    /// Converts a string argument into an unsigned byte.
    /// </summary>
    public class ByteConverter : ArgumentConverter<byte>
    {
        public override bool TryConvertArgument(string input, out byte convertedNumber)
        {
            return byte.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out convertedNumber);
        }
    }
    
    /// <summary>
    /// Converts a string argument into a signed byte.
    /// </summary>
    public class SByteConverter : ArgumentConverter<sbyte>
    {
        public override bool TryConvertArgument(string input, out sbyte convertedNumber)
        {
            return sbyte.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out convertedNumber);
        }
    }
    
    /// <summary>
    /// Converts a string argument into a signed short.
    /// </summary>
    public class ShortConverter : ArgumentConverter<short>
    {
        public override bool TryConvertArgument(string input, out short convertedNumber)
        {
            return short.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out convertedNumber);
        }
    }
    
    /// <summary>
    /// Converts a string argument into an unsigned short.
    /// </summary>
    public class UShortConverter : ArgumentConverter<ushort>
    {
        public override bool TryConvertArgument(string input, out ushort convertedNumber)
        {
            return ushort.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out convertedNumber);
        }
    }
    
    /// <summary>
    /// Converts a string argument into a signed integer.
    /// </summary>
    public class IntConverter : ArgumentConverter<int>
    {
        public override bool TryConvertArgument(string input, out int convertedNumber)
        {
            return int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out convertedNumber);
        }
    }

    /// <summary>
    /// Converts a string argument into an unsigned integer.
    /// </summary>
    public class UIntConverter : ArgumentConverter<uint>
    {
        public override bool TryConvertArgument(string input, out uint convertedNumber)
        {
            return uint.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out convertedNumber);
        }
    }
    
    /// <summary>
    /// Converts a string argument into a signed long.
    /// </summary>
    public class LongConverter : ArgumentConverter<long>
    {
        public override bool TryConvertArgument(string input, out long convertedNumber)
        {
            return long.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out convertedNumber);
        }
    }

    /// <summary>
    /// Converts a string argument into an unsigned long.
    /// </summary>
    public class ULongConverter : ArgumentConverter<ulong>
    {
        public override bool TryConvertArgument(string input, out ulong convertedNumber)
        {
            return ulong.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out convertedNumber);
        }
    }
    
    /// <summary>
    /// Converts a string argument into a float.
    /// </summary>
    public class FloatConverter : ArgumentConverter<float>
    {
        public override bool TryConvertArgument(string input, out float convertedNumber)
        {
            return float.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out convertedNumber);
        }
    }
    
    /// <summary>
    /// Converts a string argument into a double.
    /// </summary>
    public class DoubleConverter : ArgumentConverter<double>
    {
        public override bool TryConvertArgument(string input, out double convertedNumber)
        {
            return double.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out convertedNumber);
        }
    }
    
    /// <summary>
    /// Converts a string argument into a decimal.
    /// </summary>
    public class DecimalConverter : ArgumentConverter<decimal>
    {
        public override bool TryConvertArgument(string input, out decimal convertedNumber)
        {
            return decimal.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out convertedNumber);
        }
    }
}