using System;

namespace Advisor.Commands.Utils
{
    /// <summary>
    /// Logging utility, wraps around s&box log to make them clear.
    /// TODO: Replace Console.WriteLine() with s&box log functions.
    /// </summary>
    public static class AdvisorLog
    {
        /// <summary>
        /// Outputs an informative message to the console.
        /// </summary>
        /// <param name="message"> The information to output. </param>
        public static void Info(string message)
        {
            Console.WriteLine($"[Advisor]: {message}");
        }

        /// <summary>
        /// Outputs a warning to the console.
        /// </summary>
        /// <param name="message"> The warning to output. </param>
        public static void Warning(string message)
        {
            Console.WriteLine($"[Advisor]: {message}");
        }

        /// <summary>
        /// Outputs an error to the console with a related exception.
        /// </summary>
        /// <param name="exception"> The exception related to the error. </param>
        /// <param name="message"> The error to output. </param>
        public static void Error(Exception exception, string message)
        {
            Console.WriteLine($"[Advisor]: {message}");
            Console.WriteLine(exception.StackTrace);
        }
        
        /// <summary>
        /// Outputs an error to the console.
        /// </summary>
        /// <param name="message"> The error to output. </param>
        public static void Error(string message)
        {
            Console.WriteLine($"[Advisor]: {message}");
        }

        /// <summary>
        /// Outputs a debug message to the console.
        /// </summary>
        /// <param name="message"> The debug message to output. </param>
        public static void Debug(string message)
        {
            Console.WriteLine($"[Advisor] [DEBUG]: {message}");
        }
    }
}