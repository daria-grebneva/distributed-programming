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
            var sub = redis.GetSubscriber();
            sub.Subscribe("events", (channel, message) => {
                string msg = message.ToString();
                string id = msg.Split(':')[0];
                string str = msg.Split(':')[1];
                Console.WriteLine("IDENTIFICATOR: " + id);
                Console.WriteLine("VALUE: " + str);
            });
            Console.WriteLine("TextListener");
            Console.ReadLine();
        }
    }
}