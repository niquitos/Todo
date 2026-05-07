using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Domain;
using Moq;

namespace AgileBoard.Tests.Unit.UseCases.Sprints;

[TestFixture]
public class GetSprintsQueryTests
{
    private Mock<ISprintRepository> _repositoryMock = null!;
    private GetSprintsQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ISprintRepository>();
        _handler = new GetSprintsQueryHandler(_repositoryMock.Object);
    }

    [Test]
    public async Task GetSprintsQuery_ReturnsAllSprints()
    {
        // Arrange
        var sprint1 = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        var sprint2 = Sprint.Create("Sprint 2", DateTime.UtcNow.AddDays(15), DateTime.UtcNow.AddDays(29));
        var sprint3 = Sprint.Create("Sprint 3", DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddDays(44));

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Sprint> { sprint1, sprint2, sprint3 });

        var query = new GetSprintsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
    }
}
