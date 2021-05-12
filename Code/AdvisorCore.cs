// Feel free to check if ADVISOR is defined in your projects to enable/disable certain functionalities.
#define ADVISOR

using System.Diagnostics;
using System.Reflection;
using Advisor.Commands.Services;
using Advisor.Configuration;
using Advisor.DependencyInjection;
using Advisor.Utils;
using Sandbox;

namespace Advisor
{
    /// <summary>
    /// Entry point of Advisor.
    /// </summary>
    [Library("advisor", Title = "Advisor")]
    public class AdvisorCore
    {
        private CommandRegistry _commandRegistry;
        private CommandHandler _commandHandler;
        private ConfigurationService _configuration;
        private AdvisorServiceContainer _services;

        public AdvisorCore()
        {
	        InitializeAdvisor();
        }

        /// <summary>
        /// Get the service container of Advisor.
        /// </summary>
        /// <returns> The IServiceProvider Advisor uses for Dependency Injection </returns>
        public AdvisorServiceContainer GetServices() => _services;

        /// <summary>
        /// Returns a service by type.
        /// Shorthand version of GetServices().GetService<T>();
        /// </summary>
        /// <typeparam name="T"> The type of the service to get. </typeparam>
        /// <returns> The service if it exists, else null. </returns>
        public T GetService<T>() where T : class => _services.GetService<T>();
        
        private void InitializeAdvisor()
        {
	        AdvisorLog.Info("Initializing Advisor Core...");
	        var sw = new Stopwatch();
            _services = new AdvisorServiceContainer(this);

            _configuration = new ConfigurationService();
            _configuration.LoadConfiguration();
            _services.AddService(_configuration);

            _commandRegistry = new CommandRegistry(this);
            _services.AddService(_commandRegistry);

            // Register Advisor's argument converters and commands.
            _commandRegistry.RegisterArgumentConverters(Assembly.GetExecutingAssembly());
            _commandRegistry.RegisterCommandModules(Assembly.GetExecutingAssembly());

            _commandHandler = new CommandHandler(this);
            _services.AddService(_commandHandler);
            
            AdvisorLog.Info($"Successfully initialized Advisor Core in {sw.ElapsedMilliseconds} ms");
        }
    }
}
