using System;
using System.Reflection;

namespace Advisor.Commands.Entities
{
    public class CommandArgument
    {
        /// <summary>
        /// The type of this argument.
        /// </summary>
        public Type ArgumentType { get; internal set; }
        
        /// <summary>
        /// Reflected parameter info from the command method.
        /// </summary>
        public ParameterInfo Parameter { get; internal set; }
        
        /// <summary>
        /// Whether or not this argument encapsulates the remaining text after the first arguments.
        /// </summary>
        public bool Remainder { get; internal set; }
    }
}