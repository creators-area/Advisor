using System.ComponentModel.Design;
using System.Reflection;
using Advisor.Commands;
using Advisor.Configuration;
using Advisor.Extensions;

namespace Advisor
{
    /// <summary>
    /// Entry point of Advisor.
    /// I have no idea how this works in &box right now though, so it'll likely need a small refactor.
    /// </summary>
    public class AdvisorAddon
    {
        private CommandRegistry _commandRegistry;
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
        }

        public T GetService<T>()
        {
            return _services.GetRequiredService<T>();
        }
    }
}