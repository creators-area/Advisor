using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Advisor.Commands.Attributes;
using Advisor.Commands.Entities;

namespace Advisor.Commands
{
    public class CommandHandler
    {
        private readonly AdvisorAddon _advisor;
        private List<Command> _commands;
        private Dictionary<Type, CommandModule> _loadedModules;
        
        public CommandHandler(AdvisorAddon advisor)
        {
            _advisor = advisor;
            _commands = new List<Command>();
            _loadedModules = new Dictionary<Type, CommandModule>();
        }
        
        public void RegisterCommandModules(Assembly assembly)
        {
            // Retrieve all classes that derive from CommandModule from the given assembly.
            if (assembly == null)
            {
                throw new NullReferenceException(nameof(assembly));
            }

            var types = assembly.GetTypes()
                .Where(t =>
                {
                    var typeInfo = t.GetTypeInfo();
                    return !typeInfo.IsNested && typeInfo.IsPublic 
                        && !typeInfo.IsGenericType && t.IsSubclassOf(typeof(CommandModule));
                })
                .ToList();

            foreach (var commandModule in types)
            {
                RegisterCommandModule(commandModule);
            }
        }

        public void RegisterCommandModule(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException(nameof(type));
            }

            if (!type.IsSubclassOf(typeof(CommandModule)))
            {
                throw new ArgumentException("Type must derive from CommandModule.");
            }
            
            if (_loadedModules.ContainsKey(type))
            {
                return;
            }

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsNested || !typeInfo.IsPublic || typeInfo.IsGenericType)
            {
                throw new ArgumentException("Type must be public, non generic and not nested.");
            }

            var module = Activator.CreateInstance(type) as CommandModule;
            if (module == null)
            {
                throw new NullReferenceException($"Failed to instantiate command module type {type.Name}.");
            }
            
            _loadedModules.Add(type, module);
            
            // Register every declared method that fits a command's signature (Context + other arguments).
        }
    }
}