using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AgileBoard.Adapters.Persistence;
using AgileBoard.Application.UseCases.Tasks;
using AgileBoard.Domain;

namespace AgileBoard.Tests.Integration;

[TestFixture]
public class TaskItemControllerTests
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
    public async Task GET_tasks_EmptySprint_ReturnsEmptyArray()
    {
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        SeedData(sprint);

        var response = await _client.GetAsync($"/api/tasks?sprintId={sprint.Id.Value}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>();
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task POST_tasks_ValidData_Returns201Created()
    {
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        SeedData(sprint);

        var dto = new CreateTaskItemDto { Name = "Task 1", Description = "Desc", ColumnType = "New", SprintId = sprint.Id.Value };

        var response = await _client.PostAsJsonAsync($"/api/tasks", dto);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var result = await response.Content.ReadFromJsonAsync<TaskItemDto>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Task 1"));
        Assert.That(result.ColumnType, Is.EqualTo("New"));
        Assert.That(result.Position, Is.EqualTo(0));
    }

    [Test]
    public async Task PUT_tasks_update_ValidData_Returns204()
    {
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        SeedData(sprint);

        var createDto = new CreateTaskItemDto { Name = "Task 1", Description = "Desc", ColumnType = "New" };
        var createResponse = await _client.PostAsJsonAsync($"/api/tasks", createDto);
        var created = (await createResponse.Content.ReadFromJsonAsync<TaskItemDto>())!;

        var updateDto = new UpdateTaskItemDto { Name = "Updated Task", Description = "Updated Desc", ColumnType = "New", Position = 0 };

        var response = await _client.PutAsJsonAsync($"/api/tasks/{created.Id}", updateDto);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DELETE_tasks_ExistingId_Returns204()
    {
        var sprint = Sprint.Create("Sprint 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        SeedData(sprint);

        var createDto = new CreateTaskItemDto { Name = "Task 1", Description = "Desc", ColumnType = "New" };
        var createResponse = await _client.PostAsJsonAsync($"/api/tasks", createDto);
        var created = (await createResponse.Content.ReadFromJsonAsync<TaskItemDto>())!;

        var response = await _client.DeleteAsync($"/api/tasks/{created.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }
}
