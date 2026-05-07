using Microsoft.EntityFrameworkCore;
using AgileBoard.Adapters.Persistence;
using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Domain;

namespace AgileBoard.Tests.Integration.Persistence;

[TestFixture]
public class SprintRepositoryTests
{
    private AppDbContext _context = null!;
    private SprintRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);
        _repository = new SprintRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task SprintRepository_Add_ThenGetById_ReturnsSprint()
    {
        // Arrange
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14), "Description");

        // Act
        await _repository.AddAsync(sprint);
        var found = await _repository.GetByIdAsync(sprint.Id);

        // Assert
        Assert.That(found, Is.Not.Null);
        Assert.That(found!.Id, Is.EqualTo(sprint.Id));
        Assert.That(found.Name, Is.EqualTo(sprint.Name));
    }

    [Test]
    public async Task SprintRepository_GetAll_ReturnsAllSprints()
    {
        // Arrange
        var sprint1 = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        var sprint2 = Sprint.Create("Sprint 2", DateTime.UtcNow.AddDays(15), DateTime.UtcNow.AddDays(29));
        var sprint3 = Sprint.Create("Sprint 3", DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddDays(44));

        await _repository.AddAsync(sprint1);
        await _repository.AddAsync(sprint2);
        await _repository.AddAsync(sprint3);

        // Act
        var all = await _repository.GetAllAsync();

        // Assert
        Assert.That(all.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task SprintRepository_Update_ThenGetById_ReturnsUpdated()
    {
        // Arrange
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14), "Description");
        await _repository.AddAsync(sprint);

        var newStart = DateTime.UtcNow.AddDays(20);
        var newEnd = DateTime.UtcNow.AddDays(34);

        // Act
        sprint.Update("Updated", newStart, newEnd, "Updated Description");
        await _repository.UpdateAsync(sprint);
        var found = await _repository.GetByIdAsync(sprint.Id);

        // Assert
        Assert.That(found, Is.Not.Null);
        Assert.That(found!.Name, Is.EqualTo("Updated"));
        Assert.That(found.Description, Is.EqualTo("Updated Description"));
    }

    [Test]
    public async Task SprintRepository_Delete_ThenGetById_ReturnsNull()
    {
        // Arrange
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        await _repository.AddAsync(sprint);

        // Act
        await _repository.DeleteAsync(sprint.Id);
        var found = await _repository.GetByIdAsync(sprint.Id);

        // Assert
        Assert.That(found, Is.Null);
    }
}
