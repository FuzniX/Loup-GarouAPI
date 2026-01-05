using Data;
using Logic.LG.Roles.Abstractions;
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
    private Dictionary<Phase, List<ActingRole>> Order { get; } = new()
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

    public GamePlayer? GetPlayer(string? target) => Players.FirstOrDefault(player => player.Name.Equals(target, StringComparison.CurrentCultureIgnoreCase));

    private string DeadPlayersMessage => PlayersToDie.Count == 0 ? "" : "\n\nMorts :\n  - " + string.Join("\n  - ", PlayersToDie);

    private GameMasterResponse PhaseResponse => CurrentPhase switch
    {
        Phase.VillageAwakening or Phase.VillageSleeping => CurrentPhase.MessagedResponse(CurrentPhase.Message + DeadPlayersMessage),
        Phase.Lg or Phase.Vote => CurrentPhase.TargetResponse(AlivePlayers.Names.OptionalTarget),
        Phase.Beginning or Phase.Over => CurrentPhase.ButtonlessResponse,
        _ => CurrentRole!.AwakeningResponse
    };

    #endregion

    #region Game Cursor

    internal int Day { get; set; } = 1;
    internal Phase CurrentPhase { get; set; } = Phase.Beginning;
    internal GameMasterRequest? CurrentRequest { get; set; }
    private int RoleIndex { get; set; } = -1;
    private ActingRole? CurrentRole { get; set; }

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
            if (player.Role is not ActingRole actingRole) continue;
            foreach (var rolePhase in actingRole.Phases)
                Order[rolePhase.Phase].Add(actingRole);
        }

        foreach (var phase in Order.Keys)
            Order[phase].Sort((a, b) => a.OrderIndex.CompareTo(b.OrderIndex));
    }

    #endregion

    #region Game Sequence

    #region Helper Methods

    private void KillPlayers(HashSet<GamePlayer> players)
    {
        foreach (var player in players.Where(player => player.Die()))
        {
            PlayersToDie.Remove(player);
            
            if (player.Role is not ActingRole actingRole) continue;
            foreach (var phase in Order.Keys)
                Order[phase].Remove(actingRole);
        }
    }

    private void NextPhase()
    {
        RoleIndex = -1;
        CurrentPhase =
            AlivePlayers.Count == AlivePlayersInCamp(Camp.Village).Count ||
            AlivePlayers.Count == AlivePlayersInCamp(Camp.LoupGarou).Count ||
            AlivePlayers.Count < 2 ?
                Phase.Over :
                CurrentPhase.Next;
    }
    
    private void NextRole()
    {
        var rolesInPhase = Order[CurrentPhase];
        CurrentRole = ++RoleIndex < rolesInPhase.Count ? rolesInPhase[RoleIndex] : null;
    }

    #endregion

    public List<GameMasterResponse> PlayTurn(GameMasterRequest request)
    {
        CurrentRequest = request;
        if (CurrentRequest.Phase != CurrentPhase) return [PhaseResponse];
        
        var responses = new List<GameMasterResponse>();
        
        while (true)
        {
            switch (CurrentPhase)
            {
                case Phase.Beginning:
                    NextPhase();
                    break;

                case Phase.VillageSleeping:
                    Day++;
                    KillPlayers(PlayersToDie);
                    NextPhase();
                    continue;
                
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
                    if (CurrentRole is not null) // If there is even a role in this Phase
                    {
                        var next = CurrentRole.Act(out var roleResponses);
                        responses.AddRange(roleResponses);
                        if (next)
                        {
                            if (CurrentPhase.Night) responses.Add(CurrentRole.SleepingResponse);
                            NextRole();
                        }
                    }
                    
                    if (CurrentRole is null) NextRole(); // Phase started
                    if (CurrentRole is null) NextPhase(); // Phase ended
                    else if (!CurrentRole.ShouldRespond) continue;
                    break;
            }
            responses.Add(PhaseResponse);
            return responses;
        }
    }

    #endregion

    public override string ToString() => "Game:\n  -" + string.Join("\n  -", Players);
}