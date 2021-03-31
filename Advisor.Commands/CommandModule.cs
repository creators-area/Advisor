namespace Overwatch.Commands
{
    /// <summary>
    /// The base class for any module meant to create Overwatch commands.
    /// </summary>
    public class CommandModule
    {
        /// <summary>
        /// The category commands should be under if any (can be null!)
        /// </summary>
        public string Category { get; internal set; }
    }
}