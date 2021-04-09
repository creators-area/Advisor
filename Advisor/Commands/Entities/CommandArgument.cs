using System;
using System.Reflection;
using Advisor.Commands.Converters;

namespace Advisor.Commands.Entities
{
    public class CommandArgument
    {
        /// <summary>
        /// The type of this argument.
        /// </summary>
        public Type ArgumentType { get; internal set; }
        
        /// <summary>
        /// The converter for this argument's type.
        /// </summary>
        public IArgumentConverter Converter { get; internal set; }
        
        /// <summary>
        /// Reflected parameter info from the command method.
        /// </summary>
        public ParameterInfo Parameter { get; internal set; }
        
        /// <summary>
        /// Whether or not this argument encapsulates the remaining text after the first arguments.
        /// Only available for string arguments.
        /// </summary>
        public bool Remainder { get; internal set; }
        
        /// <summary>
        /// Whether or not this argument has a variable number of arguments.
        /// If so, the command arguments will be read and parsed into an array until there are none left or they cannot be parsed.
        /// </summary>
        public bool IsParams { get; internal set; }
    }
}