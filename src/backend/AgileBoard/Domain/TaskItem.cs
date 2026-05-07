namespace AgileBoard.Domain;

public class TaskItem
{
    public TaskItemId Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public SprintId SprintId { get; private set; }
    public ColumnType ColumnType { get; private set; }
    public int Position { get; private set; }

    private TaskItem()
    {
        Id = TaskItemId.New();
        Name = string.Empty;
        SprintId = SprintId.New();
    }

    public static TaskItem Create(string name, string? description, SprintId sprintId, ColumnType columnType, int position)
    {
        return new TaskItem
        {
            Id = TaskItemId.New(),
            Name = name,
            Description = description,
            SprintId = sprintId,
            ColumnType = columnType,
            Position = position
        };
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public void Move(ColumnType columnType, int position)
    {
        ColumnType = columnType;
        Position = position;
    }

    public void ReassignTo(SprintId sprintId)
    {
        SprintId = sprintId;
    }
}
