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

        var dto = new UpdateTaskItemDto("New Name", "New Description");
        var command = new UpdateTaskItemCommand(taskId, sprintId, dto);

        await _handler.Handle(command, CancellationToken.None);

        Assert.That(task.Name, Is.EqualTo("New Name"));
        Assert.That(task.Description, Is.EqualTo("New Description"));
        _repositoryMock.Verify(r => r.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void UpdateTaskItemCommand_NotFound_ThrowsNotFoundException()
    {
        var taskId = TaskItemId.New();
        var sprintId = SprintId.New();

        _repositoryMock.Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        var dto = new UpdateTaskItemDto("New Name", null);
        var command = new UpdateTaskItemCommand(taskId, sprintId, dto);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        Assert.That(act, Throws.Exception.TypeOf<NotFoundException>());
    }
}
