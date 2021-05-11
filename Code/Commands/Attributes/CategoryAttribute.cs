using System;
using System.Text.RegularExpressions;
using Sandbox;

namespace Advisor.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CategoryAttribute : LibraryAttribute
    {
        /// <summary>
        /// The display name of the category.
        /// </summary>
        public new string Name { get; }
        
        /// <summary>
        /// The name used to create permissions from commands in this module.
        /// Must only contain lowercase alphanumerical characters, as well as hyphens and underscores.
        /// </summary>
        public string PermissionName { get; }
        
        /// <summary>
        /// The prefix that will be applied to commands within this module, if any.
        /// I.e. if the prefix is 'permissions' and the command is 'list', the command will be 'permissions list'.
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// Whether or not the given permission isn't null, whitespace, and only contains lowercase alphanumeric text.
        /// Also allows for hyphens and underscores.
        /// </summary>
        /// <returns></returns>
        public bool HasValidPermissionName()
        {
            var regex = new Regex("^[a-z0-9_-]+$");
            return !string.IsNullOrWhiteSpace(PermissionName) && regex.IsMatch(PermissionName);
        }

        /// <summary>
        /// Whether or not this module's prefix, if any, is only composed of alphanumerical characters, hyphens and underscores.
        /// </summary>
        public bool HasValidPrefix()
        {
            if (Prefix != null)
            {
                return new Regex("^[a-zA-Z0-9_-]+$").IsMatch(Prefix);
            }

            return true;
        }

        public CategoryAttribute(string displayName, string permissionName, string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                Name = "Uncategorized";
            }
            else
            {
                Name = displayName.Trim();
            }

            PermissionName = permissionName;
            Prefix = prefix;
        }
    }
}
