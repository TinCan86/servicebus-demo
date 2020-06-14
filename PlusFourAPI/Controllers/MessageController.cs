using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.ServiceBus;

namespace FirstApp.PlusFourAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        static IQueueClient queueClient;
        const string ServiceBusConnectionString = "Endpoint=sb://workshop-test.servicebus.windows.net/;SharedAccessKeyName=Admin;SharedAccessKey=8ZCc0FfKjz9tjj42RFO1NoJmwHvj55tDn/dbMmqAylQ=";
        const string QueueName = "newmessagequeue";

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostMessage(int number)
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

        // #region snippet_400And201
        // [HttpPost]
        // [ProducesResponseType(StatusCodes.Status201Created)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public ActionResult<Pet> Create(Pet pet)
        // {
        //     pet.Id = _petsInMemoryStore.Any() ? 
        //              _petsInMemoryStore.Max(p => p.Id) + 1 : 1;
        //     _petsInMemoryStore.Add(pet);

        //     return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
        // }
    }
}
