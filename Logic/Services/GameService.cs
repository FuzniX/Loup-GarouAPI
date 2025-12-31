using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Data;
using Logic.Games;

namespace Logic.Services;

public class GameService(IServiceScopeFactory scopeFactory)
{
    private readonly ConcurrentDictionary<string, Game> _games = new();

    public string CreateGame(GameCreationRequest request)
    {
        var gameId = Guid.NewGuid().ToString();

        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LgDbContext>();
        
        var group = dbContext.Groups.FirstOrDefault(g => g.Id == request.Group);
        if (group == null) throw new KeyNotFoundException("Group not found.");

        var composition = dbContext.Compositions.FirstOrDefault(c => c.Id == request.Composition);
        if (composition == null) throw new KeyNotFoundException("Composition not found.");

        var game = new Game(group, composition);
        _games.TryAdd(gameId, game);

        Console.WriteLine(game);

        return gameId;
    }

    public GameMasterResponse Next(string gameId, GameMasterRequest request)
    {
        if (!_games.TryGetValue(gameId, out var gameLogic))
            throw new KeyNotFoundException($"Game with id {gameId} not found.");

        // Example
        return new GameMasterResponse
        (
            Message: "Test",
            Buttons: [new Button { Label = "Next", Action = Action.Next.ToCode(), Color = "Blue" }]
        );
    }
}

public record GameCreationRequest(int Group, int Composition);
public record GameMasterRequest(string Action, string? Target);
public record GameMasterResponse(string Message, List<Button> Buttons);
