using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace FirstApp.PlusFourService
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://workshop-test.servicebus.windows.net/;SharedAccessKeyName=Admin;SharedAccessKey=8ZCc0FfKjz9tjj42RFO1NoJmwHvj55tDn/dbMmqAylQ=";
        const string QueueName = "newmessagequeue";


        const string ServiceBusConnectionStringToCallbackBus = "Endpoint=sb://workshop-test-sb2.servicebus.windows.net/;SharedAccessKeyName=Full;SharedAccessKey=G2aRWZwLBHZipgkbEaeOQURDH2QJLzygEPssmhGOSJE=";
        const string QueueNameToCallbackBus = "messagequeue";
        static IQueueClient queueClient;
        static IQueueClient queueClientCallback;

        static async Task Main(string[] args)
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            // // Skickar ett meddelande till servicebus queue - Skapa ny connection 
            queueClientCallback = new QueueClient(ServiceBusConnectionStringToCallbackBus, QueueNameToCallbackBus);

            // Recieve Message from ServiceBus
            var messageHandlerOptions = new MessageHandlerOptions(OnException);
            queueClient.RegisterMessageHandler(OnMessage, messageHandlerOptions);
            // await listenToMessages();

            Thread.Sleep(Timeout.Infinite);
        }

        static async Task sendMessage(string messageText)
        {
            // Skickar ett meddelande till servicebus queue
            //queueClient = new QueueClient(ServiceBusConnectionStringToCallbackBus, QueueNameToCallbackBus);
            // var body = Encoding.UTF8.GetBytes(messageText);
            // var message = new Message(body);
            // Console.WriteLine(body);
            // Console.WriteLine("Starting to send message to ServiceBus2");
            // await queueClientToCallbackBus.SendAsync(message);
            // Console.WriteLine("Sent message to ServiceBus2");

            // JSON test

            // string messageBody = JsonConvert.SerializeObject(messageText);
            Console.WriteLine("Starting to send message to ServiceBus2");
            Message message = new Message(Encoding.UTF8.GetBytes(messageText));
            await queueClientCallback.SendAsync(message);
            Console.WriteLine("Finished sending message to ServiceBus2");

            await Task.CompletedTask;
        }

        // static async Task listenToMessages()
        // {
        //     var messageHandlerOptions = new MessageHandlerOptions(OnException);
        //     queueClient.RegisterMessageHandler(OnMessage, messageHandlerOptions);

        //     await Task.CompletedTask;
        // }

        static async Task OnMessage(Message m, CancellationToken ct)
        {

            // var obj = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(m.Body));
            // Console.WriteLine("JSON object: ", obj);
            // string messageBody = JsonConvert.DeserializeObject(m.Body);

            string messageText = Encoding.UTF8.GetString(m.Body);
            Console.WriteLine("Got a message:");
            Console.WriteLine(messageText);
            Console.WriteLine($"Enqueued at {m.SystemProperties.EnqueuedTimeUtc}");

            await sendMessage(messageText);
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
