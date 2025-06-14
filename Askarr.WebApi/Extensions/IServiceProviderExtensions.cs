using System;

namespace  Askarr.WebApi.Extensions
{
    public static class IServiceProviderExtensions
    {
        public static T Get<T>(this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }
    }
}