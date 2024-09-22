using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Models;
using ToDoList.Infrastructure;
using ToDoList.Infrastructure.Repositories;

public class ToDoRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;

    public ToDoRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "ToDoListTestDb")
            .Options;
    }

    private AppDbContext CreateDbContext()
    {
        var context = new AppDbContext(_dbContextOptions);
        context.Database.EnsureDeleted(); // Clear the database before each test
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task AddAsync_ShouldAddToDoItem()
    {
        // Arrange
        var context = CreateDbContext();
        var repository = new ToDoRepository(context);
        var toDoItem = new ToDoItem { Id = 1, Text = "Test", IsCompleted = false };

        // Act
        var result = await repository.AddAsync(toDoItem);

        // Assert
        Assert.Equal(toDoItem, result);
        Assert.Equal(1, await context.ToDoItems.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteToDoItem()
    {
        // Arrange
        var context = CreateDbContext();
        var repository = new ToDoRepository(context);
        var toDoItem = new ToDoItem { Id = 1, Text = "Test", IsCompleted = false };
        context.ToDoItems.Add(toDoItem);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(1);

        // Assert
        Assert.Equal(0, await context.ToDoItems.CountAsync());
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllToDoItems()
    {
        // Arrange
        var context = CreateDbContext();
        var repository = new ToDoRepository(context);
        var toDoItems = new List<ToDoItem>
        {
            new ToDoItem { Id = 1, Text = "Test 1", IsCompleted = false },
            new ToDoItem { Id = 2, Text = "Test 2", IsCompleted = true }
        };
        context.ToDoItems.AddRange(toDoItems);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnToDoItem()
    {
        // Arrange
        var context = CreateDbContext();
        var repository = new ToDoRepository(context);
        var toDoItem = new ToDoItem { Id = 1, Text = "Test", IsCompleted = false };
        context.ToDoItems.Add(toDoItem);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Equal(toDoItem, result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateToDoItem()
    {
        // Arrange
        var context = CreateDbContext();
        var repository = new ToDoRepository(context);
        var toDoItem = new ToDoItem { Id = 1, Text = "Test", IsCompleted = false };
        context.ToDoItems.Add(toDoItem);
        await context.SaveChangesAsync();

        // Act
        toDoItem.Text = "Updated Test";
        await repository.UpdateAsync(toDoItem);

        // Assert
        var updatedItem = await context.ToDoItems.FindAsync(1);
        Assert.Equal("Updated Test", updatedItem.Text);
    }
}

