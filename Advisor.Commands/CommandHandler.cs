using System;
using System.Linq;
using System.Reflection;
using Advisor.Commands.Attributes;
using Advisor.Commands.Enums;


namespace Advisor.Commands
{
    public class CommandHandler
    {
        public void InitializeCommands()
        {
            var commandClasses = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(CommandModule)))
                .ToList();
        }
    }
}