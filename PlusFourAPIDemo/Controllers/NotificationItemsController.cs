using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationApi.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace FirstApp.PlusFourAPIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationItemsController : ControllerBase
    {
        private readonly NotificationContext _context;
        static IQueueClient queueClient;
        const string ServiceBusConnectionString = "Endpoint=sb://workshop-test.servicebus.windows.net/;SharedAccessKeyName=Admin;SharedAccessKey=8ZCc0FfKjz9tjj42RFO1NoJmwHvj55tDn/dbMmqAylQ=";
        const string QueueName = "newmessagequeue";


        public NotificationItemsController(NotificationContext context)
        {
            _context = context;
        }

        // GET: api/NotificationItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationItem>>> GetNotificationItems()
        {
            return await _context.NotificationItems.ToListAsync();
        }

        // GET: api/NotificationItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationItem>> GetNotificationItem(long id)
        {
            var notificationItem = await _context.NotificationItems.FindAsync(id);

            if (notificationItem == null)
            {
                return NotFound();
            }

            return notificationItem;
        }

        [HttpPost]
        public async Task<ActionResult<NotificationItem>> PostNotificationItem(NotificationItem notificationItem)
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            // Formatting.Indented can be added for json readout
            string messageBody = JsonConvert.SerializeObject(notificationItem);
            Console.WriteLine(messageBody);
            Message message = new Message(Encoding.UTF8.GetBytes(messageBody));
            //Skickar ett meddelande till servicebus queue

            await queueClient.SendAsync(message);

            _context.NotificationItems.Add(notificationItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotificationItem), new { id = notificationItem.Id }, notificationItem);
        }
    }
}