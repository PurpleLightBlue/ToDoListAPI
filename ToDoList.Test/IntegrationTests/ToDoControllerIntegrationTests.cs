using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text;
using System.Text.Json;
using ToDoList.Application.DTOs;
using ToDoList.Application.Services;
using ToDoList.Domain.Interfaces;
using ToDoList.Domain.Models;

namespace ToDoList.Test.IntegrationTests
{
    public class ToDoControllerIntegrationTests : IClassFixture<WebApplicationFactory<Api.Program>>
    {
        private readonly HttpClient _client;
        private readonly Mock<IToDoRepository> _mockToDoRepository;

        public ToDoControllerIntegrationTests(WebApplicationFactory<Api.Program> factory)
        {
            _mockToDoRepository = new Mock<IToDoRepository>();
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_mockToDoRepository.Object);
                    services.AddScoped<IToDoService, ToDoService>(sp =>
                    {
                        var toDoRepository = sp.GetRequiredService<IToDoRepository>();
                        var memoryCache = sp.GetRequiredService<IMemoryCache>();
                        int cacheLifeSpan = 5; // Set the cache expiration time in minutes
                        return new ToDoService(toDoRepository, memoryCache, cacheLifeSpan);
                    });
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
            _mockToDoRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(toDoItems);

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
            _mockToDoRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ToDoItem)null);

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
            _mockToDoRepository.Setup(repo => repo.AddAsync(It.IsAny<ToDoItem>())).ReturnsAsync(toDoItem);

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
            _mockToDoRepository.Setup(repo => repo.UpdateAsync(It.IsAny<ToDoItem>())).Returns(Task.CompletedTask);

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
            _mockToDoRepository.Setup(repo => repo.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

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
            var toDoItems = new List<ToDoItem>
        {
            new ToDoItem { Id = 1, Text = "Test 1", IsCompleted = false },
            new ToDoItem { Id = 2, Text = "Shopping", IsCompleted = true }
        };
            _mockToDoRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(toDoItems);

            // Act
            var response = await _client.GetAsync("/api/todo/fuzzysearch?searchTerm=test");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IEnumerable<ToDoItemDto>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.Single(result);
        }
    }
}