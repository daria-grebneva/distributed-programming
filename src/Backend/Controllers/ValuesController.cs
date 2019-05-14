using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Threading;
using StackExchange.Redis;
using System.Configuration;
using Microsoft.AspNetCore.Mvc.Core;
using Newtonsoft.Json;

namespace Backend.Controllers
{
    public enum DataBasesNumber
    {
        QUEUE_DB = 4,
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static string REDIS_HOST = "127.0.0.1:6379";
        static readonly ConcurrentDictionary<string, string> _data = new ConcurrentDictionary<string, string>();
        
        // GET api/values/<id>
        [HttpGet("{id}")]
        public string Get([FromRoute] string id)
        {
            string value = null;
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);
            IDatabase queueDb = redis.GetDatabase(Convert.ToInt32(DataBasesNumber.QUEUE_DB));
            string region = queueDb.StringGet("TextRank_" + id);
            IDatabase redisDb = redis.GetDatabase(Convert.ToInt32(region));
            string data = "request limit reached";
            
            for (int i = 0; i < 5; i++)
            {
                value = redisDb.StringGet("TextRank_" + id);
                double res;
                bool isDouble = Double.TryParse(value, out res);

                if (value != null && isDouble)
                {
                    data = "letterRatio: " + value + " Region: " + GetDBRegionString(Convert.ToInt32(region));
                    break;
                }
                else
                {
                    value = "Can not get value from database";
                    Thread.Sleep(300);
                }
                
            }


            return data;
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody]string value)
        {
            var id = Guid.NewGuid().ToString();
            
            string message = value.Split(':')[0];
            int region = GetDBRegion(value.Split(':')[1]);
            string textId = "TextRank_" + id;
            
            var redisDb = ConnectionMultiplexer.Connect(REDIS_HOST).GetDatabase(region);
            redisDb.StringSet(textId, message);

            var queueDb = ConnectionMultiplexer.Connect(REDIS_HOST).GetDatabase(Convert.ToInt32(DataBasesNumber.QUEUE_DB));
            queueDb.StringSet(textId, region);

            Console.WriteLine(textId + ": " + " Message: " + message + " Region: " + region );
            
            ISubscriber sub = ConnectionMultiplexer.Connect(REDIS_HOST).GetSubscriber();
            sub.Publish("events", $"{textId}:");

            return id;
        }
        private static int GetDBRegion(string id)
        {
            switch (id.ToLower())
            {
                case "eu":
                    return 1;
                case "rus":
                    return 2;
                case "us":
                    return 3;
                default:
                    return 0;
            }
        }

        private static string GetDBRegionString(int id)
        {
            switch (id)
            {
                case 1:
                    return "eu";
                case 2:
                    return "rus";
                case 3:
                    return "us";
                default:
                    return "";
            }
        }
    }
}
