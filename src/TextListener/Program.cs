using System;
using StackExchange.Redis;

namespace TextListener
{
    class Program
    {
        private static string REDIS_HOST = "127.0.0.1:6379";
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);
            IDatabase RedisDB = redis.GetDatabase();
            var sub = RedisDB.Multiplexer.GetSubscriber();
            sub.Subscribe("events", (channel, message) => {
                Console.WriteLine("IDENTIFICATOR: " + (string)message);
                Console.WriteLine("VALUE: " + RedisDB.StringGet((string)message));
            });
            Console.WriteLine("TextListener");
            Console.ReadLine();
        }
    }
}