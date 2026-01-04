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

    private string DeadPlayersMessage => PlayersToDie.Count == 0 ? "" : "\n\nMorts :\n  - " + string.Join("\n  - ", PlayersToDie);

    private GameMasterResponse Response => CurrentPhase switch
    {
        Phase.VillageAwakening or Phase.VillageSleeping => CurrentPhase.MessagedResponse(CurrentPhase.Message + DeadPlayersMessage),
        Phase.Lg or Phase.Vote => CurrentPhase.TargetResponse(AlivePlayers.Select(p => p.Name).ToList().OptionalTarget),
        Phase.Beginning or Phase.Over => CurrentPhase.Response,
        _ => CurrentPlayer!.Role.Response
    };

    #endregion

    #region Game Cursor

    private int Day { get; set; } = 1;
    internal Phase CurrentPhase { get; set; } = Phase.Beginning;
    private int PlayerIndex { get; set; } = -1;
    private GamePlayer? CurrentPlayer { get; set; }
    internal GameMasterRequest CurrentRequest { get; set; }

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
            foreach (var rolePhase in player.Role.Phases)
                Order[rolePhase.Phase].Add(player);
        }

        foreach (var phase in Order.Keys)
            Order[phase].Sort((a, b) => a.Role.OrderIndex.CompareTo(b.Role.OrderIndex));
    }

    #endregion

    #region Game Sequence

    private void KillPlayers(HashSet<GamePlayer> players)
    {
        foreach (var player in players.Where(player => player.Die()))
        {
            PlayersToDie.Remove(player);
            foreach (var phase in Order.Keys)
                Order[phase].Remove(player);
        }
    }

    private void NextPhase() => CurrentPhase =
        AlivePlayers.Count == AlivePlayersInCamp(Camp.Village).Count ||
        AlivePlayers.Count == AlivePlayersInCamp(Camp.LoupGarou).Count ||
        AlivePlayers.Count < 2 ?
            Phase.Over :
            CurrentPhase.Next;

    public GameMasterResponse PlayTurn(GameMasterRequest request)
    {
        CurrentRequest = request;
        if (CurrentRequest.Phase != CurrentPhase) return Response;
        
        while (true)
        {
            switch (CurrentPhase)
            {
                case Phase.Beginning:
                    NextPhase();
                    break;

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
                    break;

                case Phase.RolesBeforeLg:
                case Phase.RolesAfterLg:
                case Phase.RolesBeforeVote:
                case Phase.RolesAfterVote:
                default:
                    if (CurrentPlayer is null || CurrentPlayer.Role.Act())
                        CurrentPlayer = NextPlayer;

                    if (CurrentPlayer is null) NextPhase();

                    break;
            }
            return Response;
        }
    }

    #endregion

    public override string ToString() => "Game:\n  -" + string.Join("\n  -", Players);
}