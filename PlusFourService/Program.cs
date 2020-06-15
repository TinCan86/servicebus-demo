using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.ServiceBus;

namespace FirstApp.PlusFourService
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://workshop-test.servicebus.windows.net/;SharedAccessKeyName=Admin;SharedAccessKey=8ZCc0FfKjz9tjj42RFO1NoJmwHvj55tDn/dbMmqAylQ=";
        const string QueueName = "newmessagequeue";
        static IQueueClient queueClient;

        static async Task Main(string[] args)
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            // Recieve Message from ServiceBus
            var messageHandlerOptions = new MessageHandlerOptions(OnException);
            queueClient.RegisterMessageHandler(OnMessage, messageHandlerOptions);
            Console.ReadKey();
            // await listenToMessages();

        }

        static async Task createMessages(int num)
        {
            // Skickar ett meddelande till servicebus queue
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            string randomNumberString = num.ToString();
            Console.WriteLine(randomNumberString);
            var messageText = randomNumberString;
            var body = Encoding.UTF8.GetBytes(messageText);
            var message = new Message(body);
            await queueClient.SendAsync(message);
            Console.WriteLine("Sent message");
        }

        // static async Task listenToMessages()
        // {
        //     var messageHandlerOptions = new MessageHandlerOptions(OnException);
        //     queueClient.RegisterMessageHandler(OnMessage, messageHandlerOptions);

        //     await Task.CompletedTask;
        // }

        static async Task OnMessage(Message m, CancellationToken ct)
        {
            string messageText = Encoding.UTF8.GetString(m.Body);
            Console.WriteLine("Got a message:");
            Console.WriteLine(messageText);
            Console.WriteLine($"Enqueued at {m.SystemProperties.EnqueuedTimeUtc}");

            // const string ServiceBusConnectionStringToCallbackBus = "Endpoint=sb://workshop-test.servicebus.windows.net/;SharedAccessKeyName=Admin;SharedAccessKey=8ZCc0FfKjz9tjj42RFO1NoJmwHvj55tDn/dbMmqAylQ=";
            // const string QueueNameToCallbackBus = "messagequeue";
            // // Skickar ett meddelande till servicebus queue - Skapa ny connection 
            // var queueClientToCallbackBus = new QueueClient(ServiceBusConnectionStringToCallbackBus, QueueNameToCallbackBus);

            // Console.WriteLine("Sending message to CallbackServicebus");
            // var body = Encoding.UTF8.GetBytes(messageText);
            // var message = new Message(body);
            // await queueClientToCallbackBus.SendAsync(message);
            // Console.WriteLine("Sent message");

            //int num = Convert.ToInt16(messageText);
            // // Skickar ett meddelande till servicebus queue
            // await createMessages(new String(num + 4));
            await Task.CompletedTask;
        }

        static Task OnException(ExceptionReceivedEventArgs args)
        {
            Console.WriteLine("Got an exception:");
            Console.WriteLine(args.Exception.Message);
            Console.WriteLine(args.ExceptionReceivedContext.ToString());
            return Task.CompletedTask;
        }
    }
}
