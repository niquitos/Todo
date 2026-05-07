using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Domain;
using Moq;

namespace AgileBoard.Tests.Unit.UseCases.Sprints;

[TestFixture]
public class GetSprintByIdQueryTests
{
    private Mock<ISprintRepository> _repositoryMock = null!;
    private GetSprintByIdQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ISprintRepository>();
        _handler = new GetSprintByIdQueryHandler(_repositoryMock.Object);
    }

    [Test]
    public async Task GetSprintByIdQuery_ExistingId_ReturnsSprintDto()
    {
        // Arrange
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14), "Description");

        _repositoryMock.Setup(r => r.GetByIdAsync(sprint.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sprint);

        var query = new GetSprintByIdQuery(sprint.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Id, Is.EqualTo(sprint.Id.Value));
        Assert.That(result.Name, Is.EqualTo(sprint.Name));
    }

    [Test]
    public void GetSprintByIdQuery_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        var sprintId = SprintId.New();
        _repositoryMock.Setup(r => r.GetByIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sprint?)null);

        var query = new GetSprintByIdQuery(sprintId);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(act, Throws.Exception.TypeOf<NotFoundException>());
    }
}
