namespace Advisor.Enums
{
    public enum SandboxRealm
    {
        /// <summary>
        /// Runs the command on the calling client, after verifying the user has access to it serverside.
        /// </summary>
        Client,
        
        /// <summary>
        /// Runs the command on the server.
        /// </summary>
        Server,
        
        /// <summary>
        /// Runs the command on both the calling client and the server.
        /// </summary>
        Shared,
    }
}