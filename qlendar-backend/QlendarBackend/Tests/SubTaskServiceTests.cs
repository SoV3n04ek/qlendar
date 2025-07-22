using Xunit;
using FluentAssertions;
using Moq;
using QlendarBackend.Qlendar.Services;
using QlendarBackend.Qlendar.Models;
using Microsoft.EntityFrameworkCore;
using QlendarBackend.Qlendar.Infrastructure;

namespace Qlendar.Tests.Unit.Services;

public class SubTaskServiceTests : IDisposable
{
    private readonly SubTaskService _service;
    private readonly AppDbContext _context;

    public SubTaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new SubTaskService(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddSubTask()
    {
        // Arrange
        var subTask = new SubTask { Title = "Test Subtask" };

        // Act
        var result = await _service.CreateAsync(subTask);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        _context.SubTasks.Should().Contain(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingSubTask()
    {
        // Arrange
        var existing = new SubTask { Title = "Original" };
        _context.SubTasks.Add(existing);
        await _context.SaveChangesAsync();

        var update = new SubTask { Id = existing.Id, Title = "Updated" };

        // Act
        var result = await _service.UpdateAsync(update);

        // Assert
        result.Title.Should().Be("Updated");
    }

    public void Dispose() => _context.Dispose();
}