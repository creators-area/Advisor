using System;
using System.Linq;
using System.Reflection;
using Overwatch.Commands.Attributes;
using Overwatch.Commands.Enums;


namespace Overwatch.Commands
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