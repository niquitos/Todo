using AgileBoard.Domain;

namespace AgileBoard.Application.UseCases.Tasks;

public record TaskItemDto(
    Guid Id,
    string Name,
    string? Description,
    Guid SprintId,
    string ColumnType,
    int Position
)
{
    public static TaskItemDto FromTaskItem(TaskItem taskItem) => new(
        taskItem.Id.Value,
        taskItem.Name,
        taskItem.Description,
        taskItem.SprintId.Value,
        taskItem.ColumnType.ToString(),
        taskItem.Position
    );
}

public record CreateTaskItemDto(
    string Name,
    string? Description,
    string ColumnType
);

public record UpdateTaskItemDto(
    string Name,
    string? Description
);

public record MoveTaskItemDto(
    Guid TaskId,
    string NewColumnType,
    int NewPosition
);
