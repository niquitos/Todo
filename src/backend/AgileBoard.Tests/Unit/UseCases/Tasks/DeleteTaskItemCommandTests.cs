using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Application.UseCases.Tasks;
using AgileBoard.Domain;
using Moq;

namespace AgileBoard.Tests.Unit.UseCases.Tasks;

[TestFixture]
public class DeleteTaskItemCommandTests
{
    private Mock<ITaskItemRepository> _repositoryMock = null!;
    private DeleteTaskItemCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ITaskItemRepository>();
        _handler = new DeleteTaskItemCommandHandler(_repositoryMock.Object);
    }

    [Test]
    public async Task DeleteTaskItemCommand_DeletesAndShifts()
    {
        var sprintId = SprintId.New();
        var taskToDelete = TaskItem.Create("Task to delete", "Desc", sprintId, ColumnType.New, 0);
        var taskBelow1 = TaskItem.Create("Below 1", "Desc", sprintId, ColumnType.New, 1);
        var taskBelow2 = TaskItem.Create("Below 2", "Desc", sprintId, ColumnType.New, 2);

        var allTasks = new List<TaskItem> { taskToDelete, taskBelow1, taskBelow2 };

        _repositoryMock.Setup(r => r.GetByIdAsync(taskToDelete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskToDelete);
        _repositoryMock.Setup(r => r.GetBySprintIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(allTasks);

        var command = new DeleteTaskItemCommand(taskToDelete.Id, sprintId);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.DeleteAsync(taskToDelete.Id, It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(taskBelow1.Position, Is.EqualTo(0));
        Assert.That(taskBelow2.Position, Is.EqualTo(1));
    }

    [Test]
    public void DeleteTaskItemCommand_NotFound_ThrowsNotFoundException()
    {
        var taskId = TaskItemId.New();
        var sprintId = SprintId.New();

        _repositoryMock.Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        var command = new DeleteTaskItemCommand(taskId, sprintId);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        Assert.That(act, Throws.Exception.TypeOf<NotFoundException>());
    }
}
