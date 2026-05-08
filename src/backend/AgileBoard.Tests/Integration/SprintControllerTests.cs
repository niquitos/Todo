using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AgileBoard.Adapters.Persistence;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Domain;

namespace AgileBoard.Tests.Integration;

[TestFixture]
public class SprintControllerTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _dbName = null!;

    [SetUp]
    public void SetUp()
    {
        _dbName = $"TestDb_{Guid.NewGuid()}";
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase(_dbName));
                });
            });
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private void SeedData(params Sprint[] sprints)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Sprints.AddRange(sprints);
        context.SaveChanges();
    }

    [Test]
    public async Task GET_api_sprints_EmptyDatabase_ReturnsOnlyDefaultSprint()
    {
        // Act
        var response = await _client.GetAsync("/api/sprints");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<SprintDto>>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Count, Is.EqualTo(1));
        Assert.That(result[0].IsDefault, Is.True);
    }

    [Test]
    public async Task GET_api_sprints_WithSprints_ReturnsArray()
    {
        // Arrange
        var sprint1 = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        var sprint2 = Sprint.Create("Sprint 2", DateTime.UtcNow.AddDays(15), DateTime.UtcNow.AddDays(29));
        SeedData(sprint1, sprint2);

        // Act
        var response = await _client.GetAsync("/api/sprints");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<SprintDto>>();
        Assert.That(result!.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task GET_api_sprints_id_ExistingId_ReturnsSprintWithColumns()
    {
        // Arrange
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14), "Description");
        SeedData(sprint);

        // Act
        var response = await _client.GetAsync($"/api/sprints/{sprint.Id.Value}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<SprintDto>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(sprint.Id.Value));
        Assert.That(result.Name, Is.EqualTo("Sprint 1"));
    }

    [Test]
    public async Task POST_api_sprints_ValidData_Returns201Created()
    {
        // Arrange
        var dto = new CreateSprintDto("Sprint 1", new DateTime(2026, 1, 1), new DateTime(2026, 1, 14), "Test");

        // Act
        var response = await _client.PostAsJsonAsync("/api/sprints", dto);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var result = await response.Content.ReadFromJsonAsync<SprintDto>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Sprint 1"));
    }

    [Test]
    public async Task POST_api_sprints_OverlappingDates_Returns400BadRequest()
    {
        // Arrange
        var existingSprint = Sprint.Create("Existing", new DateTime(2026, 1, 1), new DateTime(2026, 1, 14));
        SeedData(existingSprint);

        var dto = new CreateSprintDto("Overlap Sprint", new DateTime(2026, 1, 10), new DateTime(2026, 1, 20), "Test");

        // Act
        var response = await _client.PostAsJsonAsync("/api/sprints", dto);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task PUT_api_sprints_id_ValidData_Returns204NoContent()
    {
        // Arrange
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14), "Description");
        SeedData(sprint);

        var dto = new UpdateSprintDto("Updated Name", sprint.StartDate, sprint.EndDate, "Updated Description");

        // Act
        var response = await _client.PutAsJsonAsync($"/api/sprints/{sprint.Id.Value}", dto);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DELETE_api_sprints_id_ExistingId_Returns204NoContent()
    {
        // Arrange
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        SeedData(sprint);

        // Act
        var response = await _client.DeleteAsync($"/api/sprints/{sprint.Id.Value}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GET_api_sprints_id_NonExistingId_Returns404NotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/sprints/{Guid.NewGuid()}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
