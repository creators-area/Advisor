using System.Collections.Generic;
using Sandbox;

namespace Advisor.Commands.Entities
{
    /// <summary>
    /// Holds the current context in an executed command.
    /// </summary>
    public struct CommandContext
    {
        /// <summary>
        /// A reference to the AdvisorAddon instance.
        /// </summary>
        public AdvisorCore Advisor { get; internal set; }
        
        /// <summary>
        /// The command that was ran.
        /// </summary>
        public Command Command { get; internal set; }
        
        /// <summary>
        /// The player who ran the command, unless it was the server.
        /// </summary>
        public Player Caller { get; internal set; }
        
        /// <summary>\
        /// If so, there won't be any caller.
        /// </summary>
        public bool RanOnConsole { get; internal set; }
        
        /// <summary>
        /// The raw message that instigated the command.
        /// If RanOnConsole is set, this will be the string that was typed in the console.
        /// TODO: Check if possible.
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        /// The raw arguments in string from (before they were converted), if any.
        /// </summary>
        public IReadOnlyList<string> RawArguments => InternalRawArguments;
        
        /// <summary>
        /// Mutable raw arguments for internal usage only.
        /// </summary>
        internal List<string> InternalRawArguments { get; set; }
    }
}