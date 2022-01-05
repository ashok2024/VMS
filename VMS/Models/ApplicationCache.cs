using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace VMS.Models
{
    public enum AppCachePriority
    {
        Default,
        NotRemovable
    }

    public class ApplicationCache
    {
        private static ObjectCache cache = MemoryCache.Default;
        private CacheItemPolicy policy = null;
        private CacheEntryRemovedCallback callback = null;

        public void AddtoCache(string CacheKeyName, Object CacheItem, AppCachePriority AppCacheItemPriority, double? seconds)
        {
            callback = new CacheEntryRemovedCallback(this.MyCachedItemRemovedCallback);
            policy = new CacheItemPolicy();
            policy.Priority = (AppCacheItemPriority == AppCachePriority.Default) ? CacheItemPriority.Default : CacheItemPriority.NotRemovable;
            if (seconds != null)
                //policy.AbsoluteExpiration = DateTime.Now.AddSeconds(Convert.ToDouble(seconds));
                policy.SlidingExpiration = TimeSpan.FromDays(1);
            policy.RemovedCallback = callback;
            if (CacheKeyName != "" && CacheItem != null)
                cache.Set(CacheKeyName, CacheItem, policy);
        }
        public Object GetMyCachedItem(String CacheKeyName)
        {
            // 
            return cache[CacheKeyName] as Object;
        }

        public void RemoveMyCachedItem(String CacheKeyName)
        {
            // 
            if (CacheKeyName != null)
            {
                if (cache.Contains(CacheKeyName))
                {
                    cache.Remove(CacheKeyName);
                }
            }

        }
        private void MyCachedItemRemovedCallback(CacheEntryRemovedArguments arguments)
        {
            // Log these values from arguments list 
            String strLog = String.Concat("Reason: ", arguments.RemovedReason.ToString(), " | Key-Name: ", arguments.CacheItem.Key, " | Value-Object: ", arguments.CacheItem.Value.ToString());
        }

    }
}