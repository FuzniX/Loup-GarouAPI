namespace Logic.LG;

public record Button(string Label, string Action, string Color)
{
    public static Button Next => new("Suivant", nameof(ActionType.Next), "Blue");
    public static Button LgChoice => new("Victime", nameof(ActionType.LgChoice), "Red");
    public static Button Vote => new("Vote", nameof(ActionType.Vote), "Green");
}