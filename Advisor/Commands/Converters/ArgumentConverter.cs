using System;

namespace Advisor.Commands.Converters
{
    /// <summary>
    /// Base class for converting a string argument into the given generic type.
    /// Only one argument converter can exist per type. Any duplicates will be ignored.
    /// </summary>
    /// <typeparam name="T"> The type this converter handles. </typeparam>
    public abstract class ArgumentConverter<T>
    {
        /// <summary>
        /// Returns the type handled by this argument converter.
        /// </summary>
        public Type GetHandledType()
        {
            return typeof(T);
        }

        // TODO: Pass in the context as an argument. Could be useful for converting arguments differently based on context.
        // Or also to get an easier access to Advisor or S&box.
        
        /// <summary>
        /// Attempts to convert the given string argument into the type this converter handles.
        /// </summary>
        /// <param name="input"> The string argument to convert. </param>
        /// <param name="convertedObject"> The converted object, or default if it failed. </param>
        /// <returns> True if we succesfully converted the argument. </returns>
        public abstract bool TryConvertArgument(string input, out T convertedObject);
    }
}