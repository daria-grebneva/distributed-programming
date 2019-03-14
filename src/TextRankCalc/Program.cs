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
            const string CALCULATE_HINTS_CHANNEL = "calculate_hints";
            const string CALCULATE_QUEUE_NAME = "calculate_queue";

            var db = RedisStore.RedisDB;
            var sub = db.Multiplexer.GetSubscriber();
            sub.Subscribe("events", (channel, message) =>
            {
                string id = (string)message;
                Console.WriteLine("TextCreated: " + id);
                string str = db.StringGet(id);

                 // put message to queue
                db.ListLeftPush( CALCULATE_QUEUE_NAME,  $"{id}:{str}", flags: CommandFlags.FireAndForget );
                // and notify consumers
                db.Multiplexer.GetSubscriber().Publish( CALCULATE_HINTS_CHANNEL, "" );
            });
            
            Console.WriteLine("TextRankCalc");
            Console.ReadLine();
        }
    }
}