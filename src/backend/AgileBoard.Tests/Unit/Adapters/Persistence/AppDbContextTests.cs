using Microsoft.EntityFrameworkCore;
using AgileBoard.Adapters.Persistence;

namespace AgileBoard.Tests.Unit.Adapters.Persistence;

[TestFixture]
public class AppDbContextTests
{
    [Test]
    public void AppDbContext_Creates_Successfully()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("Test")
            .Options;

        // Act
        var context = new AppDbContext(options);

        // Assert
        Assert.That(context, Is.Not.Null);
        Assert.That(context.Database, Is.Not.Null);
    }
}
