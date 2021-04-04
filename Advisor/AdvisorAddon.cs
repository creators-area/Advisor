using System;
using System.ComponentModel.Design;
using System.Reflection;
using Advisor.Commands;
using Advisor.Extensions;

namespace Advisor
{
    /// <summary>
    /// Entry point of Advisor.
    /// I have no idea how this works in S&box right now though, so it'll likely need a small refactor.
    /// </summary>
    public class AdvisorAddon
    {
        private CommandHandler _commandHandler;
        private ServiceContainer _services;

        public AdvisorAddon()
        {
            InitializeAdvisor();
        }

        private void InitializeAdvisor()
        {
            _services = new ServiceContainer();
            _commandHandler = new CommandHandler(this);
            _services.AddService(typeof(CommandHandler), _commandHandler);

            // Register Advisor's commands.
            _commandHandler.RegisterCommandModules(Assembly.GetExecutingAssembly());
        }
    }
}