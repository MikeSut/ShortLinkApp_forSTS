using Microsoft.Extensions.Caching.Distributed;
using ShortLinks.Domain.Entity;

namespace ShortLinks.Application.Services;

public class CacheService
{
    IDistributedCache _cache;

    public CacheService(IDistributedCache distributedCache)
    {
        _cache = distributedCache;
    }

    public async Task AddUrl(Url? strUrl)
    {
        var diffTime = strUrl.ExpirationDate.Subtract(DateTime.Now);
        var seconds = 86400 * diffTime.Days + 3600 * diffTime.Hours + 60 * diffTime.Minutes + diffTime.Seconds;
        
        await _cache.SetStringAsync(strUrl.ShortUrl, strUrl.FullUrl, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(seconds)
        });
        
    }

    public async Task<string?> GetUrl(string shortUrl)
    {
        var urlString = await _cache.GetStringAsync(shortUrl);
        return urlString;    
    }
}