using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using StackExchange.Redis;

namespace VowelConsRater
{
    public enum DataBasesNumber
    {
        QUEUE_DB = 4,
    }
    class Program
    { 
        private static string REDIS_HOST = "127.0.0.1:6379";
        const string RATER_HINTS_CHANNEL = "rater_hints";
        const string RATER_QUEUE_NAME = "rater_queue";
        static void Main(string[] args)
        {
            
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);
            IDatabase RedisDB = redis.GetDatabase();
            var sub = RedisDB.Multiplexer.GetSubscriber();   
            sub.Subscribe(RATER_HINTS_CHANNEL, delegate
            {
                IDatabase redisQueue = redis.GetDatabase(Convert.ToInt32(DataBasesNumber.QUEUE_DB));
                // process all messages in queue
                string msg = redisQueue.ListRightPop(RATER_QUEUE_NAME);
                Console.WriteLine(msg);
                while (msg != null)
                {
                    string id = msg.Split(':')[0];                    

                    string letterRatio =  msg.Split(':')[1];
                    string location = redisQueue.StringGet(id);
                    int regionNumber = GetDBRegion(location);

                    IDatabase redisDb = redis.GetDatabase(Convert.ToInt32(regionNumber));
                    redisDb.StringSet(id, letterRatio);
                    Console.WriteLine(id + ": " + "letterRatio: " + letterRatio + " - saved to redis. Database: " + regionNumber + " - " + location);
                    
                    msg = redisQueue.ListRightPop(RATER_QUEUE_NAME);
                }
            });
            Console.WriteLine("VovelConsRater");
            Console.ReadLine();
        }

        public static int GetDBRegion(string id)
        {
            switch (id.ToLower())
            {
                case "eu":
                    return 1;
                case "rus":
                    return 2;
                case "us":
                    return 3;
                default:
                    return 0;
            }
        }
    }
}