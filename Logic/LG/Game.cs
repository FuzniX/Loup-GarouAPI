using Data;
using Logic.Services;

namespace Logic.LG;

public class Game
{
    #region Public Properties

    public bool Over => CurrentPhase == Phase.Over;

    #endregion

    #region Collections

    private HashSet<GamePlayer> Players { get; } = [];
    private HashSet<GamePlayer> PlayersToDie { get; } = [];
    private Dictionary<Phase, List<GamePlayer>> Order { get; } = new()
    {
        { Phase.RolesBeforeLg, [] },
        { Phase.RolesAfterLg, [] },
        { Phase.RolesBeforeVote, [] },
        { Phase.RolesAfterVote, [] }
    };

    #endregion

    #region Utils

    internal HashSet<GamePlayer> AlivePlayers => Players.Where(player => !player.Dead).ToHashSet();

    private HashSet<GamePlayer> AlivePlayersInCamp(Camp camp) => AlivePlayers.Where(player => player.Role.Camp == camp).ToHashSet();

    private GamePlayer? NextPlayer
    {
        get
        {
            PlayerIndex++;

            var playersInPhase = Order[CurrentPhase];
            if (PlayerIndex < playersInPhase.Count) return playersInPhase[PlayerIndex];

            PlayerIndex = -1;
            return null;
        }
    }

    private GamePlayer? GetPlayer(string? target) => Players.FirstOrDefault(player => player.Name == target);

    private string DeadPlayersMessage => "Morts :\n  - " + string.Join("\n  -", PlayersToDie);

    private GameMasterResponse CurrentPhaseResponse => CurrentPhase switch
    {
        Phase.VillageAwakening or Phase.VillageSleeping => CurrentPhase.MessagedResponse(CurrentPhase.Message + "\n\n" + DeadPlayersMessage),
        Phase.Lg or Phase.Vote => CurrentPhase.CandidatedResponse(AlivePlayers.Select(p => p.Name).ToList()),
        _ => CurrentPhase.Response
    };

    #endregion

    #region Game Cursor

    private int Day { get; set; } = 1;
    private Phase CurrentPhase { get; set; } = Phase.Beginning;
    private int PlayerIndex { get; set; } = -1;
    private GamePlayer? CurrentPlayer { get; set; }

    #endregion

    #region Constructor

    public Game(RoleFactoryService roleFactoryService, Group group, Composition composition)
    {

        if (group.Players.Count != composition.Roles.Count)
            throw new ArgumentException("Should instantiate the same amount of players and roles in the group and composition");

        var groupPlayers = group.Players.Shuffle();
        var compositionRoles = composition.Roles.Shuffle();

        foreach (var playerRolePair in groupPlayers.Zip(compositionRoles))
        {
            var player = new GamePlayer
            {
                Name = playerRolePair.First.Name,
                Role = roleFactoryService.New(playerRolePair.Second.Name, this)
            };
            player.Role.Owner = player;
            Players.Add(player);
            if (player.Role.Phase is { } phase) Order[phase].Add(player);
        }

        foreach (var phase in Order.Keys)
            Order[phase].Sort((a, b) => a.Role.OrderIndex.CompareTo(b.Role.OrderIndex));
    }

    #endregion

    #region Game Sequence

    private void KillPlayers(HashSet<GamePlayer> players)
    {
        foreach (var player in players)
            if (player.Die() && player.Role.Phase is { } phase) Order[phase].Remove(player);
    }

    private void NextPhase() => CurrentPhase =
        AlivePlayers.Count == AlivePlayersInCamp(Camp.Village).Count ||
        AlivePlayers.Count == AlivePlayersInCamp(Camp.LoupGarou).Count ||
        AlivePlayers.Count < 2 ?
            Phase.Over :
            CurrentPhase.Next;

    public GameMasterResponse PlayTurn(GameMasterRequest request)
    {
        while (true)
        {
            switch (CurrentPhase)
            {
                case Phase.Beginning:
                    NextPhase();
                    return CurrentPhase.Response;

                case Phase.VillageSleeping:
                case Phase.VillageAwakening:
                    KillPlayers(PlayersToDie);
                    NextPhase();
                    continue;

                case Phase.Lg:
                case Phase.Vote:
                    var target = GetPlayer(request.Target);
                    if (target != null) PlayersToDie.Add(target);
                    NextPhase();
                    continue;

                case Phase.Over:
                    return Phase.Over.Response;

                case Phase.RolesBeforeLg:
                case Phase.RolesAfterLg:
                case Phase.RolesBeforeVote:
                case Phase.RolesAfterVote:
                default:
                    if (CurrentPlayer is null || CurrentPlayer.Role.Act(request))
                        CurrentPlayer = NextPlayer;

                    if (CurrentPlayer != null) return CurrentPlayer.Role.Response;

                    NextPhase();
                    CurrentPlayer = null;
                    return CurrentPhaseResponse;
            }
        }
    }

    #endregion

    public override string ToString() => "Game:\n  -" + string.Join("\n  -", Players);
}