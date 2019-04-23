using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using StackExchange.Redis;

namespace TextStatistics
{
    class Program
    {
        public enum DataBasesNumber
        {
            QUEUE_DB = 4,
        }
        private static string REDIS_HOST = "127.0.0.1:6379";

        const string RATER_CALCULATED_CHANNEL = "text_rank_calculated_hints";
        const string RATER_CALCULATED_NAME = "text_rank_calculated_queue";

        private static int textNum = 0;

        private static int highRankPart = 0;

        private static double avgRank = 0;

        private static double result = 0;
        
        static void Main(string[] args)
        {        
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);
            IDatabase RedisDB = redis.GetDatabase();
            var sub = RedisDB.Multiplexer.GetSubscriber();   

            sub.Subscribe("events", (channel, message) =>
            {
                string msg = message;
                
                string value = msg.Split(':')[1];
                Console.WriteLine(value);
                value = msg.Split(':')[2];
                result += Convert.ToDouble(value);

                ++textNum;

                if (Convert.ToDouble(value) > 0.5)
                {
                    ++highRankPart;
                }

                avgRank = result / textNum;

                string resultMessage = "TextNum: " + textNum + "\n HighRankPart: " + highRankPart + "\n AvgRank: " +
                                        avgRank;

                var redisDb = redis.GetDatabase(Convert.ToInt32(DataBasesNumber.QUEUE_DB));
                redisDb.StringSet("text_statistics", resultMessage);
                
                Console.WriteLine(resultMessage);

            });
            
            Console.ReadLine();
        }
    }
}