using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class StatisticController : Controller
    {
        public enum DataBasesNumber
        {
            QUEUE_DB = 4,
        }
        private static string REDIS_HOST = "127.0.0.1:6379";

        [HttpGet("{text_statistics}")]
        public IActionResult Get()
        {
            string value = null;

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);
            IDatabase db = redis.GetDatabase(Convert.ToInt32(DataBasesNumber.QUEUE_DB));

            value = db.StringGet("text_statistics");

            return Ok(value);
        }

        // POST api/statistic
        [HttpPost]
        public string Post([FromBody] string value)
        {
            return null;
        }

    }
}