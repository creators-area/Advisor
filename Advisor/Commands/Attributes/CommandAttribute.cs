using System;
using System.Text.RegularExpressions;
using Advisor.Enums;

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
        /// Must be alphanumerical, allows for hyphens and underscores.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The realm on which this command will be executed.
        /// </summary>
        public SandboxRealm ExecutionRealm { get; }

        /// <summary>
        /// Whether or not this command's name is valid.
        /// </summary>
        public bool HasValidCommandName()
        {
            return !string.IsNullOrWhiteSpace(Name) && new Regex("^[a-zA-Z0-9_-]+$").IsMatch(Name);
        }

        public CommandAttribute(string name, SandboxRealm executesOn)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                // This will throw on command registration.
                Name = null;
            }
            else
            {
                Name = name.Trim();
            }
            
            ExecutionRealm = executesOn;
        }
    }
}