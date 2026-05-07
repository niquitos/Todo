using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Domain;
using Moq;

namespace AgileBoard.Tests.Unit.UseCases.Sprints;

[TestFixture]
public class UpdateSprintCommandTests
{
    private Mock<ISprintRepository> _repositoryMock = null!;
    private UpdateSprintCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ISprintRepository>();
        _handler = new UpdateSprintCommandHandler(_repositoryMock.Object);
    }

    [Test]
    public async Task UpdateSprintCommand_ValidData_UpdatesSprint()
    {
        // Arrange
        var sprintId = SprintId.New();
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14), "Description");

        _repositoryMock.Setup(r => r.GetByIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sprint);
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Sprint>());

        var dto = new UpdateSprintDto("Updated", DateTime.UtcNow, DateTime.UtcNow.AddDays(14), "Updated Description");
        var command = new UpdateSprintCommand(sprintId, dto);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(sprint, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void UpdateSprintCommand_OverlappingDates_ThrowsSprintOverlapException()
    {
        // Arrange
        var sprintId = SprintId.New();
        var sprint = Sprint.Create("Sprint 1", new DateTime(2026, 1, 1), new DateTime(2026, 1, 14));
        var otherSprint = Sprint.Create("Other", new DateTime(2026, 1, 10), new DateTime(2026, 1, 20));

        _repositoryMock.Setup(r => r.GetByIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sprint);
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Sprint> { otherSprint });

        var dto = new UpdateSprintDto("Updated", new DateTime(2026, 1, 12), new DateTime(2026, 1, 25), "Description");
        var command = new UpdateSprintCommand(sprintId, dto);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(act, Throws.Exception.TypeOf<SprintOverlapException>());
    }
}
