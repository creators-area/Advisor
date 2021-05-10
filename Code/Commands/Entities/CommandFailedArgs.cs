using System;
using Advisor.Commands.Utils;

namespace Advisor.Commands.Entities
{
    /// <summary>
    /// Arguments of the OnCommandExecuted event.
    /// <seealso cref="CommandHandler.OnCommandFailed"/>
    /// </summary>
    public readonly struct CommandFailedArgs
    {
        /// <summary>
        /// Context of the command that failed, if any.
        /// </summary>
        public CommandContext Context { get; init; }
        
        /// <summary>
        /// The reason the command failed.
        /// </summary>
        public CommandFailureReason Reason { get; init; }
        
        /// <summary>
        /// The error message of the failure if any.
        /// </summary>
        public string ErrorMessage { get; init; }
        
        /// <summary>
        /// The exception that was raised during the execution of the command.
        /// Only available if the failure reason is <see cref="CommandFailureReason.ExceptionThrown"/>
        /// </summary>
        public Exception Exception { get; init; }

        public static CommandFailedArgs FromUnknownCommand(string command)
        {
            var args = new CommandFailedArgs
            {
                Reason = CommandFailureReason.UnknownCommand,
                ErrorMessage = $"Unknown command '{command}'"
            };

            return args;
        }

        public static CommandFailedArgs FromInvalidArguments(CommandContext ctx, ArgumentParserResult result)
        {
            var args = new CommandFailedArgs
            {
                Context = ctx,
                Reason = CommandFailureReason.ArgumentParserError,
                ErrorMessage = result.FailureReason
            };

            return args;
        }

        public static CommandFailedArgs FromCommandException(CommandContext ctx, Exception exception)
        {
            var args = new CommandFailedArgs
            {
                Context = ctx,
                ErrorMessage =
                    $"An exception was thrown while executing the command. Check the server console for more information.",
                Reason = CommandFailureReason.ExceptionThrown,
                Exception = exception
            };

            return args;
        }
    }
}