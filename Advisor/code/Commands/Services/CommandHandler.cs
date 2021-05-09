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
            
            // Sub command takes priority.
            if (subCommand != null)
            {
                var subRaw = subCommand.Arguments.Count > 0
                    ? arguments.TakeLast(arguments.Length - 2).ToList()
                    : new List<string>();
                
                var subContext = new CommandContext
                {
                    Advisor = _advisor,
                    Caller = sender,
                    Command = subCommand,
                    Message = text,
                    RanOnConsole = false,
                    RawArguments = subRaw,
                };
                
                // Execute the command now if it doesn't require any arguments.
                if (subCommand.MethodDelegateNoParams != null)
                {
                    subCommand.MethodDelegateNoParams(subContext);
                    return;
                }
                
                var subResult = ArgumentParser.Parse(subContext, subCommand.Arguments, subRaw);
                if (!subResult.IsSuccessful)
                {
                    if (rootCommand == null)
                    {
                        var args = CommandFailedArgs.FromInvalidArguments(subContext, subResult);
                        OnCommandFailed(args);
                        // TODO: Message the user with the failure reason.
                        return;
                    }
                }
                else
                {
                    // Execute the command.
                    var objs = new object[subResult.Arguments.Length + 1];
                    objs[0] = subContext;
                    for (int i = 1; i < objs.Length; i++)
                    {
                        objs[i] = subResult.Arguments[i - 1];
                    }

                    try
                    {
                        subCommand.ExecuteCommand(objs);
                        var successArgs = new CommandExecutedArgs
                        {
                            Context = subContext
                        };
                        
                        OnCommandExecuted(successArgs);
                    }
                    catch (Exception e)
                    {
                        var failureArgs = CommandFailedArgs.FromCommandException(subContext, e);
                        OnCommandFailed(failureArgs);
                        AdvisorLog.Error(e, $"Command '{subContext.Command.FullName}' has thrown an exception during execution: ");
                        // TODO: Tell user the command fucked up.
                        return;
                    }
                }
            }
        }
    }
}