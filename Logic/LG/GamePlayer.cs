namespace Logic.LG;

public class GamePlayer
{
    public required string Name { get; init; }
    public required GameRole Role { get; init; } // Will change later
    public bool Dead { get; set; }

    public override string ToString() => $"{Name} ({Role})";

    public bool Die() => Role.Die(); // TODO Couple: Make lover die too, Ancien: no death for once
}