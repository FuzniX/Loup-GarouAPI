using Data;
using Logic.Services;

namespace Logic.LG.Roles.Abstractions;

public abstract class ActingRole(Role definition, Game game) : GameRole(definition, game)
{
    public int OrderIndex = definition.DefaultPriority;
    public readonly ICollection<RolePhase> Phases = definition.Phases;
    
    #region Protected Properties

    protected Phase CurrentPhase => Game.CurrentPhase;
    protected virtual string CurrentPhaseDescription => Phases.First(rp => rp.Phase == CurrentPhase).Description;
    protected GameMasterRequest Request => Game.CurrentRequest!;
    protected bool CanUsePower = true;

    #endregion
    
    #region Game Master Response Properties

    protected virtual string AwakeningMessage => (CurrentPhase.Night ? $"{Name} se réveille !\n\n" : "") + CurrentPhaseDescription; // TODO Improve by having it correctly said with a "Spelling" field in database
    protected virtual string SleepingMessage => $"{Name} se rendort.";
    protected virtual Target? Target => Game.AlivePlayers.Names.Target;
    protected virtual IEnumerable<Button> Buttons => Button.Next;
    public virtual bool ShouldRespond => Phases.Select(rp => rp.Phase).Contains(CurrentPhase);
    public GameMasterResponse AwakeningResponse => new(
        Message: AwakeningMessage,
        Image: ImageUrl,
        Phase: CurrentPhase,
        Buttons: Buttons);
    public GameMasterResponse SleepingResponse => new(
        Message: SleepingMessage,
        Image: null,
        Phase: CurrentPhase,
        Buttons: null);

    protected IEnumerable<Button> PowerButtons => CanUsePower ?
        Target.UseablePower :
        [Button.UnusedPower];
    
    #endregion

    public abstract bool Act(out List<GameMasterResponse> responses);
}