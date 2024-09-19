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

            // Invalidate cache after adding a new item
            //_cache.Remove(_cacheKey);
            await RefreshCacheAsync();

            return createdItem;
        }

        public async Task DeleteAsync(int id)
        {
            await _toDoRepository.DeleteAsync(id);

            // Invalidate cache after delete
            _cache.Remove(_cacheKey);
        }

        public async Task<IEnumerable<ToDoItem>> GetAllAsync()
        {
            if (!_cache.TryGetValue(_cacheKey, out IEnumerable<ToDoItem> cachedToDoItems))
            {
                // Cache miss: fetch from database
                cachedToDoItems = await _toDoRepository.GetAllAsync();


                await RefreshCacheAsync();
                // Set cache options
                //var cacheOptions = new MemoryCacheEntryOptions
                //{
                //    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache for 5 minutes
                //};

                //// Save in cache
                //_cache.Set(_cacheKey, cachedToDoItems, cacheOptions);
            }

            return cachedToDoItems;
        }

        public async Task<ToDoItem?> GetByIdAsync(int id)
        {
            return await _toDoRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(ToDoItem toDoItem)
        {
            await _toDoRepository.UpdateAsync(toDoItem);

            // Invalidate cache after update
            _cache.Remove(_cacheKey);
        }

        // This method is responsible for refreshing the cache
        private async Task<IEnumerable<ToDoItem>> RefreshCacheAsync()
        {
            var freshData = await _toDoRepository.GetAllAsync();
            _cache.Set(_cacheKey, freshData, TimeSpan.FromMinutes(_cacheLifeSpan));

            return freshData;
        }
    }
}
