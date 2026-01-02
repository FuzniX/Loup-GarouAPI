using Logic.Services;

namespace Logic.LG;

public enum Phase
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
    extension(Phase phase)
    {
        public Phase Next => phase switch
        {
            Phase.Beginning => Phase.VillageSleeping,
            Phase.VillageSleeping => Phase.RolesBeforeLg,
            Phase.RolesBeforeLg => Phase.Lg,
            Phase.Lg => Phase.RolesAfterLg,
            Phase.RolesAfterLg => Phase.VillageAwakening,
            Phase.VillageAwakening => Phase.RolesBeforeVote,
            Phase.RolesBeforeVote => Phase.Vote,
            Phase.Vote => Phase.RolesAfterVote,
            Phase.RolesAfterVote => Phase.VillageSleeping,
            _ => Phase.Over
        };

        public string Message => phase switch
        {
            Phase.Beginning => "Début de la partie.",
            Phase.VillageSleeping => "Le village s'endort.",
            Phase.Lg => "Les Loups-Garous se réveillent.",
            Phase.VillageAwakening => "Le village se réveille.",
            Phase.Vote => "Le village vote.",
            Phase.Over => "Fin de la partie.",
            _ => "..."
        };

        public List<Button> Button => phase switch
        {
            Phase.Lg => [Button.LgChoice],
            Phase.Vote => [Button.Vote],
            Phase.Over => [],
            _ => [Button.Next]
        };

        public GameMasterResponse Response => new(phase.Message, phase.ToString(), phase.Button, null);

        public GameMasterResponse MessagedResponse(string message) => new(message, phase.ToString(), phase.Button, null);
        
        public GameMasterResponse CandidatedResponse(List<string> candidates) => new (phase.Message, phase.ToString(), phase.Button, candidates);
    }
}