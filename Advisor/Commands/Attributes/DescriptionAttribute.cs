using System;

namespace Advisor.Commands.Attributes
{
    /// <summary>
    /// Gives a readable description to a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// Gets the description of this command.
        /// </summary>
        public string Description { get; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}