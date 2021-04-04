using System;

namespace Advisor.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CategoryAttribute : Attribute
    {
        /// <summary>
        /// The display name of the category.
        /// </summary>
        public string Name { get; }

        public CategoryAttribute(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                Name = "Uncategorized";
            }
            else
            {
                Name = displayName.Trim();
            }
        }
    }
}