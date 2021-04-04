using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Advisor.Commands.Attributes;
using Advisor.Commands.Converters;
using Advisor.Commands.Entities;

namespace Advisor.Commands
{
    public class CommandHandler
    {
        private readonly AdvisorAddon _advisor;
        private List<Command> _commands;
        private Dictionary<Type, CommandModule> _loadedModules;
        private Dictionary<Type, IArgumentConverter> _converters;
        
        public CommandHandler(AdvisorAddon advisor)
        {
            _advisor = advisor;
            _commands = new List<Command>();
            _loadedModules = new Dictionary<Type, CommandModule>();
            _converters = new Dictionary<Type, IArgumentConverter>();
        }

        /// <summary>
        /// Registers all argument converters that exist in the given assembly.
        /// </summary>
        /// <param name="assembly"> The assembly to instantiate the converters from. </param>
        /// <exception cref="ArgumentNullException"> Thrown if the given assembly is null. </exception>
        public void RegisterArgumentConverters(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var converters = assembly.GetTypes()
                .Where(t => typeof(IArgumentConverter).IsAssignableFrom(t))
                .ToList();

            foreach (var type in converters)
            {
                RegisterArgumentConverter(type);
            }
        }

        /// <summary>
        /// Register an argument converter.
        /// </summary>
        /// <param name="type"> The type of the argument converter to register. </param>
        /// <exception cref="ArgumentNullException"> The given type is null. </exception>
        /// <exception cref="InvalidOperationException"> The given type does not implement IArgumentConverter. </exception>
        /// <exception cref="NullReferenceException"> Failed to instantiate argument converter, or the converter returned a null converted type. </exception>
        public void RegisterArgumentConverter(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(IArgumentConverter).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("Type must implement IArgumentConverter.");
            }
            
            IArgumentConverter converter = Activator.CreateInstance(type) as IArgumentConverter;

            if (converter == null)
            {
                throw new NullReferenceException($"Could not instantiate argument converter of type '{type.Name}'.");
            }

            var convertedType = converter.GetConvertedType();
            if (convertedType == null)
            {
                throw new NullReferenceException($"Converter '{type.Name}' returned null converted type.");
            }

            if (_converters.ContainsKey(convertedType))
            {
                // TODO: Warning log, attempted to register converter but one already exists for converted type.
                return;
            }

            Console.WriteLine($"Advisor: Register argument converter '{type.Name}' for type '{converter.GetConvertedType().Name}'");
            _converters.Add(convertedType, converter);
        }
        
        /// <summary>
        /// Register all the command modules in a given assembly.
        /// </summary>
        /// <param name="assembly"> The assembly to load all command modules from. </param>
        /// <exception cref="NullReferenceException"> Thrown if the given assembly is null. </exception>
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
        
        /// <summary>
        /// Register a command module from its type.
        /// </summary>
        /// <param name="type"> The type of command module to register. </param>
        /// <exception cref="NullReferenceException"> Thrown if the given type is null, or we failed to instantiate the command module. </exception>
        /// <exception cref="ArgumentException"> Throw if the type does not derive from <see cref="CommandModule"/>, or isn't public, non generic and non nested. </exception>
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