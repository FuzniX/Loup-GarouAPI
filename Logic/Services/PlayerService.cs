using Data;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Services;

public class PlayerService(IServiceScopeFactory scopeFactory)
{
    public void CreatePlayer(PlayerCreationRequest request)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();

        var player = new Player { Name = request.Name };

        dbContext.Players.Add(player);
        dbContext.SaveChanges();
    }

    public PlayerGetResponse GetPlayer(int id)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();

        var player = dbContext.Players.FirstOrDefault(p => p.Id == id);
        return player is null ?
            throw new KeyNotFoundException($"Player {id} not found") :
            new PlayerGetResponse(player.Id, player.Name);
    }

    public List<PlayerGetResponse> GetAllPlayers()
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();
    
        return dbContext.Players
            .Select(player => new PlayerGetResponse(player.Id, player.Name))
            .ToList();
    }
}

public record PlayerGetResponse(int Id, string Name);
public record PlayerCreationRequest(string Name);