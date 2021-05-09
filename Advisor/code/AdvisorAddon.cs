﻿// Feel free to check if ADVISOR is defined in your projects to enable/disable certain functionalities.
#define ADVISOR

using System;
using System.ComponentModel.Design;
using System.Reflection;
using Advisor.Commands.Services;
using Advisor.Configuration;
using Advisor.Extensions;
using Sandbox;

namespace Advisor
{
    /// <summary>
    /// Entry point of Advisor.
    /// I have no idea how this works in s&box right now though, so it'll likely need a small refactor.
    /// </summary>
    [Library("advisor")]
    public class AdvisorAddon
    {
        private CommandRegistry _commandRegistry;
        private CommandHandler _commandHandler;
        private ConfigurationService _configuration;
        private ServiceContainer _services;

        public AdvisorAddon()
        {
            InitializeAdvisor();
        }

        private void InitializeAdvisor()
        {
            _services = new ServiceContainer();

            _configuration = new ConfigurationService();
            _configuration.LoadConfiguration();
            _services.AddService(typeof(ConfigurationService), _configuration);
                        
            _commandRegistry = new CommandRegistry(this);
            _services.AddService(typeof(CommandRegistry), _commandRegistry);

            // Register Advisor's argument converters and commands.
            _commandRegistry.RegisterArgumentConverters(Assembly.GetExecutingAssembly());
            _commandRegistry.RegisterCommandModules(Assembly.GetExecutingAssembly());

            _commandHandler = new CommandHandler(this);
            _services.AddService(typeof(CommandHandler), _commandHandler);
        }

        public T GetService<T>()
        {
            var svc = _services.GetRequiredService<T>();
            if (svc == null)
            {
                throw new InvalidOperationException($"Could not get unknown service '{typeof(T).FullName}'");
            }

            return svc;
        }
    }
}