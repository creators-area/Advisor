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

        /// <summary>
        /// If not null, the reason the conversion failed.
        /// </summary>
        public string FailureReason { get; }

        internal ArgumentConverterResult(bool success, object value = null, string failureReason = null)
        {
            Result = value;
            IsSuccessful = success;
            FailureReason = failureReason;
        }
        
        public static ArgumentConverterResult FromSuccess(object value)
        {
            return new(true, value);
        }

        public static ArgumentConverterResult Failed()
        {
            return new(false);
        }

        public static ArgumentConverterResult Failed(string reason)
        {
            return new(false, null, reason);
        }
    }
}