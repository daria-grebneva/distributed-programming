using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using StackExchange.Redis;

namespace VowelConsRater
{
    class Program
    { 
        const string RATER_HINTS_CHANNEL = "rater_hints";
        const string RATER_QUEUE_NAME = "rater_queue";
        static void Main(string[] args)
        {
            var db = RedisStore.RedisDB;
            var sub = db.Multiplexer.GetSubscriber();   
            sub.Subscribe(RATER_HINTS_CHANNEL, delegate
            {
                // process all messages in queue
                string msg = db.ListRightPop(RATER_QUEUE_NAME);
                Console.WriteLine(msg);
                while (msg != null)
                {
                    string id = msg.Split(':')[0];
                    double vowelsNum = Convert.ToDouble(msg.Split(':')[1]);
                    double consonantsNum = Convert.ToDouble(msg.Split(':')[2]);
                    double letterRatio = consonantsNum == 0 ? 0 : vowelsNum / consonantsNum;

                    db.StringSet("TextRank_" + id, letterRatio);

                    msg = db.ListRightPop(RATER_QUEUE_NAME);
                }
            });
            Console.WriteLine("VovelConsRater");
            Console.ReadLine();
        }
    }
}