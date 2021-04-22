using System.ComponentModel.Design;

namespace Advisor.Extensions
{
    public static class ServiceExtensions
    {
        public static T GetRequiredService<T>(this ServiceContainer container)
        {
            return (T)container.GetService(typeof(T));
        }
    }
}