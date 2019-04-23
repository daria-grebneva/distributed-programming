using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace Frontend.Controllers
{
    
    public class StatisticsController : Controller
    {
        
        [HttpGet]
        public async Task<IActionResult> TextStatistics()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"http://localhost:5000/api/statistic/text_statistics");
            string stat = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Statistics: " + stat);
            ViewData["StatisticsInfo"] = stat;
            return View();
        }

    }
}