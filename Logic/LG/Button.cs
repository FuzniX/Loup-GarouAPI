namespace Logic.LG;

public enum Color
{
    Blue,
    Red,
    Green
}

public record Button(string Label, ActionType Action, Color Color)
{
    public static Button Next => new("Suivant", ActionType.Next, Color.Blue);
    public static Button LgChoice => new("Victime", ActionType.LgChoice, Color.Red);
    public static Button Vote => new("Vote", ActionType.Vote, Color.Green);
}