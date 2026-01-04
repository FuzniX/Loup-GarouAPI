using Data;
using Logic.Services;

namespace Logic.LG;

public abstract class GameRole(Role definition, Game game)
{
    #region Static Properties

    private Role Definition { get; } = definition;
    public string Name => Definition.Name;
    public string ImageUrl => Definition.ImageUrl;

    #endregion

    #region Game state dependant properties
    
    #region Public Properties

    public Camp Camp { get; set; } = definition.Camp;
    public int OrderIndex { get; set; } = definition.DefaultPriority;
    public ICollection<RolePhase> Phases { get; set; } = definition.Phases;
    public required GamePlayer Owner { get; set; }
    
    #endregion

    #region Protected Properties

    protected Phase CurrentPhase => game.CurrentPhase;
    protected string CurrentPhaseDescription => Phases.First(rp => rp.Phase == CurrentPhase).Description;
    protected GameMasterRequest CurrentRequest => game.CurrentRequest;

    #endregion
    
    #endregion

    #region Game Master Response Properties

    public string Message => (CurrentPhase.Night ? $"{Name} se réveille !\n\n" : "") + CurrentPhaseDescription; // TODO Improve by having it correctly said with a "Spelling" field in database
    public Target? Target => null;
    public List<Button>? Buttons => [Button.Next];
    public GameMasterResponse Response => new(
        Message: Message,
        Phase: CurrentPhase,
        Buttons: Buttons,
        Target: Target
    );

    #endregion

    public abstract bool Act();

    public bool Die()
    {
        Owner.Dead = true;
        return true;
    }

    public override string ToString() => Name;
}