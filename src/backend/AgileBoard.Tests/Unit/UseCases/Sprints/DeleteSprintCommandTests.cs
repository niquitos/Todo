using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Domain;
using Moq;

namespace AgileBoard.Tests.Unit.UseCases.Sprints;

[TestFixture]
public class DeleteSprintCommandTests
{
    private Mock<ISprintRepository> _sprintRepositoryMock = null!;
    private Mock<ITaskItemRepository> _taskItemRepositoryMock = null!;
    private DeleteSprintCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _sprintRepositoryMock = new Mock<ISprintRepository>();
        _taskItemRepositoryMock = new Mock<ITaskItemRepository>();
        _handler = new DeleteSprintCommandHandler(_sprintRepositoryMock.Object, _taskItemRepositoryMock.Object);
    }

    [Test]
    public async Task DeleteSprintCommand_ExistingId_DeletesSprint()
    {
        // Arrange
        var sprintId = SprintId.New();
        var defaultSprint = Sprint.Create("Не запланировано", DateTime.MinValue, DateTime.MaxValue, isDefault: true);
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));

        _sprintRepositoryMock.Setup(r => r.GetByIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sprint);
        _sprintRepositoryMock.Setup(r => r.GetDefaultAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(defaultSprint);
        _taskItemRepositoryMock.Setup(r => r.GetBySprintIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem>());

        var command = new DeleteSprintCommand(sprintId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _sprintRepositoryMock.Verify(r => r.DeleteAsync(sprintId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void DeleteSprintCommand_DefaultSprint_Throws()
    {
        // Arrange
        var sprintId = SprintId.New();
        var defaultSprint = Sprint.Create("Не запланировано", DateTime.MinValue, DateTime.MaxValue, isDefault: true);

        _sprintRepositoryMock.Setup(r => r.GetByIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(defaultSprint);

        var command = new DeleteSprintCommand(sprintId);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task DeleteSprintCommand_WithTasks_ReassignsTasksToDefault()
    {
        // Arrange
        var sprintId = SprintId.New();
        var defaultSprint = Sprint.Create("Не запланировано", DateTime.MinValue, DateTime.MaxValue, isDefault: true);
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        var task = TaskItem.Create("Task 1", null, sprintId, ColumnType.New, 0);

        _sprintRepositoryMock.Setup(r => r.GetByIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sprint);
        _sprintRepositoryMock.Setup(r => r.GetDefaultAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(defaultSprint);
        _taskItemRepositoryMock.Setup(r => r.GetBySprintIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem> { task });

        var command = new DeleteSprintCommand(sprintId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _taskItemRepositoryMock.Verify(r => r.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Once);
        _sprintRepositoryMock.Verify(r => r.DeleteAsync(sprintId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
