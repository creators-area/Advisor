using System.Reflection.Metadata;

namespace Advisor.Commands.Converters
{
    public readonly struct ArgumentConverterResult
    {
        /// <summary>
        /// The converted result, if successful.
        /// </summary>
        public object Result { get; }
        
        /// <summary>
        /// Whether or not the conversion was successfull.
        /// </summary>
        public bool IsSuccessful { get; }

        internal ArgumentConverterResult(bool success, object value = null)
        {
            Result = value;
            IsSuccessful = success;
        }
        
        public static ArgumentConverterResult FromSuccess(object value)
        {
            return new(true, value);
        }

        public static ArgumentConverterResult Failed()
        {
            return new(false);
        }
    }
}