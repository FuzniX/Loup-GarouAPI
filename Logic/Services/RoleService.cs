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
            ImageUrl = request.ImageUrl,
            DefaultPriority = request.DefaultPriority,
            Camp = request.Camp,
            Phases = request.Phases
        };

        dbContext.Roles.Add(role);
        dbContext.SaveChanges();
    }

    public RoleGetResponse GetRole(int id)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();

        var role = dbContext.Roles
            .Include(role => role.Phases)
            .FirstOrDefault(p => p.Id == id);
        return role is null ?
            throw new KeyNotFoundException($"Role {id} not found") :
            new RoleGetResponse(
                role.Id,
                role.Name,
                role.ImageUrl,
                role.DefaultPriority,
                role.Camp,
                role.Phases);
    }

    public List<RoleGetResponse> GetAllRoles()
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();

        return dbContext.Roles
            .Select(role => new RoleGetResponse(role.Id, role.Name, role.ImageUrl, role.DefaultPriority, role.Camp, role.Phases))
            .ToList();
    }
}

public record RoleGetResponse(int Id, string Name, string ImageUrl, int DefaultPriority, Camp Camp, ICollection<RolePhase> Phases);

public record RoleCreationRequest(string Name, string ImageUrl, int DefaultPriority, Camp Camp, ICollection<RolePhase> Phases);