using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Services;

public class GroupService(IServiceScopeFactory scopeFactory)
{
    public void CreateGroup(GroupCreationRequest request)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();

        var group = new Group
        {
            Name = request.Name,
            Description = request.Description
        };

        request.Players.ForEach(playerString =>
        {
            var player = dbContext.Players.FirstOrDefault(p => p.Name == playerString);
            if (player is null) throw new KeyNotFoundException($"Player {playerString} not found");
            group.Players.Add(player);
        });

        dbContext.Groups.Add(group);
        dbContext.SaveChanges();
    }

    public GroupGetResponse GetGroup(int id)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();

        var group = dbContext.Groups.Include(group => group.Players).FirstOrDefault(g => g.Id == id);
        return group is null ?
            throw new KeyNotFoundException($"Group {id} not found") :
            new GroupGetResponse(group.Id, group.Name, group.Description, group.Players.Select(r => r.Name).ToList());
    }

    public List<GroupGetResponse> GetGroups()
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();
        
        var groups = dbContext.Groups.Include(group => group.Players).ToList();
        return groups
            .Select(group => new GroupGetResponse(group.Id, group.Name, group.Description, group.Players.Select(r => r.Name).ToList()))
            .ToList();
    }
}

public record GroupGetResponse(int Id, string Name, string Description, List<string> Players);

public record GroupCreationRequest(string Name, string Description, List<string> Players);