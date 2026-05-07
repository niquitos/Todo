using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Domain;
using Moq;

namespace AgileBoard.Tests.Unit.UseCases.Sprints;

[TestFixture]
public class CreateSprintCommandTests
{
    private Mock<ISprintRepository> _repositoryMock = null!;
    private CreateSprintCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ISprintRepository>();
        _handler = new CreateSprintCommandHandler(_repositoryMock.Object);
    }

    [Test]
    public async Task CreateSprintCommand_ValidData_ReturnsId()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Sprint>());

        var dto = new CreateSprintDto("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14), "Description");
        var command = new CreateSprintCommand(dto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void CreateSprintCommand_OverlappingDates_ThrowsSprintOverlapException()
    {
        // Arrange
        var existingSprint = Sprint.Create("Existing", new DateTime(2026, 1, 1), new DateTime(2026, 1, 14));
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Sprint> { existingSprint });

        var dto = new CreateSprintDto("New Sprint", new DateTime(2026, 1, 10), new DateTime(2026, 1, 20), "Description");
        var command = new CreateSprintCommand(dto);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(act, Throws.Exception.TypeOf<SprintOverlapException>());
    }
}
