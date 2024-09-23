using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text;
using System.Text.Json;
using ToDoList.Application.DTOs;
using ToDoList.Domain.Interfaces;
using ToDoList.Domain.Models;

public class ToDoControllerEndpointTests : IClassFixture<WebApplicationFactory<ToDoList.Api.Program>>
{
    private readonly HttpClient _client;
    private readonly Mock<IToDoService> _mockToDoService;

    public ToDoControllerEndpointTests(WebApplicationFactory<ToDoList.Api.Program> factory)
    {
        _mockToDoService = new Mock<IToDoService>();
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mockToDoService.Object);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResult()
    {
        // Arrange
        var toDoItems = new List<ToDoItem>
        {
            new ToDoItem { Id = 1, Text = "Test 1", IsCompleted = false },
            new ToDoItem { Id = 2, Text = "Test 2", IsCompleted = true }
        };
        _mockToDoService.Setup(service => service.GetAllAsync()).ReturnsAsync(toDoItems);

        // Act
        var response = await _client.GetAsync("/api/todo");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IEnumerable<ToDoItemDto>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFoundResult_WhenRepositoryReturnsNull()
    {
        // Arrange
        _mockToDoService.Setup(service => service.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ToDoItem)null);

        // Act
        var response = await _client.GetAsync("/api/todo/1");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Add_ShouldReturnCreatedResult()
    {
        // Arrange
        var toDoItemCreate = new ToDoItemCreateDTO { Text = "New Task", IsCompleted = false };
        var toDoItem = new ToDoItem { Id = 1, Text = "New Task", IsCompleted = false };
        _mockToDoService.Setup(service => service.AddAsync(It.IsAny<ToDoItem>())).ReturnsAsync(toDoItem);

        // Act
        var content = new StringContent(JsonSerializer.Serialize(toDoItemCreate), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/todo", content);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContentResult()
    {
        // Arrange
        var toDoItemDto = new ToDoItemDto { Id = 1, Text = "Updated Task", IsCompleted = true };
        _mockToDoService.Setup(service => service.UpdateAsync(It.IsAny<ToDoItem>())).Returns(Task.CompletedTask);

        // Act
        var content = new StringContent(JsonSerializer.Serialize(toDoItemDto), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("/api/todo/1", content);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContentResult()
    {
        // Arrange
        _mockToDoService.Setup(service => service.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

        // Act
        var response = await _client.DeleteAsync("/api/todo/1");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task FuzzySearch_ShouldReturnOkResult()
    {
        // Arrange
        var searchResults = new List<ToDoItem>
        {
            new ToDoItem { Id = 1, Text = "Test 1", IsCompleted = false }
        };
        _mockToDoService.Setup(service => service.FuzzySearchAsync(It.IsAny<string>())).ReturnsAsync(searchResults);

        // Act
        var response = await _client.GetAsync("/api/todo/fuzzysearch?searchTerm=test");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IEnumerable<ToDoItemDto>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.Single(result);
    }
}
