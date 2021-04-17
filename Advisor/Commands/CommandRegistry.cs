﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Advisor.Commands.Attributes;
using Advisor.Commands.Converters;
using Advisor.Commands.Entities;

namespace Advisor.Commands
{
    /// <summary>
    /// Handles the registration of commands and argument converters.
    /// </summary>
    public class CommandRegistry
    {
        private readonly AdvisorAddon _advisor;
        private Dictionary<Type, CommandModule> _loadedModules;
        private Dictionary<Type, IArgumentConverter> _converters;

        // Commands that aren't executed using a category prefix.
        private Dictionary<string, Command> _rootCommands;
        // Commands that are executed using a category prefix.
        private Dictionary<string, Dictionary<string, Command>> _categorizedCommands;

        internal CommandRegistry(AdvisorAddon advisor)
        {
            _advisor = advisor;
            _loadedModules = new Dictionary<Type, CommandModule>();
            _converters = new Dictionary<Type, IArgumentConverter>();

            _rootCommands = new Dictionary<string, Command>();
            _categorizedCommands = new Dictionary<string, Dictionary<string, Command>>();
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
                .Where(t => typeof(IArgumentConverter).IsAssignableFrom(t)
                    && !t.IsNested && t.IsPublic && !t.IsInterface)
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
        /// <exception cref="InvalidOperationException"> Failed to instantiate argument converter, or the converter returned a null converted type. </exception>
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
                throw new InvalidOperationException($"Could not instantiate argument converter of type '{type.Name}'.");
            }

            var convertedType = converter.GetConvertedType();
            if (convertedType == null)
            {
                throw new InvalidOperationException($"Converter '{type.Name}' returned null converted type.");
            }

            if (_converters.ContainsKey(convertedType))
            {
                // TODO: Warning log, attempted to register converter but one already exists for converted type.
                return;
            }

            Console.WriteLine($"Advisor: Registered argument converter '{type.Name}' for type '{converter.GetConvertedType().Name}'");
            _converters.Add(convertedType, converter);
        }

        /// <summary>
        /// Whether or not the given type has a registered argument converter.
        /// </summary>
        /// <param name="type"> The type to check against. </param>
        /// <returns> True if a converter exists for the type. </returns>
        public bool HasArgumentConverter(Type type) => _converters.ContainsKey(type);

        /// <summary>
        /// Return an argument converter for the given type, if any.
        /// </summary>
        /// <param name="type"> The type that the argument converter is assigned to. </param>
        /// <returns> The found argument converter. </returns>
        /// <exception cref="ArgumentException"> Thrown if the given type has no argument converter. </exception>
        public IArgumentConverter GetArgumentConverter(Type type)
        {
            if (!_converters.ContainsKey(type))
            {
                throw new ArgumentException($"Could not find any argument converter for type '{type.Name}'");
            }

            return _converters[type];
        }
        
        /// <summary>
        /// Register all the command modules in a given assembly.
        /// </summary>
        /// <param name="assembly"> The assembly to load all command modules from. </param>
        /// <exception cref="ArgumentNullException"> Thrown if the given assembly is null. </exception>
        public void RegisterCommandModules(Assembly assembly)
        {
            // Retrieve all classes that derive from CommandModule from the given assembly.
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var types = assembly.GetTypes()
                .Where(t =>
                {
                    var typeInfo = t.GetTypeInfo();
                    return !typeInfo.IsNested && typeInfo.IsPublic
                        && !typeInfo.IsGenericType && t.IsSubclassOf(typeof(CommandModule));
                });

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
                throw new ArgumentNullException(nameof(type));
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

            var categoryAttr = type.GetCustomAttribute<CategoryAttribute>();
            if (categoryAttr == null)
            {
                throw new InvalidOperationException("Command modules must have a CategoryAttribute.");
            }

            // Check that the command module's permission name is valid.
            if (!categoryAttr.HasValidPermissionName())
            {
                throw new ArgumentException("Command module permission names can only consist of lowercase characters a-z, 0-9, -, _");
            }
            
            // Check that the command module's prefix is valid.
            if (!categoryAttr.HasValidPrefix())
            {
                throw new ArgumentException("Command module prefix can only consist of characters A-Z, 0-9, -, _");
            }
            
            if (Activator.CreateInstance(type) is not CommandModule module)
            {
                throw new InvalidOperationException($"Failed to instantiate command module type {type.Name}.");
            }

            module.Category = categoryAttr.Name;
            module.PermissionName = categoryAttr.PermissionName;
            module.Prefix = categoryAttr.Prefix;

            _loadedModules.Add(type, module);

            // Register every declared method that fits a command's signature (Context + other arguments).
            // Right now we'll grab all methods with the CommandAttribute and throw if any is wrong.
            // That'll let the developer know they need to fix their stuff.
            var commandMethods = typeInfo.DeclaredMethods
                .Where(m => m.IsDefined(typeof(CommandAttribute)));

            foreach (var method in commandMethods)
            {
                RegisterCommand(module, method);
            }
        }

