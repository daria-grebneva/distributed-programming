using StackExchange.Redis;
using System;
using System.Configuration;

namespace VowelConsCounter
{
    public class RedisStore
    {
        public static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        public static IDatabase RedisDB = redis.GetDatabase();
    }
}