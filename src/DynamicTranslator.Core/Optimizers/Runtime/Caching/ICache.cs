﻿namespace DynamicTranslator.Core.Optimizers.Runtime.Caching
{
    #region using

    using System;
    using System.Threading.Tasks;

    #endregion

    public interface ICache : IDisposable
    {
        /// <summary>
        ///     Default sliding expire time of cache items.
        ///     Default value: 60 minutes. Can be changed by configuration.
        /// </summary>
        TimeSpan DefaultSlidingExpireTime { get; set; }

        /// <summary>
        ///     Unique name of the cache.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Clears all items in this cache.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Clears all items in this cache.
        /// </summary>
        Task ClearAsync();

        /// <summary>
        ///     Gets an item from the cache.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached item</returns>
        object Get(string key, Func<string, object> factory);

        /// <summary>
        ///     Gets an item from the cache.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached item</returns>
        Task<object> GetAsync(string key, Func<string, Task<object>> factory);

        /// <summary>
        ///     Gets an item from the cache or null if not found.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Cached item or null if not found</returns>
        object GetOrDefault(string key);

        /// <summary>
        ///     Gets an item from the cache or null if not found.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Cached item or null if not found</returns>
        Task<object> GetOrDefaultAsync(string key);

        /// <summary>
        ///     Removes a cache item by it's key.
        /// </summary>
        /// <param name="key">Key</param>
        void Remove(string key);

        /// <summary>
        ///     Removes a cache item by it's key (does nothing if given key does not exists in the cache).
        /// </summary>
        /// <param name="key">Key</param>
        Task RemoveAsync(string key);

        /// <summary>
        ///     Saves/Overrides an item in the cache by a key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        void Set(string key, object value, TimeSpan? slidingExpireTime = null);

        /// <summary>
        ///     Saves/Overrides an item in the cache by a key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null);
    }
}