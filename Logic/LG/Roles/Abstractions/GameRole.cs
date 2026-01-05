using Data;

namespace Logic.LG.Roles.Abstractions;

public abstract class GameRole(Role definition, Game game)
{
    #region Static Properties

    private Role Definition { get; } = definition;
    protected Game Game => game;
    public string Name => Definition.Name;
    public string ImageUrl => Definition.ImageUrl;

    #endregion

    #region Game state dependant properties

    public Camp Camp = definition.Camp;
    public required GamePlayer Owner;
    
    #endregion

    public virtual bool Die()
    {
        Owner.Dead = true;
        return true;
    }

    public override string ToString() => Name;
}