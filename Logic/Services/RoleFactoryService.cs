using System.Reflection;
using Data;
using Logic.LG;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Services;

public class RoleFactoryService
{
    private readonly Dictionary<string, Role> _definitions;
    private readonly Dictionary<string, Type> _types;

    public RoleFactoryService(IServiceScopeFactory scopeFactory)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LgDbContext>();
        
        _definitions = context.Roles.ToDictionary(r => r.Name, r => r);
        _types = new Dictionary<string, Type>();
        
        var roleTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<RoleIdentifierAttribute>() != null);

        foreach (var type in roleTypes)
        {
            var attr = type.GetCustomAttribute<RoleIdentifierAttribute>()!;
            _types.Add(attr.RoleName, type);
        }
    }

    public GameRole New(string roleName)
    {
        if (_types.TryGetValue(roleName, out var type))
            return (GameRole) Activator.CreateInstance(type, _definitions[roleName])!;
        throw new KeyNotFoundException($"No role {roleName} found.");
    }
}