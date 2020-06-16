using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace FirstApp.PlusFourService
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://workshop-test-sb2.servicebus.windows.net/;SharedAccessKeyName=Full;SharedAccessKey=G2aRWZwLBHZipgkbEaeOQURDH2QJLzygEPssmhGOSJE=";
        const string QueueName = "messagequeue";
        static IQueueClient queueClient;

        static async Task Main(string[] args)
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            // Recieve Message from ServiceBus
            var messageHandlerOptions = new MessageHandlerOptions(OnException);
            queueClient.RegisterMessageHandler(OnMessage, messageHandlerOptions);


            Thread.Sleep(Timeout.Infinite);
            //Console.ReadKey();
        }

        // static async Task listenToMessages()
        // {
        //     var messageHandlerOptions = new MessageHandlerOptions(OnException);
        //     queueClient.RegisterMessageHandler(OnMessage, messageHandlerOptions);

        //     await Task.CompletedTask;
        // }

        static async Task OnMessage(Message m, CancellationToken ct)
        {
            var messageText = Encoding.UTF8.GetString(m.Body);
            Console.WriteLine("Got a message:");
            Console.WriteLine(messageText);
            Console.WriteLine($"Enqueued at {m.SystemProperties.EnqueuedTimeUtc}");

            // TODO: Post to http://functions/api/callback
            await PostAsync(messageText);

            await Task.CompletedTask;
        }

        static Task OnException(ExceptionReceivedEventArgs args)
        {
            Console.WriteLine("Got an exception:");
            Console.WriteLine(args.Exception.Message);
            Console.WriteLine(args.ExceptionReceivedContext.ToString());
            return Task.CompletedTask;
        }

        static async Task PostAsync(string data)
        {
            string postUrl = "http://functions/api/callback";

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(postUrl, new StringContent(data));

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            await Task.Run(() => JsonConvert.SerializeObject(content));
            // return await Task.Run(() => JsonObject.Parse(content));
            await Task.CompletedTask;
        }
    }
}
