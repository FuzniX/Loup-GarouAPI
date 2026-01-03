using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Data;
using Logic.LG;

namespace Logic.Services;

public class GameService(IServiceScopeFactory scopeFactory)
{
    private readonly ConcurrentDictionary<string, Game> _games = new();

    public string CreateGame(GameCreationRequest request)
    {
        var gameId = Guid.NewGuid().ToString();

        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();
        var roleFactory = scope.ServiceProvider.GetRequiredService<RoleFactoryService>();

        var group = dbContext.Groups.FirstOrDefault(g => g.Id == request.Group);
        if (group == null) throw new KeyNotFoundException("Group not found.");

        var composition = dbContext.Compositions.FirstOrDefault(c => c.Id == request.Composition);
        if (composition == null) throw new KeyNotFoundException("Composition not found.");

        var game = new Game(roleFactory, group, composition);
        _games.TryAdd(gameId, game);

        return gameId;
    }

    public GameMasterResponse Next(string gameId, GameMasterRequest request)
    {
        var response = _games.TryGetValue(gameId, out var game) ?
            game.PlayTurn(request) :
            throw new KeyNotFoundException($"Game with id {gameId} not found.");
        
        if (game.Over) _games.Remove(gameId, out _);
        
        return response;
    }
}

public record GameCreationRequest(int Group, int Composition);
public record GameMasterRequest(string Phase, string Action, string? Target);
public record GameMasterResponse(string Message, string Phase, List<Button>? Buttons, List<string>? Candidates);