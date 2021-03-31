using System;
using Advisor.Commands.Enums;

namespace Advisor.Commands.Attributes
{
    /// <summary>
    /// Marks a method as an ingame command callable by clients with permission.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of this command.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The realm on which this command will be executed.
        /// </summary>
        public TargetRealm ExecutionTarget { get; }

        public CommandAttribute(string name, TargetRealm executesOn)
        {
            Name = name;
            ExecutionTarget = executesOn;
        }
    }
}