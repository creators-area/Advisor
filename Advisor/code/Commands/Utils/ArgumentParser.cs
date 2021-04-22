using Advisor.Commands.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advisor.Commands.Utils
{
    public static class ArgumentParser
    {
        #region OpenMod Source
        /// <summary>
        /// C-like argument parser
        /// Thanks to Trojaner for giving me this from https://github.com/openmod/openmod
        /// </summary>
        /// <param name="line">Line string with arguments.</param>
        /// <returns>The args[] array (argv)</returns>
        public static string[] ToStringArray(string line)
        {
            var argsBuilder = new StringBuilder(line);
            var args = new List<string>();

            var currentArg = new StringBuilder();

            var inQuote = false;
            var inApostrophes = false;
            var isEscaped = false;

            for (var i = 0; i < argsBuilder.Length; i++)
            {
                var currentChar = argsBuilder[i];
                if (isEscaped)
                {
                    currentArg.Append(currentChar);
                    isEscaped = false;
                    continue;
                }

                var nextIndex = i + 1;
                switch (currentChar)
                {
                    case '\\':
                        isEscaped = true;
                        break;
                    case '\'':
                        if (!inQuote && (currentArg.Length == 0 || argsBuilder.Length == nextIndex || IsSpace(argsBuilder[nextIndex])))
                        {
                            inApostrophes = !inApostrophes;
                        }
                        else
                        {
                            currentArg.Append(currentChar);
                        }
                        break;
                    case '"':
                        if (!inApostrophes && (currentArg.Length == 0 || argsBuilder.Length == nextIndex || IsSpace(argsBuilder[nextIndex])))
                        {
                            inQuote = !inQuote;
                        }
                        else
                        {
                            currentArg.Append(currentChar);
                        }
                        break;
                    default:
                        if (IsSpace(currentChar))
                        {
                            if (inQuote || inApostrophes)
                            {
                                currentArg.Append(currentChar);
                            }
                            else if (currentArg.Length != 0)
                            {
                                args.Add(currentArg.ToString());
                                currentArg.Clear();
                            }
                            break;
                        }
                        currentArg.Append(currentChar);
                        break;
                }
            }

            if (inQuote || inApostrophes) //command: 'command "player    ' -> args: 'command', 'player'
            {
                currentArg = TrimEnd(currentArg);
            }

            args.Add(currentArg.ToString());
            return args.ToArray();
        }
        
        private static StringBuilder TrimEnd(StringBuilder currentArg)
        {
            var length = 0;
            for (var i = currentArg.Length - 1; 0 <= i; i--)
            {
                if (!IsSpace(currentArg[i]))
                {
                    if (length != 0)
                        currentArg.Remove(i + 1, length);

                    break;
                }

                length++;
            }

            return currentArg;
        }

        private static bool IsSpace(char character)
        {
            return char.IsWhiteSpace(character) || character == char.MinValue;
        }
        #endregion // OpenMod Source

        /// <summary>
        /// Parse raw arguments into an object[] for the given command arguments.
        /// </summary>
        /// <param name="arguments"> The command arguments to parse the raw arguments for. </param>
        /// <param name="rawArguments">The raw arguments to parse.</param>
        /// <returns> An array of parsed and converted arguments for method invocation. </returns>
        public static ArgumentParserResult Parse(IReadOnlyList<CommandArgument> arguments, string[] rawArguments)
        {
            var current = 0;
            var objects = new object[arguments.Count];

            foreach (var arg in arguments)
            {
                if (rawArguments.Length <= current)
                {
                    // If it has a default value, we can just grab whatever it has.
                    if (arg.Parameter.HasDefaultValue)
                    {
                        objects[objects.Length] = arg.Parameter.RawDefaultValue;
                    }
                    else
                    {
                        return ArgumentParserResult.FromFailure("Not enough arguments specified for the given command arguments.");
                    }
                }
                else
                {
                    var rawArg = rawArguments[current];
                
                    // Read the remaining arguments into a string and parse.
                    if (arg.Remainder)
                    {
                        var remainder = rawArguments
                            .Skip(current)
                            .Take(rawArguments.Length - current)
                            .Aggregate((curr, next) => $"{curr} {next}");

                        var result = arg.Converter.ConvertArgument(remainder);
                        if (!result.IsSuccessful)
                        {
                            return ArgumentParserResult.FromFailure($"Failed to parse '{remainder}' into '{arg.Parameter.Name}' (expected: {arg.Converter.GetFriendlyTypeName()}).");
                        }

                        objects[objects.Length] = result.Result;
                        return ArgumentParserResult.FromSuccess(objects.ToArray());
                    }                    
                    
                    // Loop through the remaining raw arguments and add them to an array.
                    if (arg.IsParams)
                    {
                        var paramsObjects = new object[rawArguments.Length - current];
                        for (int i = current; i < rawArguments.Length; i++)
                        {
                            var raw = rawArguments[i];
                            var result = arg.Converter.ConvertArgument(raw);
                            if (!result.IsSuccessful)
                            {
                                return ArgumentParserResult.FromFailure($"Failed to parse '{raw}' into '{arg.Parameter.Name}' (expected: {arg.Converter.GetFriendlyTypeName()}).");
                            }

                            paramsObjects[paramsObjects.Length] = result.Result;
                        }
                        
                        objects[objects.Length] = paramsObjects;
                        return ArgumentParserResult.FromSuccess(objects.ToArray());
                    }
                    
                    // Just read this object in this case.
                    var converted = arg.Converter.ConvertArgument(rawArg);
                    if (!converted.IsSuccessful)
                    {
                        return ArgumentParserResult.FromFailure($"Failed to parse '{rawArg}' into '{arg.Parameter.Name}' (expected: {arg.Converter.GetFriendlyTypeName()}).");
                    }

                    objects[objects.Length] = converted.Result;
                }

                current++;
            }
            
            return ArgumentParserResult.FromSuccess(objects);
        }
    }
}