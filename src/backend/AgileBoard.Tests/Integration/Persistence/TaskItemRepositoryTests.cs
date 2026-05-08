using Microsoft.EntityFrameworkCore;
using AgileBoard.Adapters.Persistence;
using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Domain;

namespace AgileBoard.Tests.Integration.Persistence;

[TestFixture]
public class TaskItemRepositoryTests
{
    private AppDbContext _context = null!;
    private TaskItemRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);
        _repository = new TaskItemRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task TaskItemRepository_Add_ThenGetById_ReturnsTaskItem()
    {
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        _context.Sprints.Add(sprint);
        await _context.SaveChangesAsync();

        var task = TaskItem.Create("Task 1", "Desc", sprint.Id, ColumnType.New, 0);
        await _repository.AddAsync(task);
        var found = await _repository.GetByIdAsync(task.Id);

        Assert.That(found, Is.Not.Null);
        Assert.That(found!.Id, Is.EqualTo(task.Id));
        Assert.That(found.Name, Is.EqualTo("Task 1"));
    }

    [Test]
    public async Task TaskItemRepository_GetBySprintId_ReturnsOrderedByPosition()
    {
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        _context.Sprints.Add(sprint);
        await _context.SaveChangesAsync();

        var task2 = TaskItem.Create("Task B", "Desc", sprint.Id, ColumnType.New, 2);
        var task0 = TaskItem.Create("Task A", "Desc", sprint.Id, ColumnType.New, 0);
        var task1 = TaskItem.Create("Task C", "Desc", sprint.Id, ColumnType.New, 1);
        await _repository.AddAsync(task2);
        await _repository.AddAsync(task0);
        await _repository.AddAsync(task1);

        var tasks = await _repository.GetBySprintIdAsync(sprint.Id);

        Assert.That(tasks.Count, Is.EqualTo(3));
        Assert.That(tasks[0].Position, Is.EqualTo(0));
        Assert.That(tasks[1].Position, Is.EqualTo(1));
        Assert.That(tasks[2].Position, Is.EqualTo(2));
    }

    [Test]
    public async Task TaskItemRepository_Update_ThenGetById_ReturnsUpdated()
    {
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        _context.Sprints.Add(sprint);
        await _context.SaveChangesAsync();

        var task = TaskItem.Create("Task 1", "Desc", sprint.Id, ColumnType.New, 0);
        await _repository.AddAsync(task);

        task.Update("Updated", "New Desc");
        await _repository.UpdateAsync(task);
        var found = await _repository.GetByIdAsync(task.Id);

        Assert.That(found, Is.Not.Null);
        Assert.That(found!.Name, Is.EqualTo("Updated"));
        Assert.That(found.Description, Is.EqualTo("New Desc"));
    }

    [Test]
    public async Task TaskItemRepository_Delete_ThenGetById_ReturnsNull()
    {
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        _context.Sprints.Add(sprint);
        await _context.SaveChangesAsync();

        var task = TaskItem.Create("Task 1", "Desc", sprint.Id, ColumnType.New, 0);
        await _repository.AddAsync(task);

        await _repository.DeleteAsync(task.Id);
        var found = await _repository.GetByIdAsync(task.Id);

        Assert.That(found, Is.Null);
    }

    [Test]
    public async Task TaskItemRepository_GetMaxPosition_EmptyColumn_ReturnsMinusOne()
    {
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        _context.Sprints.Add(sprint);
        await _context.SaveChangesAsync();

        var maxPos = await _repository.GetMaxPositionAsync(sprint.Id, ColumnType.New);

        Assert.That(maxPos, Is.EqualTo(-1));
    }
}
