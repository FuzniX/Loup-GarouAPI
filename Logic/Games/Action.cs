namespace Logic;

public enum Action
{
    Next
}

public static class ActionExtensions
{
    public static string ToDescription(this Action action)
    {
        return action switch
        {
            Action.Next => "Next step",
            _ => "Unknown"
        };
    }
    
    public static string ToCode(this Action action)
    {
        return action switch
        {
            Action.Next => "NEXT",
            _ => "UNKNOWN"
        };
    }
}