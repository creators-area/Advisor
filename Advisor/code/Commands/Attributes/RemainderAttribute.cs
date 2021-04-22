using System;

namespace Advisor.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class RemainderAttribute : Attribute
    {
    }
}