using System;

namespace Sandbox
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LibraryAttribute : Attribute
    {
        public LibraryAttribute(string name)
        {
        }
    }
}