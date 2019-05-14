using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace TextProcessingLimiter
{
    class Program
    {
        public enum DataBasesNumber
        {
            QUEUE_DB = 4,
        }
        private static string REDIS_HOST = "127.0.0.1:6379";
        static void Main(string[] args)
        {
            
            var data = GetApplicationParams();
            Console.WriteLine(data["TEXT_PROCESSING_LIMIT"]);
           
               
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);
            IDatabase RedisDB = redis.GetDatabase();
            var sub = RedisDB.Multiplexer.GetSubscriber();   
            int textChecksNumber = 0;

            sub.Subscribe("events", (channel, message) =>
            {
                string msg = message;

                if (msg.Contains("TextRank_"))
                {
                    if (msg.Split(":")[1] != "")
                    {
                        return;
                    }
                    Console.WriteLine("TextRank!!! " + msg);
                    textChecksNumber++;
                    IDatabase queueDb = redis.GetDatabase(Convert.ToInt32(DataBasesNumber.QUEUE_DB));
                    bool result = textChecksNumber <= Convert.ToInt32(data["TEXT_PROCESSING_LIMIT"]);
                    sub.Publish("events", (msg + result.ToString()));
                    if (!result)
                    {
                        Task.Run(async () =>
                        {
                            await Task.Delay(Convert.ToInt32(data["PROCESS_ASSEPTED"]) * 1000);
                            textChecksNumber = 0;
                        });
                    }
                }

                if (msg.Contains("Statistic_"))
                {
                    Console.WriteLine("Statistic!!! " + msg);
                    double value =  Convert.ToDouble(msg.Split(':')[1]);
                    if (value <= 0.5)
                    {
                        textChecksNumber--;
                    }
                }

            });

            Console.ReadLine();
        }

        static Dictionary<string, string> GetApplicationParams()
        {
            var data = new Dictionary<string, string>();
            foreach (var row in File.ReadAllLines(Directory.GetParent(Directory.GetCurrentDirectory()) + "/Config/app.configuration"))
            data.Add(row.Split('=')[0], string.Join("=",row.Split('=').Skip(1).ToArray()));

            return data;
        }
    }
}
