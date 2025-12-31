using Data;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Services;

public class RoleService(IServiceScopeFactory scopeFactory)
{
    public void CreateRole(RoleCreationRequest request)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();

        var role = new Role
        {
            Name = request.Name,
            Description = request.Description,
            ImageURL = request.ImageUrl,
            DefaultPriority = request.DefaultPriority
        };

        dbContext.Roles.Add(role);
        dbContext.SaveChanges();
    }

    public RoleGetResponse GetRole(int id)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();

        var role = dbContext.Roles.FirstOrDefault(p => p.Id == id);
        return role is null ?
            throw new KeyNotFoundException($"Role {id} not found") :
            new RoleGetResponse(role.Id, role.Name, role.Description, role.ImageURL, role.DefaultPriority);
    }

    public List<RoleGetResponse> GetAllRoles()
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();
        
        return dbContext.Roles
            .Select(role => new RoleGetResponse(role.Id, role.Name, role.Description, role.ImageURL, role.DefaultPriority))
            .ToList();
    }
}

public record RoleGetResponse(int Id, string Name, string Description, string ImageUrl, int DefaultPriority);
public record RoleCreationRequest(string Name, string Description, string ImageUrl, int DefaultPriority);