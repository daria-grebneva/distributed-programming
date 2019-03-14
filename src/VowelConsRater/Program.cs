using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using StackExchange.Redis;

namespace VowelConsRater
{
    class Program
    { 
        const string COUNTER_HINTS_CHANNEL = "counter_hints";
        const string COUNTER_QUEUE_NAME = "counter_queue";
        static void Main(string[] args)
        {
            var db = RedisStore.RedisDB;
            var sub = db.Multiplexer.GetSubscriber();   
            sub.Subscribe(COUNTER_HINTS_CHANNEL, delegate
            {
                // process all messages in queue
                string msg = db.ListRightPop(COUNTER_QUEUE_NAME);
                    Console.WriteLine(msg);
                while (msg != null)
                {
                    string id = msg.Split(':')[0];
                    int vowelsNum = Convert.ToInt64(msg.Split(':')[1]);
                    int consonantsNum = Convert.ToInt64(msg.Split(':')[2]);
                    double letterRatio = consonantsNum == 0 ? 0 : vowelsNum / consonantsNum;

                    db.StringSet("TextRank_" + id, letterRatio);

                    msg = db.ListRightPop(COUNTER_QUEUE_NAME);
                }
            });
            Console.WriteLine("VovelConsRater");
            Console.ReadLine();
        }
    }
}
