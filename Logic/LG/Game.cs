using Data;
using Logic.Services;

namespace Logic.LG;

public class Game
{
    private readonly Group _group;
    private readonly Composition _composition;
    
    private readonly List<GamePlayer> _players = [];
    
    public Game(RoleFactoryService roleFactoryService, Group group, Composition composition)
    {
        _group = group;
        _composition = composition;

        if (_group.Players.Count != _composition.Roles.Count) 
            throw new ArgumentException("Should instanciate the same amount of players and roles in the group and composition");
        
        var groupPlayers = _group.Players.Shuffle();
        var compositionRoles = _composition.Roles.Shuffle();

        foreach (var player in groupPlayers.Zip(compositionRoles))
            _players.Add(new GamePlayer(player.First.Name, roleFactoryService.New(player.Second.Name)));
    }
    
    public override string ToString() => string.Join(", ", _players);
}