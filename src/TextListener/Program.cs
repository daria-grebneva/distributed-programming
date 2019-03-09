using System;
using StackExchange.Redis;

namespace TextListener
{
    class Program
    {
        static void Main(string[] args)
        {
            var redis = RedisStore.RedisDB;
            var sub = redis.Multiplexer.GetSubscriber();
            sub.Subscribe("events", (channel, message) => {
                Console.WriteLine("IDENTIFICATOR: " + (string)message);
                Console.WriteLine("VALUE: " + redis.StringGet((string)message));
            });
            Console.WriteLine("TextListener");
            Console.ReadLine();
        }
    }
}