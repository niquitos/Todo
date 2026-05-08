using AgileBoard.Domain;

namespace AgileBoard.Application.UseCases.Sprints;

public record SprintDto(
    Guid Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    string? Description,
    bool IsDefault
)
{
    public static SprintDto FromSprint(Sprint sprint) => new(
        sprint.Id.Value,
        sprint.Name,
        sprint.StartDate,
        sprint.EndDate,
        sprint.Description,
        sprint.IsDefault
    );
}

public record CreateSprintDto(
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    string? Description
);

public record UpdateSprintDto(
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    string? Description
);
