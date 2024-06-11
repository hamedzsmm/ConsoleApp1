using StackExchange.Redis;
using System.Configuration;

namespace ConsoleApp1
{
    public static class RedisDataService
    {
        private static readonly ConnectionMultiplexer Redis = ConnectionMultiplexer.Connect(ConfigurationManager.ConnectionStrings["Redis"].ConnectionString);
        private static readonly IDatabase Database = Redis.GetDatabase();

        public static async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var serializedValue = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            await Database.StringSetAsync(key, serializedValue, expiry);
        }

        public static async Task<T?> GetAsync<T>(string key)
        {
            var value = await Database.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                return default(T);
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value!);
        }
    }
}