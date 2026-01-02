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

    private HashSet<GamePlayer> AlivePlayers => Players.Where(player => !player.Dead).ToHashSet();
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
    private string DeadPlayersMessage => "Morts :\n  - " + string.Join("\n  -", PlayersToDie);

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
                Role = roleFactoryService.New(playerRolePair.Second.Name)
            };
            Players.Add(player);
            Order[player.Role.Phase].Add(player);
        }

        foreach (var phase in Order.Keys)
            Order[phase].Sort((a, b) => a.Role.OrderIndex.CompareTo(b.Role.OrderIndex));
    }

    #endregion

    #region Game Sequence

    private GamePlayer? GetPlayer(string? target) => Players.FirstOrDefault(player => player.Name == target);

    private void KillPlayers(HashSet<GamePlayer> players)
    {
        foreach (var player in players)
        {
            player.Dead = true;
            Order[player.Role.Phase].Remove(player);
        }
    }

    private void NextPhase() => CurrentPhase = CurrentPhase.Next;

    public GameMasterResponse PlayTurn(GameMasterRequest request)
    {
        var target = GetPlayer(request.Target);

        while (true)
        {
            if (AlivePlayers.Count < 2) CurrentPhase = Phase.Over;
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
                    if (CurrentPlayer?.Role.Act(CurrentPlayer, request, Day) is true)
                        CurrentPlayer = NextPlayer;
                    
                    if (CurrentPlayer != null) return CurrentPlayer.Role.Response(Day);

                    NextPhase();
                    CurrentPlayer = null;
                    return CurrentPhase switch
                    {
                        Phase.VillageAwakening or Phase.VillageSleeping => CurrentPhase.MessagedResponse(CurrentPhase.Message + "\n\n" + DeadPlayersMessage),
                        Phase.Lg or Phase.Vote => CurrentPhase.CandidatedResponse(Players.Select(p => p.Name).ToList()),
                        _ => CurrentPhase.Response
                    };
            }
        }
    }

    #endregion

    public override string ToString() => "Game:\n  -" + string.Join("\n  -", Players);
}