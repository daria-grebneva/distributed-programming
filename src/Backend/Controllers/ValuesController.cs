using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Threading;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        static readonly ConcurrentDictionary<string, string> _data = new ConcurrentDictionary<string, string>();
        
        // GET api/values/<id>
        [HttpGet("{id}")]
        public string Get([FromRoute] string id)
        {
            string value = null;
            var db = RedisStore.RedisDB;
            
            for (int i = 0; i < 5; i++)
            {
                value = db.StringGet("TextRank_" + id);

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

            return value;
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody]string value)
        {
            var id = Guid.NewGuid().ToString();
            _data[id] = value;
            var db = RedisStore.RedisDB;
            db.StringSet(id, value);
            var pub = db.Multiplexer.GetSubscriber();
            pub.Publish("events", id);

            return id;
        }
    }
}
