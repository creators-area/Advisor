using System;
using System.Collections.Generic;
using System.Linq;
using Advisor.Commands.Entities;
using Advisor.Commands.Utils;
using Advisor.Configuration;
using Advisor.Utils;
using Sandbox;
using Sandbox.Hooks;

namespace Advisor.Commands.Services
{
    /// <summary>
    /// Handles commands from the chat and console.
    /// </summary>
    public class CommandHandler
    {
        private readonly AdvisorCore _advisor;
        private readonly CommandRegistry _commands;
        private readonly ConfigurationService _configService;

        /// <summary>
        /// Called when a command was executed succesfully.
        /// </summary>
        public event Action<CommandExecutedArgs> CommandExecuted;

        /// <summary>
        /// Called when a command didn't execute correctly.
        /// </summary>
        public event Action<CommandFailedArgs> CommandFailed;
        
        internal CommandHandler(AdvisorCore advisor)
        {
            _advisor = advisor;
            _commands = advisor.GetService<CommandRegistry>();
            _configService = advisor.GetService<ConfigurationService>();

            Chat.OnChatMessage += args =>
            {
				HandleChatMessage(args.Sender, args.Message);
            };
        }

        public void HandleChatMessage(Player sender, string text)
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
                    CommandFailed?.Invoke(failureArgs);
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
                            CommandFailed?.Invoke(failureArgs);
                            // TODO: Tell player they've typed an unknown command.
                            // Could not find any command named 'first'.
                            return;
                        }
                    }
                }
            }

            Command subCommand = null;
            if (arguments.Length > 1)
            {
                // Check if we've got a subcommand to call too.
                var second = arguments[1].ToLower();
                if (!_commands.TryGetCommand(first, second, out subCommand))
                {
                    if (rootCommand == null)
                    {
                        var failureArgs = CommandFailedArgs.FromUnknownCommand($"{first} {second}");
                        CommandFailed?.Invoke(failureArgs);
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
                                CommandFailed?.Invoke(failureArgs);
                                // TODO: Tell player they've typed an unknown command.
                                // Could not find any command named 'first'.
                                return;
                            }
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
                    if (subCommandSuccess == null)
                    {
                        throw new InvalidOperationException(
                            $"Commands that successfully executed should return a valid CommandExecutedArgs!");
                    }
                    
                    CommandExecuted?.Invoke(subCommandSuccess.Value);
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
                    if (rootCommandSuccess == null)
                    {
                        throw new InvalidOperationException(
                            $"Commands that successfully executed should return a valid CommandExecutedArgs!");
                    }
                    CommandExecuted?.Invoke(rootCommandSuccess.Value);
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
                        //throw new NotImplementedException();
                    case CommandFailureReason.ExceptionThrown:
                        // TODO: Notify user of command exception.
                        //throw new NotImplementedException();
                    default:
                        // TODO: Notify user of unknown error (notify server owner to check console and pester me).
                        //throw new NotImplementedException();
                        break;
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

            // Execute the command now if it doesn't require any arguments.
            if (command.MethodDelegateNoParams != null)
            {
                try
                {
                    command.MethodDelegateNoParams(context);
                    successArgs = new CommandExecutedArgs
                    {
                        Context = context,
                    };
                    CommandExecuted?.Invoke(successArgs.Value);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }

            var parseResult = ArgumentParser.Parse(context, command.Arguments, context.InternalRawArguments);
            if (!parseResult.IsSuccessful)
            {
                failureArgs = CommandFailedArgs.FromInvalidArguments(context, parseResult);
                CommandFailed?.Invoke(failureArgs.Value);
                return false;
            }
            else
            {
                // Execute the command.
                var objs = new object[parseResult.Arguments.Length + 1];
                objs[0] = context;
                for (int i = 1; i < objs.Length; i++)
                {
                    objs[i] = parseResult.Arguments[i - 1];
                }

                try
                {
                    command.Method.Invoke(command.ParentModule, objs);
                    successArgs = new CommandExecutedArgs
                    {
                        Context = context
                    };

                    CommandExecuted?.Invoke(successArgs.Value);
                    return true;
                }
                catch (Exception e)
                {
                    failureArgs = CommandFailedArgs.FromCommandException(context, e);
                    CommandFailed?.Invoke(failureArgs.Value);
                    AdvisorLog.Error(e, $"Command '{context.Command.FullName}' has thrown an exception during execution: ");
                    return false;
                }
            }
        }
    }
}
