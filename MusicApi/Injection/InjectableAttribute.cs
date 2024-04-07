namespace MusicApi.Injection;

[AttributeUsage(AttributeTargets.Class)]
public class InjectableAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }
    public Type[]? Types { get; set; }

    public InjectableAttribute(ServiceLifetime lifetime, params Type[] types)
    {
        Lifetime = lifetime;
        Types = types;
    }
}