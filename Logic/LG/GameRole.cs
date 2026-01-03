using Data;
using Logic.Services;

namespace Logic.LG;

public abstract class GameRole(Role definition)
{
    private Role Definition { get; } = definition;
    public required GameCamp Camp { get; set; } = Enum.Parse<GameCamp>(definition.Camp.Name);
    public required int OrderIndex { get; set; } = definition.DefaultPriority;
    public required GamePhase Phase { get; set; } = Enum.Parse<GamePhase>(definition.Phase.Name);
    public string Name => Definition.Name;
    public string Description => Definition.Description;
    public string ImageUrl => Definition.ImageUrl;

    // var config = GetType().GetCustomAttribute<RoleIdentifierAttribute>();
    // if (config == null) throw new ArgumentNullException($"{GetType().Name} does not have a RoleIdentifier.");
    //
    // Phase = config.Phase;
    // Camp = config.Camp;

    public GameMasterResponse Response(int day) => new(
        Message: $"{Name} se réveille !",
        Phase: Phase.ToString(),
        Buttons: [Button.Next],
        Candidates: null
    );

    public abstract bool Act(GamePlayer roleOwner, GameMasterRequest request, int day);

    public override string ToString() => Name;
}