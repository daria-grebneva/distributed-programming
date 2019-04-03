﻿using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using StackExchange.Redis;

namespace TextRankCalc
{
    class Program
{
        const string COUNTER_HINTS_CHANNEL = "counter_hints";
        const string COUNTER_QUEUE_NAME = "counter_queue";
        private static string REDIS_HOST = "127.0.0.1:6379";
        public enum DataBasesNumber
        {
            QUEUE_DB = 4,
        }
        static void Main(string[] args)
        {

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);
            IDatabase RedisDB = redis.GetDatabase();
            var sub = RedisDB.Multiplexer.GetSubscriber();
            sub.Subscribe("events", (channel, message) =>
            {
                string id = (string)message;
                Console.WriteLine("TextCreated: " + id);
                string str = RedisDB.StringGet(id);
                RedisDB = redis.GetDatabase(Convert.ToInt32(DataBasesNumber.QUEUE_DB));
                
                 // put message to queue
                RedisDB.ListLeftPush( COUNTER_QUEUE_NAME,  $"{id}:{str}", flags: CommandFlags.FireAndForget );
                // and notify consumers
                RedisDB.Multiplexer.GetSubscriber().Publish( COUNTER_HINTS_CHANNEL, "" );
            });
            
            Console.WriteLine("TextRankCalc");
            Console.ReadLine();
        }
    }
}