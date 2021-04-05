using System;
using Advisor.Enums;

namespace Advisor.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TargetAttribute : Attribute
    {
        /// <summary>
        /// The permission level the target(s) must have compared to yours for this command to be executed.
        /// </summary>
        public TargetPermission TargetPermissionLevel { get; }

        public TargetAttribute(TargetPermission permissionLevel)
        {
            TargetPermissionLevel = permissionLevel;
        }
    }
}