using Microsoft.Extensions.Caching.Memory;
using ToDoList.Domain.Interfaces;
using ToDoList.Domain.Models;

namespace ToDoList.Application.Services
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey = "ToDoItems";
        private readonly int _cacheLifeSpan = 5;

        public ToDoService(IToDoRepository toDoRepository, IMemoryCache memoryCache, int cacheLifeSpan)
        {
            _toDoRepository = toDoRepository;
            _cache = memoryCache;
        }

        public async Task<ToDoItem> AddAsync(ToDoItem toDoItem)
        {
            var createdItem = await _toDoRepository.AddAsync(toDoItem);

            // Refresh cache
            await RefreshCacheAsync();

            return createdItem;
        }

        public async Task DeleteAsync(int id)
        {
            await _toDoRepository.DeleteAsync(id);

            // Refresh cache
            _cache.Remove(_cacheKey);
        }

        public async Task<IEnumerable<ToDoItem>> GetAllAsync()
        {
            if (!_cache.TryGetValue(_cacheKey, out IEnumerable<ToDoItem> cachedToDoItems))
            {
                // Cache miss: fetch from database
                cachedToDoItems = await RefreshCacheAsync();
            }

            return cachedToDoItems;
        }

        public async Task<ToDoItem?> GetByIdAsync(int id)
        {
            // Try to get the entire list from the cache
            if (_cache.TryGetValue(_cacheKey, out IEnumerable<ToDoItem> cachedToDoItems))
            {
                // Search for the item in the cached list
                var toDoItem = cachedToDoItems.FirstOrDefault(item => item.Id == id);

                if (toDoItem != null)
                {
                    return toDoItem;  // Return the item if found in the cache
                }
            }

            // If the item is not in the cache, fallback to the repository
            var fetchedToDoItem = await _toDoRepository.GetByIdAsync(id);

            return fetchedToDoItem;
        }

        public async Task UpdateAsync(ToDoItem toDoItem)
        {
            await _toDoRepository.UpdateAsync(toDoItem);

            // refresh the cache
            _cache.Remove(_cacheKey);
        }

        // This method is responsible for refreshing the cache
        private async Task<IEnumerable<ToDoItem>> RefreshCacheAsync()
        {
            var freshData = await _toDoRepository.GetAllAsync();
            _cache.Set(_cacheKey, freshData, TimeSpan.FromMinutes(_cacheLifeSpan));

            return freshData;
        }

        public async Task<IEnumerable<ToDoItem>> FuzzySearchAsync(string searchTerm)
        {
            // Check if the cache is populated
            if (!_cache.TryGetValue(_cacheKey, out IEnumerable<ToDoItem> cachedItems))
            {
                // Cache is empty, so populate it from the database
                cachedItems = await RefreshCacheAsync();
            }

            // Perform the search on the cached items
            var searchResults = cachedItems
                .Where(item => !string.IsNullOrEmpty(item.Text) &&
                               item.Text.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            return searchResults;
        }
    }
}
