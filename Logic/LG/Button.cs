namespace Logic.LG;

public record Target(bool Mandatory, IEnumerable<string> Candidates);

public record Button(string Label, ActionType Action, Color Color, Target? Target)
{
    public static List<Button> Next => [new("Suivant", ActionType.Next, Color.Blue, null)];
    public static Button UnusedPower => new("Pouvoir Non Utilisé", ActionType.PowerUnused, Color.Red, null);
}

public static class ButtonExtensions
{
    extension(Target? target)
    {
        public List<Button> LgChoice => [new("Victime", ActionType.LgChoice, Color.Red, target)];
        public List<Button> Vote => [new("Vote", ActionType.Vote, Color.Gray, target)];
        public List<Button> UseablePower => [target.UsedPower, Button.UnusedPower];
        public Button UsedPower => new("Pouvoir Utilisé", ActionType.PowerUsed, Color.Green, target);
    }
    
    extension(IEnumerable<string> enumerable)
    {
        public Target Target => new(true, enumerable);
        public Target OptionalTarget => new(false, enumerable);
    }
}

public enum Color
{
    Blue,
    Red,
    Green,
    Gray,
}