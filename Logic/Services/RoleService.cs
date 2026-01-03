using Data;
using Logic.LG;
using Microsoft.EntityFrameworkCore;
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
            ImageUrl = request.ImageUrl,
            DefaultPriority = request.DefaultPriority,
            Camp = Enum.Parse<Camp>(request.Camp),
            Phase = Enum.Parse<Phase>(request.Phase)
        };

        dbContext.Roles.Add(role);
        dbContext.SaveChanges();
    }

    public RoleGetResponse GetRole(int id)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();

        var role = dbContext.Roles
            .Include(role => role.Camp)
            .Include(role => role.Phase)
            .FirstOrDefault(p => p.Id == id);
        return role is null ?
            throw new KeyNotFoundException($"Role {id} not found") :
            new RoleGetResponse(role.Id, role.Name, role.Description, role.ImageUrl, role.DefaultPriority, role.Camp.ToString(), role.Phase.ToString());
    }

    public List<RoleGetResponse> GetAllRoles()
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();
        
        return dbContext.Roles
            .Select(role => new RoleGetResponse(role.Id, role.Name, role.Description, role.ImageUrl, role.DefaultPriority, role.Camp.ToString(), role.Phase.ToString()))
            .ToList();
    }
}

public record RoleGetResponse(int Id, string Name, string Description, string ImageUrl, int DefaultPriority, string Camp, string Phase);
public record RoleCreationRequest(string Name, string Description, string ImageUrl, int DefaultPriority, string Camp, string Phase);