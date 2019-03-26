using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using StackExchange.Redis;

namespace TextRankCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            const string COUNTER_HINTS_CHANNEL = "counter_hints";
            const string COUNTER_QUEUE_NAME = "counter_queue";

            var db = RedisStore.RedisDB;
            var sub = db.Multiplexer.GetSubscriber();
            sub.Subscribe("events", (channel, message) =>
            {
                string id = (string)message;
                Console.WriteLine("TextCreated: " + id);
                string str = db.StringGet(id);

                 // put message to queue
                db.ListLeftPush( COUNTER_QUEUE_NAME,  $"{id}:{str}", flags: CommandFlags.FireAndForget );
                // and notify consumers
                db.Multiplexer.GetSubscriber().Publish( COUNTER_HINTS_CHANNEL, "" );
            });
            
            Console.WriteLine("TextRankCalc");
            Console.ReadLine();
        }
    }
}