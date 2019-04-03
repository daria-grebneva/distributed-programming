using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using StackExchange.Redis;

namespace VowelConsCounter
{
    class Program
    {
        const string COUNTER_HINTS_CHANNEL = "counter_hints";
        const string COUNTER_QUEUE_NAME = "counter_queue";
        const string RATER_HINTS_CHANNEL = "rater_hints";
        const string RATER_QUEUE_NAME = "rater_queue";
        private static string REDIS_HOST = "127.0.0.1:6379";
        public enum DataBasesNumber
        {
            QUEUE_DB = 4,
        }
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);  
            ISubscriber sub = redis.GetSubscriber();
            sub.Subscribe(COUNTER_HINTS_CHANNEL, delegate
            {
                IDatabase db = redis.GetDatabase(Convert.ToInt32(DataBasesNumber.QUEUE_DB));
                string msg = db.ListRightPop(COUNTER_QUEUE_NAME);
                Console.WriteLine(msg);
                while (msg != null)
                {
                    string id = msg.Split(':')[0];
                    string str = msg.Split(':')[1];
                    var VOWELS = new List<char>() { 'a', 'i', 'e', 'u', 'o', 'y'};
                    var CONSONANTS = new List<char>() {'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'};

                    int vowelsNum = 0;
                    int consonantsNum = 0;
                    bool err = false;

                    foreach (char letter in str)
                    {
                        if (VOWELS.Contains(letter))
                        {
                            ++vowelsNum;
                        }
                        else if (CONSONANTS.Contains(letter))
                        {
                            ++consonantsNum;
                        }
                        else
                        {
                            err = true;
                            Console.WriteLine("something wrong with your word");                    
                        }
                    }
                    
                    // put message to queue
                    db.ListLeftPush(RATER_QUEUE_NAME, $"{id}:{vowelsNum}:{consonantsNum}", flags: CommandFlags.FireAndForget);
                    // and notify consumers
                    db.Multiplexer.GetSubscriber().Publish(RATER_HINTS_CHANNEL, "");

                    msg = db.ListRightPop(COUNTER_QUEUE_NAME);
                }
            });
            
            Console.WriteLine("VovelConsCounter");
            Console.ReadLine();
        }

    }
}