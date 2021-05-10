// Feel free to check if ADVISOR is defined in your projects to enable/disable certain functionalities.
#define ADVISOR

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using Advisor.Commands.Services;
using Advisor.Configuration;
using Advisor.Extensions;
using Advisor.Utils;
using Sandbox;

namespace Advisor
{
    /// <summary>
    /// Entry point of Advisor.
    /// </summary>
    [Library("advisor", Title = "Advisor")]
    public class AdvisorCore : Game
    {
        private CommandRegistry _commandRegistry;
        private CommandHandler _commandHandler;
        private ConfigurationService _configuration;
        private ServiceContainer _services;

        public AdvisorCore()
        {
            InitializeAdvisor();
        }

        private void InitializeAdvisor()
        {
	        AdvisorLog.Info("Initializing Advisor Core...");
	        var sw = new Stopwatch();
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
            
            AdvisorLog.Info($"Successfully initialized Advisor Core in {sw.ElapsedMilliseconds} ms");
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
