using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Tasks;
using AgileBoard.Domain;
using Moq;

namespace AgileBoard.Tests.Unit.UseCases.Tasks;

[TestFixture]
public class GetTaskItemsQueryTests
{
    private Mock<ITaskItemRepository> _repositoryMock = null!;
    private GetTaskItemsQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ITaskItemRepository>();
        _handler = new GetTaskItemsQueryHandler(_repositoryMock.Object);
    }

    [Test]
    public async Task GetTaskItemsQuery_ReturnsTasksForSprint()
    {
        var sprintId = SprintId.New();
        var tasks = new List<TaskItem>
        {
            TaskItem.Create("Task 1", "Desc", sprintId, ColumnType.New, 0),
            TaskItem.Create("Task 2", "Desc", sprintId, ColumnType.New, 1)
        };

        _repositoryMock.Setup(r => r.GetBySprintIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        var query = new GetTaskItemsQuery(sprintId);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetTaskItemsQuery_EmptySprint_ReturnsEmptyList()
    {
        var sprintId = SprintId.New();

        _repositoryMock.Setup(r => r.GetBySprintIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem>());

        var query = new GetTaskItemsQuery(sprintId);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.That(result, Is.Empty);
    }
}
