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
            IDatabase redisDb = redis.GetDatabase(GetDBRegion(region));
            
            for (int i = 0; i < 5; i++)
            {
                value = redisDb.StringGet("TextRank_" + id);

                if (value != null)
                {
                    break;
                }
                else
                {
                    value = "Can not get value from database";
                    Thread.Sleep(300);
                }
            }

            string data = "letterRatio: " + value + " Region: " + region;

            return data;
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody]string value)
        {
            var id = Guid.NewGuid().ToString();
            
            string message = value.Split(':')[0];
            string region = value.Split(':')[1];
            string textId = "TextRank_" + id;
            
            var redisDb = ConnectionMultiplexer.Connect(REDIS_HOST).GetDatabase(GetDBRegion(region));
            redisDb.StringSet(textId, message);

            var queueDb = ConnectionMultiplexer.Connect(REDIS_HOST).GetDatabase(Convert.ToInt32(DataBasesNumber.QUEUE_DB));
            queueDb.StringSet(textId, region);

            Console.WriteLine(textId + ": " + " Message: " + message + " Region: " + region + " : " + GetDBRegion(region));
            
            ISubscriber sub = ConnectionMultiplexer.Connect(REDIS_HOST).GetSubscriber();
            sub.Publish("events", $"{textId}:{message}");

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
    }
}
