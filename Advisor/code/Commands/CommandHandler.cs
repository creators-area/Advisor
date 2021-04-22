using System;
using Advisor.Commands.Entities;
using Advisor.Commands.Utils;
using Advisor.Configuration;
using Sandbox;

namespace Advisor.Commands
{
    /// <summary>
    /// Handles commands from the chat and console.
    /// </summary>
    public class CommandHandler
    {
        private readonly AdvisorAddon _advisor;
        private readonly CommandRegistry _commands;
        private readonly ConfigurationService _configuration;
        
        internal CommandHandler(AdvisorAddon advisor)
        {
            _advisor = advisor;
            _commands = advisor.GetService<CommandRegistry>();
            _configuration = advisor.GetService<ConfigurationService>();
        }

        private void HandleChatMessage(Player sender, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var input = text.Trim();
            var comparison = _configuration.Configuration.CaseSensitivePrefix
                ? StringComparison.CurrentCultureIgnoreCase
                : StringComparison.CurrentCulture;
            
            // Make sure the command starts with the prefix.
            var pos = text.IndexOf(_configuration.Configuration.Prefix, comparison);
            if (pos != 0)
            {
                return;
            }

            input = input[_configuration.Configuration.Prefix.Length..];
            
            var arguments = ArgumentParser.ToStringArray(input);
            if (arguments.Length == 0)
            {
                return;
            }
            
            // Check if the first argument relates to a root command.
            var first = arguments[0].ToLower();
            Command rootCommand;
            if (!_commands.TryGetCommand(first, out rootCommand))
            {
                return;
            }
            
            
        }
    }
}