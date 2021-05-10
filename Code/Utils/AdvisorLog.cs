using System;
using Sandbox;

namespace Advisor.Utils
{
    /// <summary>
    /// Logging utility, wraps around s&box log to make them clear.
    /// TODO: Replace Console.WriteLine() with s&box log functions.
    /// </summary>
    public static class AdvisorLog
    {
	    private static Logger _log = new Logger( "Advisor" );
	    
        /// <summary>
        /// Outputs an informative message to the console.
        /// </summary>
        /// <param name="message"> The information to output. </param>
        public static void Info(string message)
        {
	        _log.Info(message);
        }

        /// <summary>
        /// Outputs a warning to the console.
        /// </summary>
        /// <param name="message"> The warning to output. </param>
        public static void Warning(string message)
        {
            _log.Warning(message);
        }

        /// <summary>
        /// Outputs an error to the console with a related exception.
        /// </summary>
        /// <param name="exception"> The exception related to the error. </param>
        /// <param name="message"> The error to output. </param>
        public static void Error(Exception exception, string message)
        {
	        _log.Error(exception, message);
        }
        
        /// <summary>
        /// Outputs an error to the console.
        /// </summary>
        /// <param name="message"> The error to output. </param>
        public static void Error(string message)
        {
            _log.Error(message);
        }

        /// <summary>
        /// Outputs a debug message to the console.
        /// </summary>
        /// <param name="message"> The debug message to output. </param>
        public static void Debug(string message)
        {
	        _log.Info($"[DEBUG]: {message}");
        }
    }
}
