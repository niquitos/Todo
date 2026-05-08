using AgileBoard.Domain;

namespace AgileBoard.Tests.Unit.Domain;

[TestFixture]
public class TaskItemTests
{
    [Test]
    public void TaskItem_Create_WithValidData_AssignsId()
    {
        var sprintId = SprintId.New();
        var task = TaskItem.Create("Task 1", "Description", sprintId, ColumnType.New, 0);

        Assert.That(task.Id.Value, Is.Not.EqualTo(Guid.Empty));
        Assert.That(task.Name, Is.EqualTo("Task 1"));
        Assert.That(task.Description, Is.EqualTo("Description"));
        Assert.That(task.SprintId, Is.EqualTo(sprintId));
        Assert.That(task.ColumnType, Is.EqualTo(ColumnType.New));
        Assert.That(task.Position, Is.EqualTo(0));
    }

    [Test]
    public void TaskItem_Update_ChangesNameAndDescription()
    {
        var task = TaskItem.Create("Task 1", "Description", SprintId.New(), ColumnType.New, 0);

        task.Update("New Name", "New Description");

        Assert.That(task.Name, Is.EqualTo("New Name"));
        Assert.That(task.Description, Is.EqualTo("New Description"));
    }

    [Test]
    public void TaskItem_Move_ChangesColumnAndPosition()
    {
        var task = TaskItem.Create("Task 1", "Description", SprintId.New(), ColumnType.New, 0);

        task.Move(ColumnType.InProgress, 2);

        Assert.That(task.ColumnType, Is.EqualTo(ColumnType.InProgress));
        Assert.That(task.Position, Is.EqualTo(2));
    }
}
