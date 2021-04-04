using System.Collections.Generic;
using System.Reflection;

namespace Advisor.Commands.Entities
{
    /// <summary>
    /// Represents a command.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Returns the name of this command.
        /// Set using the <see cref="Advisor.Commands.Attributes.CommandAttribute"/>.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Returns the possible aliases of this command.
        /// Set using the <see cref="Advisor.Commands.Attributes.AliasAttribute"/>.
        /// </summary>
        public IReadOnlyList<string> Aliases { get; internal set; }
        
        /// <summary>
        /// Returns the description of this command.
        /// Set using the <see cref="Advisor.Commands.Attributes.DescriptionAttribute"/>.
        /// </summary>
        public string Description { get; internal set; }
        
        /// <summary>
        /// The module this command came from.
        /// </summary>
        public CommandModule ParentModule { get; internal set; }

        /// <summary>
        /// The category this command belongs to, set in the ParentModule.
        /// If none is set (or an invalid one is set), this command will be under 'Uncategorized'
        /// </summary>
        public string Category => ParentModule?.Category ?? "Uncategorized";
    }
}