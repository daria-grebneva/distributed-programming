using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace TextRankCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = RedisStore.RedisDB;
            var sub = db.Multiplexer.GetSubscriber();
            sub.Subscribe("events", (channel, message) =>
            {
                string id = (string)message;
                Console.WriteLine("TextCreated: " + id);
                string str = db.StringGet(id);

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
                if (!err)
                {
                    double letterRatio = (consonantsNum == 0) ? 0 : (double)vowelsNum / (double)consonantsNum;
                    db.StringSet("TextRank_" + id, letterRatio);
                }
                Console.WriteLine("Value: " + str);
            });
            Console.WriteLine("TextRankCalc");
            Console.ReadLine();
        }
    }
}