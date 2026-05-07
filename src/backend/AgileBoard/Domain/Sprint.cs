namespace AgileBoard.Domain;

public class Sprint
{
    public SprintId Id { get; private set; }
    public string Name { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string? Description { get; private set; }

    private Sprint()
    {
        Id = SprintId.New();
        Name = string.Empty;
        StartDate = DateTime.MinValue;
        EndDate = DateTime.MinValue;
    }

    public static Sprint Create(string name, DateTime startDate, DateTime endDate, string? description = null)
    {
        return new Sprint
        {
            Id = SprintId.New(),
            Name = name,
            StartDate = startDate,
            EndDate = endDate,
            Description = description
        };
    }

    public void Update(string name, DateTime startDate, DateTime endDate, string? description = null)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
    }

    public bool OverlapsWith(DateTime start, DateTime end)
    {
        return start < EndDate && end > StartDate;
    }
}
