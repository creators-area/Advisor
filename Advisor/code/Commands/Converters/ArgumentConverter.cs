using System;
using Advisor.Commands.Entities;

namespace Advisor.Commands.Converters
{
    /// <summary>
    /// Base class for converting a string argument into an object.
    /// Only one argument converter can exist per type. Any duplicates will be ignored.
    /// </summary>
    public interface IArgumentConverter
    {
        /// <summary>
        /// Returns the type that this converter handles.
        /// </summary>
        public Type GetConvertedType();

        /// <summary>
        /// Returns a user friendly name for this type.
        /// </summary>
        public string GetFriendlyTypeName();
        
        /// <summary>
        /// Attempts to convert the given string argument into the type this converter handles.
        /// </summary>
        /// <param name="input"> The string argument to convert. </param>
        /// <returns> The argument conversion result. </returns>
        public ArgumentConverterResult ConvertArgument(CommandContext ctx, string input);
    }
}