        private void RegisterCommand(CommandModule module, MethodInfo info)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            
            // Check that the given method is actually in the module.
            if (info.DeclaringType != module.GetType())
            {
                throw new InvalidOperationException(
                    "Cannot register a MethodInfo in a CommandModule that isn't its declaring type.");
            }
            
            // Check if the method has the command attribute. This should be the case here, but never trust others.
            var commandAttr = info.GetCustomAttribute<CommandAttribute>();
            if (commandAttr == null)
            {
                throw new InvalidOperationException($"Method {info.Name} has no CommandAttribute");
            }
            
            // Check that the command's name is valid.
            if (!commandAttr.HasValidCommandName())
            {
                throw new ArgumentException("Command name can only consist of characters A-Z, 0-9, -, _");
            }

            if (module.Prefix == null)
            {
                // Make sure that there's no existing command with that name.
                if (_rootCommands.ContainsKey(commandAttr.Name.ToLower()))
                {
                    // TODO: Change to console log in S&box.
                    throw new InvalidOperationException(
                        $"Cannot register command '{commandAttr.Name}' from module '{module.GetType().Name}' as another root command with that name exists.");
                }
            }
            else
            {
                if (_categorizedCommands.ContainsKey(module.Prefix))
                {
                    var dict = _categorizedCommands[module.Prefix];
                    if (dict.ContainsKey(commandAttr.Name.ToLower()))
                    {
                        // TODO: Change to console log in S&box.
                        throw new InvalidOperationException(
                            $"Cannot register command '{module.Prefix} {commandAttr.Name}' from module '{module.GetType().Name}' as another command with that name exists.");
                    }
                }
            }
            
            // Check that the return type is void.
            if (info.ReturnType != typeof(void))
            {
                throw new InvalidOperationException($"Command '{info.Name}' must return void.");
            }
            
            // Check that the first parameter is a CommandContext.
            var cmdParameters = info.GetParameters();
            if (cmdParameters.Length == 0 || cmdParameters[0].ParameterType != typeof(CommandContext))
            {
                throw new InvalidOperationException(
                    "Commands must always have at least one parameter, and start with a CommandContext.");
            }

            if (cmdParameters[0].IsOut || cmdParameters[0].IsIn)
            {
                throw new InvalidOperationException("Commands do not support in/out parameters.");
            }

            var commandArguments = new List<CommandArgument>();
            
            // Verify that we have argument converters for every single one of these types.
            if (cmdParameters.Length > 1)
            {
                for (int i = 1; i < cmdParameters.Length; i++)
                {
                    var param = cmdParameters[i];
                    
                    if (param.IsIn || param.IsOut)
                    {
                        throw new InvalidOperationException("Commands do not support in/out parameters.");
                    }
                    
                    var remainder = param.IsDefined(typeof(RemainderAttribute));
                    
                    // Only the last argument can be catch all.
                    if (remainder && i != cmdParameters.Length - 1)
                    {
                        throw new InvalidOperationException(
                            $"Only the last parameter of a command can have the RemainderAttribute (in command '{info.Name}' of module '{module.GetType().Name}')");
                    }

                    if (remainder && param.ParameterType != typeof(string))
                    {
                        throw new InvalidOperationException(
                            $"Only an argument of type string can have the RemainderAttribute (in command '{info.Name}' of module '{module.GetType().Name}')");
                    }
                    
                    var argType = param.ParameterType;
                    var isParams = false;
                    
                    if (param.IsDefined(typeof(ParamArrayAttribute)))
                    {
                        isParams = true;
                        argType = param.ParameterType.GetElementType();
                        if (argType == null)
                        {
                            throw new InvalidOperationException(
                                $"Command '{info.Name}' of module '{module.GetType().Name}' has a params argument with an invalid element type.");
                        }
                    }
                    
                    if (!_converters.ContainsKey(argType))
                    {
                        throw new InvalidOperationException(
                            $"Command '{info.Name}' of module '{module.GetType().Name}' has parameter '{param.Name ?? "null"}' with no IArgumentConverter registered for its type ({argType.Name})");
                    }

                    var arg = new CommandArgument
                    {
                        ArgumentType = argType,
                        Parameter = param,
                        Remainder = remainder,
                        IsParams = isParams,
                    };
                    
                    commandArguments.Add(arg);
                }
            }
            
