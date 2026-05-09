using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Application.UseCases.Tasks;
using AgileBoard.Domain;
using Moq;

namespace AgileBoard.Tests.Unit.UseCases.Tasks;

[TestFixture]
public class UpdateTaskItemCommandTests
{
    private Mock<ITaskItemRepository> _repositoryMock = null!;
    private UpdateTaskItemCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ITaskItemRepository>();
        _handler = new UpdateTaskItemCommandHandler(_repositoryMock.Object);
    }

    [Test]
    public async Task UpdateTaskItemCommand_ValidData_UpdatesTask()
    {
        var sprintId = SprintId.New();
        var task = TaskItem.Create("Task 1", "Desc", sprintId, ColumnType.New, 0);
        var taskId = task.Id;

        _repositoryMock.Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        var dto = new UpdateTaskItemDto { Name = "New Name", Description = "New Description", ColumnType = "New", Position = 0 };
        var command = new UpdateTaskItemCommand(taskId, dto);

        await _handler.Handle(command, CancellationToken.None);

        Assert.That(task.Name, Is.EqualTo("New Name"));
        Assert.That(task.Description, Is.EqualTo("New Description"));
        _repositoryMock.Verify(r => r.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateTaskItemCommand_ChangeSprint_UpdatesSprintId()
    {
        var sprintId = SprintId.New();
        var newSprintId = SprintId.New();
        var task = TaskItem.Create("Task 1", "Desc", sprintId, ColumnType.New, 0);
        var taskId = task.Id;

        _repositoryMock.Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        var dto = new UpdateTaskItemDto { Name = "Updated Task", Description = "Desc", ColumnType = "New", Position = 0, SprintId = newSprintId.Value };
        var command = new UpdateTaskItemCommand(taskId, dto);

        await _handler.Handle(command, CancellationToken.None);

        Assert.That(task.SprintId.Value, Is.EqualTo(newSprintId.Value));
    }
}
