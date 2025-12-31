namespace Logic.Games;

public class Game(Data.Group group, Data.Composition composition)
{
    public override string ToString() => $"{group}\n{composition}";
}