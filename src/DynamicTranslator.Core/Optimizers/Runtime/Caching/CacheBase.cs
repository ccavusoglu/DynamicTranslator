﻿namespace DynamicTranslator.Core.Optimizers.Runtime.Caching
{
    #region using

    using System;
    using System.Threading.Tasks;
    using Nito.AsyncEx;

    #endregion

    public abstract class CacheBase : ICache
    {
        protected readonly object SyncObj = new object();

        private readonly AsyncLock _asyncLock = new AsyncLock();

        protected CacheBase(string name)
        {
            Name = name;
            DefaultSlidingExpireTime = TimeSpan.FromHours(1);
        }

        public TimeSpan DefaultSlidingExpireTime { get; set; }

        public string Name { get; }

        public abstract void Clear();

        public virtual Task ClearAsync()
        {
            Clear();
            return Task.FromResult(0);
        }

        public virtual void Dispose()
        {
        }

        public virtual object Get(string key, Func<string, object> factory)
        {
            var cacheKey = key;
            var item = GetOrDefault(key);
            if (item == null)
            {
                lock (SyncObj)
                {
                    item = GetOrDefault(key);
                    if (item == null)
                    {
                        item = factory(key);
                        if (item == null)
                        {
                            return null;
                        }

                        Set(cacheKey, item);
                    }
                }
            }

            return item;
        }

        public virtual async Task<object> GetAsync(string key, Func<string, Task<object>> factory)
        {
            var cacheKey = key;
            var item = await GetOrDefaultAsync(key);
            if (item == null)
            {
                using (await _asyncLock.LockAsync())
                {
                    item = await GetOrDefaultAsync(key);
                    if (item == null)
                    {
                        item = await factory(key);
                        if (item == null)
                        {
                            return null;
                        }

                        await SetAsync(cacheKey, item);
                    }
                }
            }

            return item;
        }

        public abstract object GetOrDefault(string key);

        public virtual Task<object> GetOrDefaultAsync(string key)
        {
            return Task.FromResult(GetOrDefault(key));
        }

        public abstract void Remove(string key);

        public virtual Task RemoveAsync(string key)
        {
            Remove(key);
            return Task.FromResult(0);
        }

        public abstract void Set(string key, object value, TimeSpan? slidingExpireTime = null);

        public virtual Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null)
        {
            Set(key, value, slidingExpireTime);
            return Task.FromResult(0);
        }
    }
}