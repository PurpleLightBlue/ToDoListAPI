using Microsoft.AspNetCore.Mvc;
using Moq;
using ToDoList.Api.Controllers;
using ToDoList.Application.DTOs;
using ToDoList.Domain.Interfaces;
using ToDoList.Domain.Models;

namespace ToDoList.Test.UnitTests
{
    public class ToDoControllerTests
    {
        private readonly Mock<IToDoService> _mockToDoService;
        private readonly ToDoController _controller;

        public ToDoControllerTests()
        {
            _mockToDoService = new Mock<IToDoService>();
            _controller = new ToDoController(_mockToDoService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfToDoItems()
        {
            // Arrange
            var toDoItems = new List<ToDoItem>
            {
                new ToDoItem { Id = 1, Text = "Test 1", IsCompleted = false },
                new ToDoItem { Id = 2, Text = "Test 2", IsCompleted = true }
            };
            _mockToDoService.Setup(service => service.GetAllAsync()).ReturnsAsync(toDoItems);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ToDoItemDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            _mockToDoService.Setup(service => service.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ToDoItem)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WithToDoItem()
        {
            // Arrange
            var toDoItem = new ToDoItem { Id = 1, Text = "Test 1", IsCompleted = false };
            _mockToDoService.Setup(service => service.GetByIdAsync(1)).ReturnsAsync(toDoItem);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ToDoItemDto>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task Add_ReturnsCreatedAtAction_WithCreatedToDoItem()
        {
            // Arrange
            var toDoItemCreate = new ToDoItemCreateDTO { Text = "Test 1", IsCompleted = false };
            var toDoItem = new ToDoItem { Id = 1, Text = "Test 1", IsCompleted = false };
            _mockToDoService.Setup(service => service.AddAsync(It.IsAny<ToDoItem>())).ReturnsAsync(toDoItem);

            // Act
            var result = await _controller.Add(toDoItemCreate);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<ToDoItem>(createdAtActionResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var toDoItemDto = new ToDoItemDto { Id = 1, Text = "Test 1", IsCompleted = false };

            // Act
            var result = await _controller.Update(2, toDoItemDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var toDoItemDto = new ToDoItemDto { Id = 1, Text = "Test 1", IsCompleted = false };

            // Act
            var result = await _controller.Update(1, toDoItemDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task FuzzySearch_ReturnsOkResult_WithSearchResults()
        {
            // Arrange
            var searchResults = new List<ToDoItem>
            {
                new ToDoItem { Id = 1, Text = "Test 1", IsCompleted = false }
            };
            _mockToDoService.Setup(service => service.FuzzySearchAsync(It.IsAny<string>())).ReturnsAsync(searchResults);

            // Act
            var result = await _controller.FuzzySearch("Test");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ToDoItem>>(okResult.Value);
            Assert.Single(returnValue);
        }
    }
}
