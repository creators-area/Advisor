using System;
using System.Collections.Generic;
using System.Linq;
using Advisor.Commands.Entities;
using Advisor.Commands.Utils;
using Advisor.Configuration;
using Sandbox;

namespace Advisor.Commands.Services
{
    /// <summary>
    /// Handles commands from the chat and console.
    /// </summary>
    public class CommandHandler
    {
        private readonly AdvisorAddon _advisor;
        private readonly CommandRegistry _commands;
        private readonly ConfigurationService _configService;

        /// <summary>
        /// Called when a command was executed succesfully.
        /// </summary>
        public Action<CommandExecutedArgs> OnCommandExecuted { get; set; }
        
        /// <summary>
        /// Called when a command didn't execute correctly.
        /// </summary>
        public Action<CommandFailedArgs> OnCommandFailed { get; set; }
        
        internal CommandHandler(AdvisorAddon advisor)
        {
            _advisor = advisor;
            _commands = advisor.GetService<CommandRegistry>();
            _configService = advisor.GetService<ConfigurationService>();
        }

        private void HandleChatMessage(Player sender, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var input = text.Trim();
            var comparison = _configService.Configuration.CaseSensitivePrefix
                ? StringComparison.CurrentCultureIgnoreCase
                : StringComparison.CurrentCulture;
            
            // Make sure the command starts with the prefix.
            var pos = text.IndexOf(_configService.Configuration.Prefix, comparison);
            if (pos != 0)
            {
                return;
            }

            input = input[_configService.Configuration.Prefix.Length..];
            
            var arguments = ArgumentParser.ToStringArray(input);
            if (arguments.Length == 0)
            {
                return;
            }
            
            // Check if the first argument relates to a root command.
            var first = arguments[0].ToLower();
            if (!_commands.TryGetCommand(first, out var rootCommand))
            {
                if (arguments.Length == 1)
                {
                    var failureArgs = CommandFailedArgs.FromUnknownCommand(first);
                    OnCommandFailed(failureArgs);
                    // TODO: Tell player they've typed an unknown command.
                    // Could not find any command named 'first'.
                    return;
                }
            }
            else
            {
                // Compare with the configured case sensitivity if needed.
                if (_configService.Configuration.CaseSensitiveCommands)
                {
                    if (!rootCommand.Name.Equals(first, StringComparison.CurrentCulture))
                    {
                        rootCommand = null;
                        if (arguments.Length == 1)
                        {
                            var failureArgs = CommandFailedArgs.FromUnknownCommand(first);
                            OnCommandFailed(failureArgs);
                            // TODO: Tell player they've typed an unknown command.
                            // Could not find any command named 'first'.
                            return;
                        }
                    }
                }
            }

            // Check if we've got a subcommand to call too.
            var second = arguments[1].ToLower();
            if (!_commands.TryGetCommand(first, second, out var subCommand))
            {
                if (rootCommand == null)
                {
                    var failureArgs = CommandFailedArgs.FromUnknownCommand($"{first} {second}");
                    OnCommandFailed(failureArgs);
                    // TODO: Tell player they've typed an unknown command.
                    // Could not find any command named 'first' or 'first second'.
                    return;
                }
            }
            else
            {
                // Compare with the configured case sensitivity if needed.
                if (_configService.Configuration.CaseSensitiveCommands)
                {
                    if (!subCommand.FullName.Equals($"{first} {second}", StringComparison.CurrentCulture))
                    {
                        subCommand = null;
                        if (rootCommand == null)
                        {
                            var failureArgs = CommandFailedArgs.FromUnknownCommand($"{first} {second}");
                            OnCommandFailed(failureArgs);
                            // TODO: Tell player they've typed an unknown command.
                            // Could not find any command named 'first'.
                            return;
                        }
                    }
                }
            }

            CommandExecutedArgs? rootCommandSuccess = null, subCommandSuccess = null;
            CommandFailedArgs? rootCommandFailure = null, subCommandFailure = null;
            bool rootSuccess, subSuccess;
            
            // Sub command takes priority.
            if (subCommand != null)
            {
                var rawArgs = arguments.Length > 2
                    ? arguments.TakeLast(arguments.Length - 2).ToList()
                    : new List<string>();
                
                var context = new CommandContext
                {
                    Advisor = _advisor,
                    Caller = sender,
                    Command = subCommand,
                    Message = text,
                    RanOnConsole = false,
                    InternalRawArguments = rawArgs
                };
                
                subSuccess = TryExecuteCommand(context, out subCommandSuccess, out subCommandFailure);
                if (subSuccess)
                {
                    return;
                }
            }
            
            // Execute the root command if any.
            if (rootCommand != null)
            {
                var rawArgs = arguments.Length > 1
                    ? arguments.TakeLast(arguments.Length - 1).ToList()
                    : new List<string>();
                
                var context = new CommandContext
                {
                    Advisor = _advisor,
                    Caller = sender,
                    Command = rootCommand,
                    Message = text,
                    RanOnConsole = false,
                    InternalRawArguments = rawArgs
                };
                
                rootSuccess = TryExecuteCommand(context, out rootCommandSuccess, out rootCommandFailure);
                if (rootSuccess)
                {
                    return;
                }
            }
            
            // If we're here, it means one or two commands failed to execute.
            // So let's just notify the user in chat, print stuff in the console if needed and call it a day.
            if (rootCommand != null)
            {
                var failure = rootCommandFailure!.Value;
                switch (failure.Reason)
                {
                    case CommandFailureReason.UnknownCommand:
                        throw new InvalidOperationException(
                            $"A command returned a CommandFailureReason of 'UnknownCommand'. This should never happen if we've found it!");
                    case CommandFailureReason.ArgumentParserError:
                        // TODO: Notify user of unknown arguments.
                        throw new NotImplementedException();
                    case CommandFailureReason.ExceptionThrown:
                        // TODO: Notify user of command exception.
                        throw new NotImplementedException();
                    default:
                        // TODO: Notify user of unknown error (notify server owner to check console and pester me).
                        throw new NotImplementedException();
                }
            }
            
            if (subCommand != null)
            {
                var failure = subCommandFailure!.Value;
                switch (failure.Reason)
                {
                    case CommandFailureReason.UnknownCommand:
                        throw new InvalidOperationException(
                            $"A command returned a CommandFailureReason of 'UnknownCommand'. This should never happen if we've found it!");
                    case CommandFailureReason.ArgumentParserError:
                        // TODO: Notify user of unknown arguments.
                        throw new NotImplementedException();
                    case CommandFailureReason.ExceptionThrown:
                        // TODO: Notify user of command exception.
                        throw new NotImplementedException();
                    default:
                        // TODO: Notify user of unknown error (notify server owner to check console and pester me).
                        throw new NotImplementedException();
                }
            }
        }

