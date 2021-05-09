namespace Advisor.Commands.Entities
{
    public enum CommandFailureReason
    {
        /// <summary>
        /// Attempted to execute an unknown command.
        /// </summary>
        UnknownCommand,

        /// <summary>
        /// The argument parser failed to parse text into command arguments.
        /// </summary>
        ArgumentParserError,
        
        /// <summary>
        /// The command threw an exception during execution,
        /// </summary>
        ExceptionThrown,
    }
}