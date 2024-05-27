using Microsoft.Extensions.Caching.Distributed;

namespace Sterling.Gateway.Data;

public class RedisRepository : IRedisRepository
{
    private readonly IDistributedCache redisCache;

    public RedisRepository(IDistributedCache redisCache)
    {
        this.redisCache = redisCache;
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

            return null!;
        }
    }

    public async Task Delete(string key)
    {
        try
        {
            await redisCache.RemoveAsync(key);
        }
        catch (System.Exception ex)
        {
            
            throw;
        }
    }
}
