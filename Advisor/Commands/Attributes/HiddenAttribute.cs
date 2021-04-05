using System;

namespace Advisor.Commands.Attributes
{
    /// <summary>
    /// Denotes that the command should be hidden from autocomplete/user facing UI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HiddenAttribute : Attribute
    {
        
    }
}