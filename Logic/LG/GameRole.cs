using System.Reflection;
using Data;
using Logic.Services;

namespace Logic.LG;

public abstract class GameRole
{
    private Role Definition { get; }
    public required Camp Camp { get; init; }
    public required int OrderIndex { get; init; }
    public required Phase Phase { get; init; }
    public string Name => Definition.Name;
    public string Description => Definition.Description;
    public string ImageURL => Definition.ImageURL;

    protected GameRole(Role definition)
    {
        Definition = definition;
        OrderIndex = definition.DefaultPriority;

        var config = GetType().GetCustomAttribute<RoleIdentifierAttribute>();
        if (config == null) throw new ArgumentNullException($"{GetType().Name} does not have a RoleIdentifier.");

        Camp = config.Camp;
        Phase = config.Phase;
    }

    public abstract GameMasterResponse Act(GamePlayer roleOwner, GameMasterRequest request, int day);

    public override string ToString() => Name;
}