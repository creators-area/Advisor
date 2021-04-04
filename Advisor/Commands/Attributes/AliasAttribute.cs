using System;
using System.Collections.Generic;
using System.Linq;

namespace Advisor.Commands.Attributes
{
    /// <summary>
    /// Gives additional possible names to a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of this command.
        /// </summary>
        public IReadOnlyList<string> Aliases { get; }

        public AliasAttribute(params string[] aliases)
        {
            Aliases = aliases.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
        }
    }
}