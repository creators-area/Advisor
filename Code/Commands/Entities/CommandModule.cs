using System;
using System.Collections.Generic;
using Advisor.Commands.Attributes;
using Sandbox;

namespace Advisor.Commands.Entities
{
    /// <summary>
    /// The base class for any module meant to create Advisor commands.
    /// </summary>
    [Library]
    public class CommandModule
    {
        /// <summary>
        /// The category commands should be under if any (can be null!)
        /// </summary>
        public string Category { get; internal set; }
        
        /// <summary>
        /// The name that will be used to create permissions for commands under this module.
        /// </summary>
        public string PermissionName { get; internal set; }
        
        /// <summary>
        /// The prefix that will be applied to commands, if any.
        /// </summary>
        public string Prefix { get; internal set; }

        /// <summary>
        /// Commands registered from this command module.
        /// </summary>
        public IReadOnlyList<Command> Commands => InternalCommands;
        
        /// <summary>
        /// Writable version of Commands.
        /// </summary>
        internal List<Command> InternalCommands { get; }

        internal CommandModule()
        {
            InternalCommands = new List<Command>();
            Category = "Uncategorized";
        }
    }
}
