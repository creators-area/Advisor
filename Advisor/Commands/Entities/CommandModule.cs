using System;
using System.Collections.Generic;
using Advisor.Commands.Attributes;

namespace Advisor.Commands.Entities
{
    /// <summary>
    /// The base class for any module meant to create Advisor commands.
    /// </summary>
    public class CommandModule
    {
        /// <summary>
        /// The category commands should be under if any (can be null!)
        /// </summary>
        public string Category { get; internal set; }
        
        /// <summary>
        /// Commands registered from this command module.
        /// </summary>
        public IReadOnlyList<Command> Commands { get; internal set; }

        internal CommandModule()
        {
            Commands = new List<Command>();
            Category = "Uncategorized";
        }
    }
}