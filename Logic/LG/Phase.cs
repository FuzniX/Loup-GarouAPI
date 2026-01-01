namespace Logic.LG;

public enum Phase
{
    VillageSleeping,
    RolesBeforeLg,
    Lg,
    RolesAfterLg,
    VillageAwakening,
    VillageAfterVote,
    Ended
    RolesBeforeVote,
    Vote,
    RolesAfterVote,
    Over
}

public static class PhaseExtensions
{
    extension(Phase phase)
    {
        public Phase Next() => phase switch
        {
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
    }
}