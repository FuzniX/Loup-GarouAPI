using Data;
using Logic.Services;

namespace Logic.LG;

public abstract class GameRole(Role role)
{
    private Role Definition { get; } = role;
    public required Camp Camp { get; init; }
    public required int OrderIndex { get; init; }
    public required Phase Phase { get; init; }
    public string Name => Definition.Name;
    public string Description => Definition.Description;
    public string ImageURL => Definition.ImageURL;

    // Name = GetType().GetCustomAttribute<RoleIdentifierAttribute>()?.RoleName ?? "Unknown";

    public abstract ActionType RequiredAction(int day);

    public abstract GameMasterResponse Act(GamePlayer roleOwner, GameMasterRequest request, int day);

    public override string ToString() => Name;
}