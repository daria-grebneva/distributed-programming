using StackExchange.Redis;
using System;
using System.Configuration;

namespace Backend.Controllers
{
    public class RedisStore
    {
        public static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        public static IDatabase RedisCache = redis.GetDatabase();
    }
}