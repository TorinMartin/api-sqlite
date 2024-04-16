using System.Reflection;

namespace MusicApi.Injection;

public static class ServiceInjector
{
    public static void InjectServices(IServiceCollection services)
    {
        var injectableTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => Attribute.IsDefined(type, typeof(InjectableAttribute)))
            .ToList();
        
        foreach (var type in injectableTypes)
        {
            if (Attribute.GetCustomAttribute(type, typeof(InjectableAttribute)) is not InjectableAttribute injectableAttribute) continue;
            injectableAttribute.Types ??= new[] { type.GetInterfaces().FirstOrDefault() ?? type };

            var addServiceDel = injectableAttribute.Lifetime switch
            {
                ServiceLifetime.Transient => new Func<Type, Type, IServiceCollection>(services.AddTransient),
                ServiceLifetime.Singleton => services.AddSingleton,
                ServiceLifetime.Scoped => services.AddScoped,
                _ => throw new Exception("Invalid service scope")
            };

            foreach (var serviceType in injectableAttribute.Types) addServiceDel(serviceType, type.IsGenericType ? type.MakeGenericType(serviceType.GetGenericArguments()) : type);
        }
    } 
}