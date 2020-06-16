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
        #region First connectionstring for Servicebus 1
        const string ServiceBusConnectionString = "Endpoint=sb://workshop-test.servicebus.windows.net/;SharedAccessKeyName=Admin;SharedAccessKey=8ZCc0FfKjz9tjj42RFO1NoJmwHvj55tDn/dbMmqAylQ=";
        const string QueueName = "newmessagequeue";
        static IQueueClient queueClient;
        #endregion

        #region Second connectionstring for Servicebus 2
        const string ServiceBusConnectionStringToCallbackBus = "Endpoint=sb://workshop-test-sb2.servicebus.windows.net/;SharedAccessKeyName=Full;SharedAccessKey=G2aRWZwLBHZipgkbEaeOQURDH2QJLzygEPssmhGOSJE=";
        const string QueueNameToCallbackBus = "messagequeue";
        static IQueueClient queueClientCallback;
        #endregion


        static async Task Main(string[] args)
        {
            // Connection with first servicebus
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            // Connection with second servicebus
            queueClientCallback = new QueueClient(ServiceBusConnectionStringToCallbackBus, QueueNameToCallbackBus);

            // Recieve Message from ServiceBus
            var messageHandlerOptions = new MessageHandlerOptions(OnException);
            queueClient.RegisterMessageHandler(OnMessage, messageHandlerOptions);

            // To make sure that console application doesnt exit with code 0. 
            Thread.Sleep(Timeout.Infinite);
        }

        static async Task sendMessage(string messageText)
        {
            Console.WriteLine("Starting to send message to ServiceBus2");
            Message message = new Message(Encoding.UTF8.GetBytes(messageText));
            await queueClientCallback.SendAsync(message);
            Console.WriteLine("Finished sending message to ServiceBus2");

            await Task.CompletedTask;
        }

        static async Task OnMessage(Message m, CancellationToken ct)
        {
            string messageText = Encoding.UTF8.GetString(m.Body);
            Console.WriteLine("Got a message:");
            Console.WriteLine(messageText);
            Console.WriteLine($"Enqueued at {m.SystemProperties.EnqueuedTimeUtc}");

            await sendMessage(messageText);

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
