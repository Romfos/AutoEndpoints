using StackExchange.Redis;
using System.Text.Json;

namespace AutoEndpoints.Redis;

internal sealed class RedisStorage(string connectionString)
{
    private readonly IDatabase database = ConnectionMultiplexer.Connect(connectionString).GetDatabase();

    public async Task<string?> GetAsync(string key)
    {
        return await database.StringGetAsync(key);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        string? value = await database.StringGetAsync(key);
        if (value is null)
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetAsync<T>(string key, T? value)
    {
        if (value is null)
        {
            await database.StringSetAsync(key, RedisValue.Null);
        }
        else
        {
            await database.StringSetAsync(key, JsonSerializer.Serialize(value));
        }
    }
}
