namespace AgileBoard.Domain;

public record TaskItemId(Guid Value)
{
    public static TaskItemId New() => new(Guid.NewGuid());
}
