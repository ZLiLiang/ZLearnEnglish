using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace Z.ASPNETCore
{
    /// <summary>
    /// 分布式内存缓存帮助接口
    /// </summary>
    public interface IDistributedCacheHelper
    {
        TResult? GetOrCreate<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, TResult?> valueFactory, int expireSeconds = 60);

        Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey,Func<DistributedCacheEntryOptions,Task<TResult?>> valueFactory,int expireSeconds = 60);

        void Remove(string cacheKey);
        Task RemoveAsync(string cacheKey);
    }
}
