using Data;
using Logic.Services;

namespace Logic.LG;

public class Game
{
    #region Public Attributes
    public bool Over => _currentPhase == Phase.Over;
    #endregion
    
    #region Private Attributes
    private readonly List<GamePlayer> _players = [];
    private readonly Dictionary<Phase, List<GamePlayer>> _order = new()
    {
        { Phase.RolesBeforeLg, [] },
        { Phase.RolesAfterLg, [] },
        { Phase.RolesBeforeVote, [] },
        { Phase.RolesAfterVote, [] }
    };
    private int _day = 1;
    #endregion
    
    #region Game Cursor
    private Phase _currentPhase = Phase.VillageSleeping;
    private int _playerIndex = -1;
    private GamePlayer? _currentPlayer;
    #endregion
    
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