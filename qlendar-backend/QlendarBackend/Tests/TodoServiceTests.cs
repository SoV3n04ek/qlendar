using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.EntityFrameworkCore;
using QlendarBackend.Qlendar.Models;
using QlendarBackend.Qlendar.Services;
using QlendarBackend.Qlendar.Infrastructure;

namespace Qlendar.Tests.Unit.Services;

public class TodoServiceTests : IDisposable
{
    private readonly TodoService _todoService;
    private readonly AppDbContext _dbContext;

    public TodoServiceTests()
    {
        // Using InMemoryDatabase for isolated testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name for each test
            .Options;

        _dbContext = new AppDbContext(options);
        _todoService = new TodoService(_dbContext);
    }

    [Fact]
    public async Task GetTodoByIdAsync_ShouldReturnTodo_WhenExistsAndUserHasAccess()
    {
        // Arrange - Prepare test data
        const string testUserId = "user-1";
        var testTodo = new Todo
        {
            Id = 1,
            UserId = testUserId,
            Title = "Test Todo"
        };

        await _dbContext.Todos.AddAsync(testTodo);
        await _dbContext.SaveChangesAsync();

        // Act - Call the method under test
        var result = await _todoService.GetTodoByIdAsync(1, testUserId);

        // Assert - Verify the results
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Test Todo");
    }

    [Fact]
    public async Task GetTodoByIdAsync_ShouldThrowKeyNotFoundException_WhenTodoDoesNotExist()
    {
        // Arrange - No test data needed
        const string testUserId = "user-1";
        const int nonExistentTodoId = 999;

        // Act & Assert - Verify exception is thrown
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _todoService.GetTodoByIdAsync(nonExistentTodoId, testUserId)
        );
    }

    [Fact]
    public async Task GetTodoByIdAsync_ShouldThrowKeyNotFoundException_WhenUserHasNoAccess()
    {
        // Arrange - Setup owned todo and unauthorized user
        const string ownerId = "user-1";
        const string unauthorizedUserId = "user-2";
        var testTodo = new Todo
        {
            Id = 1,
            UserId = ownerId,
            Title = "Private Todo"
        };

        await _dbContext.Todos.AddAsync(testTodo);
        await _dbContext.SaveChangesAsync();

        // Act & Assert - Verify unauthorized access throws
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _todoService.GetTodoByIdAsync(1, unauthorizedUserId)
        );
    }

    public void Dispose()
    {
        // Clean up database after each test
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}