        // TODO: Clean this up a little in the future.
        private bool TryExecuteCommand(CommandContext context, out CommandExecutedArgs? successArgs, 
            out CommandFailedArgs? failureArgs)
        {
            successArgs = null;
            failureArgs = null;
            
            var command = context.Command;
            var arguments = context.RawArguments;

            // Execute the command now if it doesn't require any arguments.
            if (command.MethodDelegateNoParams != null)
            {
                try
                {
                    command.MethodDelegateNoParams(context);
                    var args = new CommandExecutedArgs
                    {
                        Context = context,
                    };
                    OnCommandExecuted(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }

            var subResult = ArgumentParser.Parse(context, command.Arguments, context.InternalRawArguments);
            if (!subResult.IsSuccessful)
            {
                failureArgs = CommandFailedArgs.FromInvalidArguments(context, subResult);
                OnCommandFailed(failureArgs.Value);
                return false;
            }
            else
            {
                // Execute the command.
                var objs = new object[subResult.Arguments.Length + 1];
                objs[0] = context;
                for (int i = 1; i < objs.Length; i++)
                {
                    objs[i] = subResult.Arguments[i - 1];
                }

                try
                {
                    command.ExecuteCommand(objs);
                    successArgs = new CommandExecutedArgs
                    {
                        Context = context
                    };

                    OnCommandExecuted(successArgs.Value);
                    return true;
                }
                catch (Exception e)
                {
                    failureArgs = CommandFailedArgs.FromCommandException(context, e);
                    OnCommandFailed(failureArgs.Value);
                    AdvisorLog.Error(e, $"Command '{context.Command.FullName}' has thrown an exception during execution: ");
                    return false;
                }
            }
        }
    }
}