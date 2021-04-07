using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

            Console.WriteLine($"Advisor: Register argument converter '{type.Name}' for type '{converter.GetConvertedType().Name}'");
            _converters.Add(convertedType, converter);
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

            var module = Activator.CreateInstance(type) as CommandModule;
            if (module == null)
            {
                throw new InvalidOperationException($"Failed to instantiate command module type {type.Name}.");
            }
            
            _loadedModules.Add(type, module);

            // Register every declared method that fits a command's signature (Context + other arguments).
            // Right now we'll grab all methods with the CommandAttribute and throw if any is wrong.
            // That'll let the developer know they need to fix their stuff.
            var commandMethods = typeInfo.DeclaredMethods
                .Where(m => m.GetCustomAttribute<CommandAttribute>() != null)
                .ToList();

            foreach (var command in commandMethods)
            {
                RegisterCommand(module, command);
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
            if (string.IsNullOrWhiteSpace(commandAttr.Name))
            {
                throw new ArgumentException("Command name cannot be null or whitespace.", nameof(commandAttr.Name));
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
                        ArgumentType = param.ParameterType,
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
                    case HiddenAttribute hidden: cmd.IsHidden = true;
                        break;
                }
            }

            // Dynamically generate the delegate for the command's method.
            var paramTypes = info.GetParameters()
                .Select(p => p.ParameterType)
                .Concat(new[] { info.ReturnType })
                .ToArray();
            
            if (commandArguments.Count > 1)
            {
                // Not as fast as a known delegate signature, and requires executing the command with DynamicInvoke().
                // TODO: Benchmark this.
                var delType = Expression.GetDelegateType(paramTypes);
                cmd.MethodDelegate = Delegate.CreateDelegate(delType, module, info);   
            }
            else
            {
                // No arguments means we know exactly the type of the delegate and can build it that way.
                // Faster to execute than DynamicInvoke on an arbitrary delegate.
                cmd.MethodDelegateNoParams = info.CreateDelegate<Action<CommandContext>>(module);
            }
            
            // TODO: Maybe add a CommandContext, TargetPlayer overload? If the performance is worth it over dyn invoke.
            
            // Check if any command in the module have the same name and add it as an overload if so.
            var existingCommand = module.Commands
                .FirstOrDefault(c => c.Name.Equals(cmd.Name, StringComparison.InvariantCultureIgnoreCase));

            if (existingCommand != null)
            {
                existingCommand.Overloads.Add(existingCommand);
            }
            else
            {
                Console.WriteLine($"Registered command {cmd.Name} with {cmd.Arguments.Count} arguments.");
                var cmds = module.Commands.ToList();
                cmds.Add(cmd);
                module.Commands = cmds;
            }
        }
    }
}