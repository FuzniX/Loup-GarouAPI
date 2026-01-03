using Logic.Services;

namespace Logic.LG;

public enum GamePhase
{
    Beginning,
    VillageSleeping,
    RolesBeforeLg,
    Lg,
    RolesAfterLg,
    VillageAwakening,
    RolesBeforeVote,
    Vote,
    RolesAfterVote,
    Over
}

public static class PhaseExtensions
{
    extension(GamePhase phase)
    {
        public GamePhase Next => phase switch
        {
            GamePhase.Beginning => GamePhase.VillageSleeping,
            GamePhase.VillageSleeping => GamePhase.RolesBeforeLg,
            GamePhase.RolesBeforeLg => GamePhase.Lg,
            GamePhase.Lg => GamePhase.RolesAfterLg,
            GamePhase.RolesAfterLg => GamePhase.VillageAwakening,
            GamePhase.VillageAwakening => GamePhase.RolesBeforeVote,
            GamePhase.RolesBeforeVote => GamePhase.Vote,
            GamePhase.Vote => GamePhase.RolesAfterVote,
            GamePhase.RolesAfterVote => GamePhase.VillageSleeping,
            _ => GamePhase.Over
        };

        public string Message => phase switch
        {
            GamePhase.Beginning => "Début de la partie.",
            GamePhase.VillageSleeping => "Le village s'endort.",
            GamePhase.Lg => "Les Loups-Garous se réveillent.",
            GamePhase.VillageAwakening => "Le village se réveille.",
            GamePhase.Vote => "Le village vote.",
            GamePhase.Over => "Fin de la partie.",
            _ => "..."
        };

        public List<Button> Button => phase switch
        {
            GamePhase.Lg => [Button.LgChoice],
            GamePhase.Vote => [Button.Vote],
            GamePhase.Over => [],
            _ => [Button.Next]
        };

        public GameMasterResponse Response => new(phase.Message, phase.ToString(), phase.Button, null);

        public GameMasterResponse MessagedResponse(string message) => new(message, phase.ToString(), phase.Button, null);
        
        public GameMasterResponse CandidatedResponse(List<string> candidates) => new (phase.Message, phase.ToString(), phase.Button, candidates);
    }
}