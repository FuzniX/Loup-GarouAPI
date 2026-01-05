using Data;
using Logic.LG.Roles.Abstractions;

namespace Logic.LG;

public class GamePlayer(string name, GameRole role)
{
    public string Name = name;
    public GameRole Role = role;
    public Camp Camp = role.Camp;
    public bool Dead;

    public override string ToString() => $"{Name} ({Role})";

    public bool Die() => Role.Die(); // TODO Couple: Make lover die too, Ancien: no death for once
}

public static class PlayerExtensions
{
    extension(IEnumerable<GamePlayer> playerList)
    {
        public IEnumerable<string> Names => playerList.Select(player => player.Name);
    }
}