            // Create the command object.
            var cmd = new Command
            {
                Arguments = commandArguments,
                ParentModule = module,
                Name = commandAttr.Name,
                ExecutionRealm = commandAttr.ExecutionRealm,
                Method = info,
            };
            
            cmd.FullName = !string.IsNullOrWhiteSpace(module.Prefix) 
                ? $"{module.Prefix} {cmd.Name}".ToLower() 
                : cmd.Name.ToLower();
            
            // Populate any additional attributes.
            foreach (var attr in info.GetCustomAttributes())
            {
                switch (attr)
                {
                    case DescriptionAttribute desc: cmd.Description = desc.Description;
                        break;
                    case AliasAttribute alias: cmd.Aliases = alias.Aliases;
                        break;
                    case TargetAttribute target: cmd.TargetPermissionLevel = target.TargetPermissionLevel;
                        break;
                    case HiddenAttribute: cmd.IsHidden = true;
                        break;
                }
            }
            
            // TODO: Maybe add a CommandContext, TargetPlayer overload? If the performance is worth it over dyn invoke.
            if (commandArguments.Count > 0)
            {
                // Dynamically generate the delegate for the command's method.
                var paramTypes = info.GetParameters()
                    .Select(p => p.ParameterType)
                    .Concat(new[] { info.ReturnType })
                    .ToArray();
                
                // Not as fast as a known delegate signature, and requires executing the command with DynamicInvoke().
                var delType = Expression.GetDelegateType(paramTypes);
                cmd.MethodDelegate = Delegate.CreateDelegate(delType, module, info);   
            }
            else
            {
                // No arguments means we know exactly the type of the delegate and can build it that way.
                // Faster to execute than DynamicInvoke on an arbitrary delegate.
                cmd.MethodDelegateNoParams = info.CreateDelegate<Action<CommandContext>>(module);
            }
            
            // TODO: Change to S&box log.
            Console.WriteLine($"Registered command {cmd.Name} with {cmd.Arguments.Count} arguments.");
            module.InternalCommands.Add(cmd);

            if (module.Prefix == null)
            {
                _rootCommands.Add(cmd.FullName.ToLower(), cmd);

                foreach (string alias in cmd.Aliases)
                {
                    var a = alias.ToLower();
                    if (!_rootCommands.ContainsKey(a))
                    {
                        _rootCommands.Add(a, cmd);
                    }
                    else
                    {
                        // TODO: Change to log in S&box.
                        throw new InvalidOperationException(
                            $"Command '{cmd.Name}' in module {module.GetType().Name}' has an alias '{alias}' that conflicts with another command!");
                    }
                }
            }
            else
            {
                var prefix = module.Prefix.ToLower();
                if (!_categorizedCommands.ContainsKey(prefix))
                {
                    _categorizedCommands.Add(prefix, new Dictionary<string, Command>());
                }
                
                _categorizedCommands[prefix].Add(cmd.Name.ToLower(), cmd);
                
                foreach (string alias in cmd.Aliases)
                {
                    var a = alias.ToLower();
                    if (!_categorizedCommands[prefix].ContainsKey(a))
                    {
                        _categorizedCommands[prefix].Add(a, cmd);
                    }
                    else
                    {
                        // TODO: Change to log in S&box.
                        throw new InvalidOperationException(
                            $"Command '{module.Prefix} {cmd.Name}' in module {module.GetType().Name}' has an alias '{alias}' that conflicts with another command!");
                    }
                }
            }
        }
    }
}