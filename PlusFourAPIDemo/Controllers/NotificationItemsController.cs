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
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace FirstApp.PlusFourAPIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationItemsController : ControllerBase
    {
        private readonly NotificationContext _context;
        private readonly IQueueClient queueClient;
        const string ServiceBusConnectionString = "Endpoint=sb://workshop-test.servicebus.windows.net/;SharedAccessKeyName=Admin;SharedAccessKey=8ZCc0FfKjz9tjj42RFO1NoJmwHvj55tDn/dbMmqAylQ=";
        const string QueueName = "newmessagequeue";


        public NotificationItemsController(NotificationContext context)
        {
            _context = context;
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
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

            string id = Guid.NewGuid().ToString();

            Console.WriteLine(id);

            notificationItem.Id = id;

            string messageBody = JsonConvert.SerializeObject(notificationItem);

            Message message = new Message(Encoding.UTF8.GetBytes(messageBody));
            //Skickar ett meddelande till servicebus queue
            await queueClient.SendAsync(message);

            _context.NotificationItems.Add(notificationItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotificationItem), new { id = id }, notificationItem);

        }

        internal sealed class FormatNumbersAsTextConverter : JsonConverter
        {
            public override bool CanRead => false;
            public override bool CanWrite => true;
            public override bool CanConvert(Type type) => type == typeof(long);

            public override void WriteJson(
                JsonWriter writer, object value, JsonSerializer serializer)
            {
                long number = (long)value;
                writer.WriteValue(number.ToString(CultureInfo.InvariantCulture));
            }

            public override object ReadJson(
                JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
            {
                throw new NotSupportedException();
            }
        }
    }
}
