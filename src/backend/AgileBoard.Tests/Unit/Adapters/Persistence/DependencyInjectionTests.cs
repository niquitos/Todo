using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AgileBoard.Adapters.Persistence;

namespace AgileBoard.Tests.Unit.Adapters.Persistence;

[TestFixture]
public class DependencyInjectionTests
{
    [Test]
    public void AddPersistence_WithConnectionString_RegistersDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var connectionString = "Host=localhost;Database=test;Username=postgres;Password=pass";

        // Act
        services.AddPersistence(connectionString);
        var provider = services.BuildServiceProvider();
        var context = provider.GetService<AppDbContext>();

        // Assert
        Assert.That(context, Is.Not.Null);
    }

    [Test]
    public void AddPersistence_WithConfiguration_RegistersDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var inMemorySettings = new Dictionary<string, string>
        {
            { "ConnectionStrings:DefaultConnection", "Host=localhost;Database=test;Username=postgres;Password=pass" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        // Act
        services.AddPersistence(configuration);
        var provider = services.BuildServiceProvider();
        var context = provider.GetService<AppDbContext>();

        // Assert
        Assert.That(context, Is.Not.Null);
    }
}
