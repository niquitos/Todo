using AgileBoard.Domain;

namespace AgileBoard.Tests.Unit.Domain;

[TestFixture]
public class SprintTests
{
    [Test]
    public void Sprint_Create_WithValidData_AssignsId()
    {
        // Arrange
        var name = "Sprint 1";
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(14);

        // Act
        var sprint = Sprint.Create(name, startDate, endDate, "Description");

        // Assert
        Assert.That(sprint.Id, Is.Not.EqualTo(SprintId.New()));
        Assert.That(sprint.Name, Is.EqualTo(name));
        Assert.That(sprint.Description, Is.EqualTo("Description"));
    }

    [Test]
    public void Sprint_OverlapsWith_NonOverlappingRange_ReturnsFalse()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 1, 14, 0, 0, 0, DateTimeKind.Utc);
        var sprint = Sprint.Create("Sprint 1", startDate, endDate);

        // Act
        var overlaps = sprint.OverlapsWith(new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                                           new DateTime(2026, 1, 28, 0, 0, 0, DateTimeKind.Utc));

        // Assert
        Assert.That(overlaps, Is.False);
    }

    [Test]
    public void Sprint_OverlapsWith_OverlappingRange_ReturnsTrue()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 1, 14, 0, 0, 0, DateTimeKind.Utc);
        var sprint = Sprint.Create("Sprint 1", startDate, endDate);

        // Act
        var overlaps = sprint.OverlapsWith(new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                                           new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc));

        // Assert
        Assert.That(overlaps, Is.True);
    }

    [Test]
    public void Sprint_Update_ChangesProperties()
    {
        // Arrange
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14), "Description");
        var newStart = DateTime.UtcNow.AddDays(20);
        var newEnd = DateTime.UtcNow.AddDays(34);

        // Act
        sprint.Update("New Name", newStart, newEnd, "New Description");

        // Assert
        Assert.That(sprint.Name, Is.EqualTo("New Name"));
        Assert.That(sprint.Description, Is.EqualTo("New Description"));
        Assert.That(sprint.StartDate, Is.EqualTo(newStart));
        Assert.That(sprint.EndDate, Is.EqualTo(newEnd));
    }
}
