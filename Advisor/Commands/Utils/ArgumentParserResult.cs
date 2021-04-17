namespace Advisor.Commands.Utils
{
    public readonly struct ArgumentParserResult
    {
        /// <summary>
        /// Whether or not parsing the raw arguments for the given command arguments succeeded.
        /// </summary>
        public bool IsSuccessful { get; }
        
        /// <summary>
        /// If not successful, the reason why parsing failed.
        /// </summary>
        public string FailureReason { get; }
        
        /// <summary>
        /// The arguments that were parsed if any.
        /// </summary>
        public object[] Arguments { get; }

        internal ArgumentParserResult(object[] args)
        {
            IsSuccessful = true;
            FailureReason = null;
            Arguments = args;
        }

        internal ArgumentParserResult(string failureReason)
        {
            IsSuccessful = false;
            FailureReason = failureReason;
            Arguments = null;
        }
        
        public static ArgumentParserResult FromSuccess(object[] args)
        {
            return new ArgumentParserResult(args);
        }

        public static ArgumentParserResult FromFailure(string reason)
        {
            return new ArgumentParserResult(reason);
        }
    }
}