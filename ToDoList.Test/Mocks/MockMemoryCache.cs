using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace ToDoList.Test.Mocks
{
    public class MockMemoryCache : IMemoryCache
    {
        private readonly Dictionary<object, object> _cache = new Dictionary<object, object>();

        public ICacheEntry CreateEntry(object key)
        {
            var entry = new Mock<ICacheEntry>();
            entry.SetupAllProperties();
            entry.Object.Value = key;
            _cache[key] = entry.Object;
            return entry.Object;
        }

        public void Dispose() { }

        public void Remove(object key)
        {
            _cache.Remove(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void Set<TItem>(object key, TItem value, MemoryCacheEntryOptions options)
        {
            _cache[key] = value;
        }
    }
}