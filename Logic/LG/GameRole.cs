using Data;
using Logic.Services;

namespace Logic.LG;

public abstract class GameRole(Role definition, Game game)
{
    private Role Definition { get; } = definition;
    public Camp Camp { get; set; } = definition.Camp;
    public int OrderIndex { get; set; } = definition.DefaultPriority;
    public Phase? Phase { get; set; } = definition.Phase;
    public string Name => Definition.Name;
    public string Description => Definition.Description;
    public string ImageUrl => Definition.ImageUrl;
    public required GamePlayer Owner { get; set; }

    // var config = GetType().GetCustomAttribute<RoleIdentifierAttribute>();
    // if (config == null) throw new ArgumentNullException($"{GetType().Name} does not have a RoleIdentifier.");
    //
    // Phase = config.Phase;
    // Camp = config.Camp;

    public GameMasterResponse Response(Game game) => new(
        Message: $"{Name} se réveille !",
        Phase: Phase?.ToString() ?? "...",
        Buttons: [Button.Next],
        Candidates: null
    );

    public abstract bool Act(GameMasterRequest request);

    public override string ToString() => Name;
}