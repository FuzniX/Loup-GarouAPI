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
    private Dictionary<Phase, List<GameRole>> Order { get; } = new()
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

    private GameRole? NextRole
    {
        get
        {
            RoleIndex++;

            var rolesInPhase = Order[CurrentPhase];
            if (RoleIndex < rolesInPhase.Count) return rolesInPhase[RoleIndex];

            RoleIndex = -1;
            return null;
        }
    }

    public GamePlayer? GetPlayer(string? target) => Players.FirstOrDefault(player => player.Name == target);

    private string DeadPlayersMessage => PlayersToDie.Count == 0 ? "" : "\n\nMorts :\n  - " + string.Join("\n  - ", PlayersToDie);

    private GameMasterResponse Response => CurrentPhase switch
    {
        Phase.VillageAwakening or Phase.VillageSleeping => CurrentPhase.MessagedResponse(CurrentPhase.Message + DeadPlayersMessage),
        Phase.Lg or Phase.Vote => CurrentPhase.TargetResponse(AlivePlayers.Names.OptionalTarget),
        Phase.Beginning or Phase.Over => CurrentPhase.ButtonlessResponse,
        _ => CurrentRole!.Response
    };

    #endregion

    #region Game Cursor

    internal int Day { get; set; } = 1;
    internal Phase CurrentPhase { get; set; } = Phase.Beginning;
    internal GameMasterRequest? CurrentRequest { get; set; }
    private int RoleIndex { get; set; } = -1;
    private GameRole? CurrentRole { get; set; }

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
            var player = new GamePlayer(
                name: playerRolePair.First.Name,
                role: roleFactoryService.New(playerRolePair.Second.Name, this)
            );
            player.Role.Owner = player;
            Players.Add(player);
            foreach (var rolePhase in player.Role.Phases)
                Order[rolePhase.Phase].Add(player.Role);
        }

        foreach (var phase in Order.Keys)
            Order[phase].Sort((a, b) => a.OrderIndex.CompareTo(b.OrderIndex));
    }

    #endregion

    #region Game Sequence

    private void KillPlayers(HashSet<GamePlayer> players)
    {
        foreach (var player in players.Where(player => player.Die()))
        {
            PlayersToDie.Remove(player);
            foreach (var phase in Order.Keys)
                Order[phase].Remove(player.Role);
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
        if (CurrentRequest.Phase != CurrentPhase) return Response!;
        
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
                    if (CurrentRole is null || CurrentRole.Act())
                        CurrentRole = NextRole;

                    if (CurrentRole is null) NextPhase();
                    else if (!CurrentRole.ShouldRespond) continue;
                    break;
            }
            return Response;
        }
    }

    #endregion

    public override string ToString() => "Game:\n  -" + string.Join("\n  -", Players);
}