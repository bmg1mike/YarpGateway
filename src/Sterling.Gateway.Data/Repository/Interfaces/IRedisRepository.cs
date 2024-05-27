namespace Sterling.Gateway.Data;

public interface IRedisRepository
{
    Task<string> Get(string key);
    Task<string> AddOrUpdate(string key, string value);
    Task Delete(string key);
}
