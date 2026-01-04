using Data;
using Logic.Services;

namespace Logic.LG;

public abstract class GameRole(Role definition, Game game)
{
    #region Static Properties

    private Role Definition { get; } = definition;
    protected Game Game => game;
    public string Name => Definition.Name;
    public string ImageUrl => Definition.ImageUrl;

    #endregion

    #region Game state dependant properties
    
    #region Public Properties

    public Camp Camp = definition.Camp;
    public int OrderIndex = definition.DefaultPriority;
    public readonly ICollection<RolePhase> Phases = definition.Phases;
    public required GamePlayer Owner;
    
    #endregion

    #region Protected Properties

    protected Phase CurrentPhase => Game.CurrentPhase;
    protected virtual string CurrentPhaseDescription => Phases.First(rp => rp.Phase == CurrentPhase).Description;
    protected GameMasterRequest Request => Game.CurrentRequest!;
    protected bool CanUsePower = true;

    #endregion
    
    #endregion

    #region Game Master Response Properties

    public string Message => (CurrentPhase.Night ? $"{Name} se réveille !\n\n" : "") + CurrentPhaseDescription; // TODO Improve by having it correctly said with a "Spelling" field in database
    public virtual Target? Target => Game.AlivePlayers.Names.Target;
    public virtual IEnumerable<Button> Buttons => Button.Next;
    public virtual bool ShouldRespond => Phases.Select(rp => rp.Phase).Contains(CurrentPhase);
    public GameMasterResponse Response => new(
        Message: Message,
        Image: ImageUrl,
        Phase: CurrentPhase,
        Buttons: Buttons
    );

    public IEnumerable<Button> PowerButtons => CanUsePower ?
        Target.UseablePower :
        [Button.UnusedPower];
    
    #endregion

    #region Game Methods

    public virtual bool Act() => false;

    public virtual bool Die()
    {
        Owner.Dead = true;
        return true;
    }

    #endregion

    public override string ToString() => Name;
}