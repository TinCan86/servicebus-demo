using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text;

namespace FirstApp.PlusFourAPIDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        static IQueueClient queueClient;
        const string ServiceBusConnectionString = "Endpoint=sb://workshop-test.servicebus.windows.net/;SharedAccessKeyName=Admin;SharedAccessKey=8ZCc0FfKjz9tjj42RFO1NoJmwHvj55tDn/dbMmqAylQ=";
        const string QueueName = "newmessagequeue";

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [HttpPost]
        public async Task<IActionResult> PostMessage()
        {
            //Skickar ett meddelande till servicebus queue
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            Random random = new Random();
            int num = random.Next(1000);
            string randomNumberString = num.ToString();
            var messageText = randomNumberString;
            var body = Encoding.UTF8.GetBytes(messageText);
            var messageBody = new Message(body);
            await queueClient.SendAsync(messageBody);

            // return new JsonResult(
            //     new List<object>() {
            //         new { Id = 1, Message = "First chatmessage" }
            //     });

            return Ok(num);
        }
    }
}
