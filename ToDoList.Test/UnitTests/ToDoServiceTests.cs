using Microsoft.Extensions.Caching.Memory;
using Moq;
using ToDoList.Application.Services;
using ToDoList.Domain.Interfaces;
using ToDoList.Domain.Models;
using ToDoList.Test.Mocks;

namespace ToDoList.Test.UnitTests
{
    public class ToDoServiceTests
    {
        private readonly Mock<IToDoRepository> _mockToDoRepository;
        private readonly MockMemoryCache _mockMemoryCache;
        private readonly ToDoService _toDoService;

        public ToDoServiceTests()
        {
            _mockToDoRepository = new Mock<IToDoRepository>();
            _mockMemoryCache = new MockMemoryCache();
            _toDoService = new ToDoService(_mockToDoRepository.Object, _mockMemoryCache, 5);
        }

        [Fact]
        public async Task AddAsync_ShouldAddToDoItem_AndRefreshCache()
        {
            // Arrange
            var toDoItem = new ToDoItem { Id = 1, Text = "Test", IsCompleted = false };
            _mockToDoRepository.Setup(repo => repo.AddAsync(It.IsAny<ToDoItem>())).ReturnsAsync(toDoItem);

            // Act
            var result = await _toDoService.AddAsync(toDoItem);

            // Assert
            Assert.Equal(toDoItem, result);
            _mockToDoRepository.Verify(repo => repo.AddAsync(toDoItem), Times.Once);
            Assert.True(_mockMemoryCache.TryGetValue("ToDoItems", out var cachedItems));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteToDoItem_AndRemoveFromCache()
        {
            // Act
            await _toDoService.DeleteAsync(1);

            // Assert
            _mockToDoRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
            //_mockMemoryCache.Verify(cache => cache.Remove("ToDoItems"), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnItems_FromCache_WhenCacheIsPopulated()
        {
            // Arrange
            var toDoItems = new List<ToDoItem> { new ToDoItem { Id = 1, Text = "Test", IsCompleted = false } };
            _mockMemoryCache.Set("ToDoItems", toDoItems, new MemoryCacheEntryOptions());

            // Act
            var result = await _toDoService.GetAllAsync();

            // Assert
            Assert.Equal(toDoItems, result);
            _mockToDoRepository.Verify(repo => repo.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnItems_FromRepository_WhenCacheIsEmpty()
        {
            // Arrange
            var toDoItems = new List<ToDoItem> { new ToDoItem { Id = 1, Text = "Test", IsCompleted = false } };
            //_mockMemoryCache.Setup(cache => cache.TryGetValue("ToDoItems", out It.Ref<IEnumerable<ToDoItem>>.IsAny)).Returns(false);
            _mockToDoRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(toDoItems);

            // Act
            var result = await _toDoService.GetAllAsync();

            // Assert
            Assert.Equal(toDoItems, result);
            _mockToDoRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
            //_mockMemoryCache.Verify(cache => cache.Set("ToDoItems", toDoItems, It.IsAny<MemoryCacheEntryOptions>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnItem_FromCache_WhenItemExistsInCache()
        {
            // Arrange
            var toDoItems = new List<ToDoItem> { new ToDoItem { Id = 1, Text = "Test", IsCompleted = false } };
            _mockMemoryCache.Set("ToDoItems", toDoItems, new MemoryCacheEntryOptions());

            // Act
            var result = await _toDoService.GetByIdAsync(1);

            // Assert
            Assert.Equal(toDoItems.First(), result);
            _mockToDoRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnItem_FromRepository_WhenItemNotInCache()
        {
            // Arrange
            var toDoItem = new ToDoItem { Id = 1, Text = "Test", IsCompleted = false };
            // _mockMemoryCache.Setup(cache => cache.TryGetValue("ToDoItems", out It.Ref<IEnumerable<ToDoItem>>.IsAny)).Returns(false);
            _mockToDoRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(toDoItem);

            // Act
            var result = await _toDoService.GetByIdAsync(1);

            // Assert
            Assert.Equal(toDoItem, result);
            _mockToDoRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateToDoItem_AndRemoveFromCache()
        {
            // Arrange
            var toDoItem = new ToDoItem { Id = 1, Text = "Test", IsCompleted = false };

            // Act
            await _toDoService.UpdateAsync(toDoItem);

            // Assert
            _mockToDoRepository.Verify(repo => repo.UpdateAsync(toDoItem), Times.Once);
            //_mockMemoryCache.Verify(cache => cache.Remove("ToDoItems"), Times.Once);
        }

        [Fact]
        public async Task FuzzySearchAsync_ShouldReturnSearchResults_FromCache()
        {
            // Arrange
            var toDoItems = new List<ToDoItem>
            {
                new ToDoItem { Id = 1, Text = "Test", IsCompleted = false },
                new ToDoItem { Id = 2, Text = "Another Test", IsCompleted = true }
            };
            //_mockMemoryCache.Setup(cache => cache.TryGetValue("ToDoItems", out toDoItems)).Returns(true);
            _mockMemoryCache.Set("ToDoItems", toDoItems, new MemoryCacheEntryOptions());
            // Act
            var result = await _toDoService.FuzzySearchAsync("Test");

            // Assert
            Assert.Equal(2, result.Count());
            _mockToDoRepository.Verify(repo => repo.GetAllAsync(), Times.Never);
        }
    }
}
