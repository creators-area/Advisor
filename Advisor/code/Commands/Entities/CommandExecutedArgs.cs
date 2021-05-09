using Advisor.Commands.Services;

namespace Advisor.Commands.Entities
{
    /// <summary>
    /// Arguments of the OnCommandExecuted event.
    /// <seealso cref="CommandHandler.CommandExecuted"/>
    /// </summary>
    public struct CommandExecutedArgs
    {
        /// <summary>
        /// The context of the command that was executed.
        /// </summary>
        public CommandContext Context { get; internal set; }
    }
}