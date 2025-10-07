namespace LibraryManagementRedis.Infrastructure.Caching;
public class CacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDatabase _cache = redis.GetDatabase();
    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _cache.StringGetAsync(key);
        if (value.IsNullOrEmpty) return default;
        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.KeyDeleteAsync(key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _cache.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(5));
    }
}
