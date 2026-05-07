using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Domain;
using Moq;

namespace AgileBoard.Tests.Unit.UseCases.Sprints;

[TestFixture]
public class DeleteSprintCommandTests
{
    private Mock<ISprintRepository> _repositoryMock = null!;
    private DeleteSprintCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ISprintRepository>();
        _handler = new DeleteSprintCommandHandler(_repositoryMock.Object);
    }

    [Test]
    public async Task DeleteSprintCommand_ExistingId_DeletesSprint()
    {
        // Arrange
        var sprintId = SprintId.New();
        _repositoryMock.Setup(r => r.GetByIdAsync(sprintId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14)));

        var command = new DeleteSprintCommand(sprintId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(sprintId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
