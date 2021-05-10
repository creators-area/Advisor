namespace Advisor.Enums
{
    /// <summary>
    /// Allows additional checks for commands that target another user/multiple users.
    /// TODO: Move to Permissions namespace.
    /// </summary>
    public enum TargetPermission
    {
        /// <summary>
        /// This command can only be ran on a user with a lower permission level than yours.
        /// </summary>
        LowerPermissionLevel,
        
        /// <summary>
        /// This command can be ran on a user with a lower or equal permission level than yours.
        /// </summary>
        SamePermissionLevel,
        
        /// <summary>
        /// This command can target all users regardless of their permission level.
        /// </summary>
        All,
    }
}