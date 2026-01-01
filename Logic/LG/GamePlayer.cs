namespace Logic.LG;

public class GamePlayer
{
    public required string Name { get; init; }
    public required GameRole Role { get; init; } // Will change later

    public override string ToString() => $"{Name} ({Role})";
}