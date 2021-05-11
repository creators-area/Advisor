using System;
//using System.IO;
using System.Text.Json;

namespace Advisor.Configuration
{
    /// <summary>
    /// Handles the base configuration of Advisor.
    /// </summary>
    public class ConfigurationService
    {
        public static readonly string ConfigurationFileName = "advisorConfiguration.json";
        public static readonly string ConfigurationTemplateName = "advisorConfiguration.template.json";

        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// The configuration object.
        /// </summary>
        public ConfigurationModel Configuration { get; private set; }
        
        /// <summary>
        /// Raised every time the configuration is either modified and saved, or loaded.
        /// </summary>
        public Action<ConfigurationModel> OnConfigurationModified{ get; }

        internal ConfigurationService()
        {
            Configuration = new ConfigurationModel();
            _serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
            };
            
            var json = JsonSerializer.Serialize(Configuration, _serializerOptions);
            
            // File.WriteAllText(ConfigurationTemplateName, json);
            // if (!File.Exists(ConfigurationFileName))
            // {
            //     File.WriteAllText(ConfigurationFileName, json);
            // }
        }

        public bool LoadConfiguration()
        {
            // if (File.Exists(ConfigurationFileName))
            // {
            //     var json = File.ReadAllText(ConfigurationFileName);
            //     try
            //     {
            //         Configuration = JsonSerializer.Deserialize<ConfigurationModel>(json, _serializerOptions);
            //         OnConfigurationModified(Configuration);
            //         return true;
            //     }
            //     catch (Exception e)
            //     {
            //         // TODO: Log to S&box logger
            //         Console.WriteLine($"Failed to load configuration file: {e.Message}");
            //         return false;
            //     }
            // }
            
            // TODO: Ditto
            Console.WriteLine($"Could not find {ConfigurationFileName}, resetting to defaults.");
            Configuration = new ConfigurationModel();
            SaveConfiguration();

            return true;
        }

        public void SaveConfiguration()
        {
            var json = JsonSerializer.Serialize(Configuration, _serializerOptions);
            
            // File.WriteAllText(ConfigurationFileName, json);
        }
    }
}
