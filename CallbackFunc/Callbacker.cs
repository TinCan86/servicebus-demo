using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CallbackFunc
{
    public static class Callbacker
    {
        [FunctionName("Callbacker")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "callback")] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            string responseMessage = string.IsNullOrEmpty(requestBody)
                ? "No body received"
                : requestBody;

            log.LogInformation(responseMessage);

            return new OkObjectResult("ok");
        }


        // [FunctionName("ServiceBusQueueFunction")]
        // public static void Run([ServiceBusTrigger("taskqueue", Connection = "ServiceBusConnectionString")] Message message, TraceWriter log)
        // {
        //     Customer customer = JsonConvert.DeserializeObject<Customer>(Encoding.UTF8.GetString(message.Body));
        // }
    }
}
