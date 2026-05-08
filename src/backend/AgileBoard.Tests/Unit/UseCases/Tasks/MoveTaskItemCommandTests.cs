using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Application.UseCases.Tasks;
using AgileBoard.Domain;
using Moq;

namespace AgileBoard.Tests.Unit.UseCases.Tasks;

[TestFixture]
public class MoveTaskItemCommandTests
{
    private Mock<ITaskItemRepository> _repositoryMock = null!;
    private MoveTaskItemCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ITaskItemRepository>();
        _handler = new MoveTaskItemCommandHandler(_repositoryMock.Object);
    }

    [Test]
    public async Task MoveTaskItemCommand_BetweenColumns_RecalculatesBoth()
    {
        var sprintId = SprintId.New();
        var taskToMove = TaskItem.Create("Task", "Desc", sprintId, ColumnType.New, 0);
        var taskInTarget = TaskItem.Create("In Progress Task", "Desc", sprintId, ColumnType.InProgress, 0);

        var allTasks = new List<TaskItem> { taskToMove, taskInTarget };

        _repositoryMock.Setup(r => r.GetByIdAsync(taskToMove.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskToMove);
        _repositoryMock.Setup(r => r.GetBySprintIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(allTasks);

        var command = new MoveTaskItemCommand(taskToMove.Id, sprintId, ColumnType.InProgress, 1);

        await _handler.Handle(command, CancellationToken.None);

        Assert.That(taskToMove.ColumnType, Is.EqualTo(ColumnType.InProgress));
        Assert.That(taskToMove.Position, Is.EqualTo(1));
        Assert.That(taskInTarget.Position, Is.EqualTo(0));
    }

    [Test]
    public async Task MoveTaskItemCommand_WithinColumn_Reorders()
    {
        var sprintId = SprintId.New();
        var task0 = TaskItem.Create("Task 0", "Desc", sprintId, ColumnType.New, 0);
        var task1 = TaskItem.Create("Task 1", "Desc", sprintId, ColumnType.New, 1);
        var task2 = TaskItem.Create("Task 2", "Desc", sprintId, ColumnType.New, 2);

        var allTasks = new List<TaskItem> { task0, task1, task2 };

        _repositoryMock.Setup(r => r.GetByIdAsync(task0.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task0);
        _repositoryMock.Setup(r => r.GetBySprintIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(allTasks);

        // Move task from position 0 to position 2
        var command = new MoveTaskItemCommand(task0.Id, sprintId, ColumnType.New, 2);

        await _handler.Handle(command, CancellationToken.None);

        Assert.That(task0.Position, Is.EqualTo(2));
        Assert.That(task1.Position, Is.EqualTo(0));
        Assert.That(task2.Position, Is.EqualTo(1));
    }

    [Test]
    public void MoveTaskItemCommand_NotFound_ThrowsNotFoundException()
    {
        var taskId = TaskItemId.New();
        var sprintId = SprintId.New();

        _repositoryMock.Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        var command = new MoveTaskItemCommand(taskId, sprintId, ColumnType.InProgress, 0);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        Assert.That(act, Throws.Exception.TypeOf<NotFoundException>());
    }
}
