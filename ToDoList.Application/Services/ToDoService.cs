using Microsoft.Extensions.Caching.Memory;
using ToDoList.Domain.Interfaces;
using ToDoList.Domain.Models;

namespace ToDoList.Application.Services
{
    /// <summary>
    /// Service for managing to-do items.
    /// </summary>
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey = "ToDoItems";
        private readonly int _cacheLifeSpan = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToDoService"/> class.
        /// </summary>
        /// <param name="toDoRepository">The to-do repository.</param>
        /// <param name="memoryCache">The memory cache.</param>
        /// <param name="cacheLifeSpan">The cache lifespan in minutes.</param>
        public ToDoService(IToDoRepository toDoRepository, IMemoryCache memoryCache, int cacheLifeSpan)
        {
            _toDoRepository = toDoRepository;
            _cache = memoryCache;
        }

        /// <summary>
        /// Adds a new to-do item.
        /// </summary>
        /// <param name="toDoItem">The to-do item to add.</param>
        /// <returns>The added to-do item.</returns>
        public async Task<ToDoItem> AddAsync(ToDoItem toDoItem)
        {
            var createdItem = await _toDoRepository.AddAsync(toDoItem);

            // Refresh cache
            await RefreshCacheAsync();

            return createdItem;
        }

        /// <summary>
        /// Deletes a to-do item by ID.
        /// </summary>
        /// <param name="id">The ID of the to-do item to delete.</param>
        public async Task DeleteAsync(int id)
        {
            await _toDoRepository.DeleteAsync(id);

            // Refresh cache
            _cache.Remove(_cacheKey);
        }

        /// <summary>
        /// Gets all to-do items.
        /// </summary>
        /// <returns>A list of to-do items.</returns>
        public async Task<IEnumerable<ToDoItem>> GetAllAsync()
        {
            if (!_cache.TryGetValue(_cacheKey, out IEnumerable<ToDoItem> cachedToDoItems))
            {
                // Cache miss: fetch from database
                cachedToDoItems = await RefreshCacheAsync();
            }

            return cachedToDoItems;
        }

        /// <summary>
        /// Gets a to-do item by ID.
        /// </summary>
        /// <param name="id">The ID of the to-do item.</param>
        /// <returns>The to-do item, or null if not found.</returns>
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

        /// <summary>
        /// Updates an existing to-do item.
        /// </summary>
        /// <param name="toDoItem">The to-do item to update.</param>
        public async Task UpdateAsync(ToDoItem toDoItem)
        {
            await _toDoRepository.UpdateAsync(toDoItem);

            // refresh the cache
            _cache.Remove(_cacheKey);
        }

        /// <summary>
        /// Refreshes the cache with the latest to-do items.
        /// </summary>
        /// <returns>A list of the latest to-do items.</returns>
        private async Task<IEnumerable<ToDoItem>> RefreshCacheAsync()
        {
            var freshData = await _toDoRepository.GetAllAsync();
            _cache.Set(_cacheKey, freshData, TimeSpan.FromMinutes(_cacheLifeSpan));

            return freshData;
        }

        /// <summary>
        /// Performs a fuzzy search for to-do items.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <returns>A list of to-do items matching the search term.</returns>
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
