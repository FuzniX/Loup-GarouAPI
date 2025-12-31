using Logic.LG.Roles;

namespace Logic.LG;

public class GamePlayer(string name, GameRole role)
{
    public override string ToString() => $"{name} ({role})";
}