using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Tasks;
using AgileBoard.Domain;
using Moq;

namespace AgileBoard.Tests.Unit.UseCases.Tasks;

[TestFixture]
public class CreateTaskItemCommandTests
{
    private Mock<ITaskItemRepository> _repositoryMock = null!;
    private CreateTaskItemCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ITaskItemRepository>();
        _handler = new CreateTaskItemCommandHandler(_repositoryMock.Object);
    }

    [Test]
    public async Task CreateTaskItemCommand_ValidData_ReturnsId()
    {
        var sprintId = SprintId.New();
        _repositoryMock.Setup(r => r.GetBySprintIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem>());

        var dto = new CreateTaskItemDto { Name = "Task 1", Description = "Description", ColumnType = "New" };
        var command = new CreateTaskItemCommand(sprintId, dto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.That(result, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task CreateTaskItemCommand_ShiftsExistingTasks()
    {
        var sprintId = SprintId.New();
        var existingTasks = new List<TaskItem>
        {
            TaskItem.Create("Task A", "Desc", sprintId, ColumnType.New, 0),
            TaskItem.Create("Task B", "Desc", sprintId, ColumnType.New, 1)
        };

        _repositoryMock.Setup(r => r.GetBySprintIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTasks);

        var dto = new CreateTaskItemDto { Name = "Task 1", Description = "Description", ColumnType = "New" };
        var command = new CreateTaskItemCommand(sprintId, dto);

        await _handler.Handle(command, CancellationToken.None);

        // Existing tasks should be shifted to positions 1 and 2
        Assert.That(existingTasks[0].Position, Is.EqualTo(1));
        Assert.That(existingTasks[1].Position, Is.EqualTo(2));
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
