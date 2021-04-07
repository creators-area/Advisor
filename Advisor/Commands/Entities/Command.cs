using System;
using System.Collections.Generic;
using System.Reflection;
using Advisor.Enums;

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
        /// Whether or not this command should be visible by autocompletion and user facing UI.
        /// </summary>
        public bool IsHidden { get; internal set; }
        
        /// <summary>
        /// If set, the caller's permission level will be checked against the target(s) permission level(s) to see if they can be targeted.
        /// </summary>
        public TargetPermission? TargetPermissionLevel { get; internal set; }

        /// <summary>
        /// The realm this command should be executed on.
        /// </summary>
        public SandboxRealm ExecutionRealm { get; internal set; }
        
        /// <summary>
        /// The module this command came from.
        /// </summary>
        public CommandModule ParentModule { get; internal set; }

        /// <summary>
        /// The category this command belongs to, set in the ParentModule.
        /// If empty or invalid, this command will be under 'Uncategorized'
        /// </summary>
        public string Category => ParentModule?.Category ?? "Uncategorized";
        
        /// <summary>
        /// The arguments of this command.
        /// </summary>
        public IReadOnlyList<CommandArgument> Arguments { get; internal set; }

        /// <summary>
        /// Commands with the same name will be added as overloads to the first one that's loaded.
        /// </summary>
        internal List<Command> Overloads { get; set; }
        
        /// <summary>
        /// The method this command was created from.
        /// </summary>
        internal MethodInfo Method { get; set; }

        /// <summary>
        /// Dynamically generated delegate from the command's MethodInfo.
        /// </summary>
        internal Delegate MethodDelegate { get; set; }
        
        /// <summary>
        /// Dynamically generated delegate for commands with no more arguments than the CommandContext.
        /// Offers faster execution.
        /// </summary>
        internal Action<CommandContext> MethodDelegateNoParams { get; set; }

        internal void ExecuteCommand(object[] args)
        {
            if (args.Length == 0 || args[0] is not CommandContext)
            {
                throw new ArgumentException("Args must have a CommandContext as its first argument.");
            }
            
            if (MethodDelegate != null)
            {
                MethodDelegate.DynamicInvoke(args);
            }
            else if (MethodDelegateNoParams != null && args[0] is CommandContext ctx)
            {
                MethodDelegateNoParams(ctx);
            }
        }
    }
}