namespace AgileBoard.Domain;

public record SprintId(Guid Value)
{
    public static SprintId New() => new(Guid.NewGuid());
}
