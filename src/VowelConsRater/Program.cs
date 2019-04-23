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
        const string RATER_CALCULATED_CHANNEL = "text_rank_calculated_hints";
        const string RATER_CALCULATED_NAME = "text_rank_calculated_queue";
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

                    double consonants = Convert.ToDouble(msg.Split(':')[2]);
                    double vowel = Convert.ToDouble(msg.Split(':')[1]);
                    Console.WriteLine(id + ": " + "vowel: " + vowel + " consonants: " + consonants);

                    double letterRatio = (consonants == 0) ? vowel : vowel / consonants ;
                    string region = redisQueue.StringGet(id);

                    IDatabase redisDb = redis.GetDatabase(Convert.ToInt32(region));
                    redisDb.StringSet(id, letterRatio);
                    Console.WriteLine(id + ": " + "letterRatio: " + letterRatio + " Database: " + region);
                    
                    msg = redisQueue.ListRightPop(RATER_QUEUE_NAME);
                    sub.Publish("events", $"{"Statistic: " + id}:{letterRatio}");
                }
            });
            Console.WriteLine("VovelConsRater");
            Console.ReadLine();
        }
    }
}