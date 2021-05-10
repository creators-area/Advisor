using System.Text.Json.Serialization;

namespace Advisor.Configuration
{
    /// <summary>
    /// Configuration model for Advisor.
    /// </summary>
    public class ConfigurationModel
    {
        /// <summary>
        /// The prefix used for command execution.
        /// </summary>
        [JsonInclude]
        public string Prefix { get; internal set; } = "!";

        /// <summary>
        /// Whether or not the command prefix is case sensitive.
        /// </summary>
        [JsonInclude]
        public bool CaseSensitivePrefix { get; internal set; } = false;

        /// <summary>
        /// Whether or not commands are case sensitive.
        /// </summary>
        [JsonInclude]
        public bool CaseSensitiveCommands { get; internal set; } = false;
    }
}