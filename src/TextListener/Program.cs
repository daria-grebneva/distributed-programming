using System;
using StackExchange.Redis;

namespace TextListener
{
    public enum DataBasesNumber
    {
        QUEUE_DB = 4,
    }
    class Program
    {
        private static string REDIS_HOST = "127.0.0.1:6379";
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);
            IDatabase redisQueue = redis.GetDatabase(Convert.ToInt32(DataBasesNumber.QUEUE_DB));
            var sub = redis.GetSubscriber();
            sub.Subscribe("events", (channel, message) => {
                string msg = message.ToString();
                string id = msg.Split(':')[0];
                if (id.Contains("TextRank_"))
                {
                    Console.WriteLine(msg);
                    string region = redisQueue.StringGet(id);
                    var regionDb = ConnectionMultiplexer.Connect(REDIS_HOST).GetDatabase(Convert.ToInt32(region));
                    
                    string text = regionDb.StringGet(id);
                    Console.WriteLine("IDENTIFICATOR: " + id);
                    Console.WriteLine("REGION: " + region);
                    Console.WriteLine("TEXT: " + text);
                }
            });
            Console.WriteLine("TextListener");
            Console.ReadLine();
        }
    }
}