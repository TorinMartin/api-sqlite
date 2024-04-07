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
        
        //TODO: I can prob DRY this out a bit
        foreach (var type in injectableTypes)
        {
            if (Attribute.GetCustomAttribute(type, typeof(InjectableAttribute)) is not InjectableAttribute injectableAttribute) continue;
            
            injectableAttribute.Types ??= new[] { type.GetInterfaces().FirstOrDefault() ?? type };
            switch (injectableAttribute.Lifetime)
            {
                case ServiceLifetime.Transient:
                    foreach (var t in injectableAttribute.Types) services.AddTransient(t, type.IsGenericType ? type.MakeGenericType(t.GetGenericArguments()) : type);
                    break;
                case ServiceLifetime.Singleton:
                    foreach (var t in injectableAttribute.Types) services.AddSingleton(t, type.IsGenericType ? type.MakeGenericType(t.GetGenericArguments()) : type);
                    break;
                case ServiceLifetime.Scoped:
                    foreach (var t in injectableAttribute.Types) services.AddScoped(t, type.IsGenericType ? type.MakeGenericType(t.GetGenericArguments()) : type);
                    break;
                default:
                    throw new Exception("Invalid service scope");
            }
        }
    } 
}