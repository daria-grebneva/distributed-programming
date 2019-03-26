﻿using System;
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
        static void Main(string[] args)
        {
            var db = RedisStore.RedisDB;
            var sub = db.Multiplexer.GetSubscriber();   
            sub.Subscribe(COUNTER_HINTS_CHANNEL, delegate
            {
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
        private static void DoJob( string jobData )
        {
            Console.WriteLine( $"Job data: {jobData}" );
            System.Threading.Thread.Sleep(1500); // emulate loading
        }

        private static string ParseData( string msg )
        {
            return msg.Split( ':' )[1];
        }

    }
}