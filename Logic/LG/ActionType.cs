namespace Logic.LG;

public enum ActionType
{
    Next
}

public static class ActionExtensions
{
    extension(ActionType actionType)
    {
        public string ToDescription() => actionType switch
            {
                ActionType.Next => "Next step",
                _ => "Unknown"
            };

        public string ToCode() => actionType switch
            {
                ActionType.Next => "NEXT",
                _ => "UNKNOWN"
            };
    }
}