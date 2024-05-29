using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Sterling.Gateway.Data;

public class RedisRepository : IRedisRepository
{
    private readonly IDistributedCache redisCache;
    private readonly ILogger<RedisRepository> logger;

    public RedisRepository(IDistributedCache redisCache, ILogger<RedisRepository> logger)
    {
        this.redisCache = redisCache;
        this.logger = logger;
    }

    public async Task<string> Get(string key)
    {
        var value = await redisCache.GetStringAsync(key);

        if (String.IsNullOrEmpty(value))
            return null!;

        return value;
    }
    public async Task<string> AddOrUpdate(string key, string value)
    {
        try
        {
            
            await redisCache.SetStringAsync(key, value);

            return await Get(key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,ex.Message);
            return string.Empty;
        }
    }

    public async Task Delete(string key)
    {
        try
        {
            await redisCache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,ex.Message);
        }
    }
}
