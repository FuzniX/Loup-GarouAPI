using Data;
using Logic.Services;

namespace Logic.LG;

public abstract class GameRole(Role definition, Game game)
{
    #region Static Properties

    private Role Definition { get; } = definition;
    public string Name => Definition.Name;
    public string Description => Definition.Description;
    public string ImageUrl => Definition.ImageUrl;

    #endregion
    
    #region Game state dependant properties

    public Camp Camp { get; set; } = definition.Camp;
    public int OrderIndex { get; set; } = definition.DefaultPriority;
    public Phase? Phase { get; set; } = definition.Phase;
    public required GamePlayer Owner { get; set; }
    
    #endregion
    
    #region Game Master Response Properties

    public string Message => $"{Name} se réveille !\n\n{Description}"; // TODO Improve by having it correctly said with a "Spelling" field in database
    public List<string>? Candidates => null;
    public List<Button>? Buttons => [Button.Next];
    public GameMasterResponse Response => new(
        Message: Message,
        Phase: Phase.ToString()!,
        Buttons: Buttons,
        Candidates: Candidates
    );

    #endregion

    public abstract bool Act(GameMasterRequest request);

    public bool Die() => Owner.Dead = true;

    public override string ToString() => Name;
}