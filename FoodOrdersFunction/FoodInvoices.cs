using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;
using System.Text;
using Azure.Messaging.ServiceBus;

namespace FoodOrdersFunction
{
    public class FoodInvoices
    {
        private readonly ILogger<FoodInvoices> _logger;

        public FoodInvoices(ILogger<FoodInvoices> log)
        {
            _logger = log;
        }

        [FunctionName("FoodInvoices")]
        public void Run([ServiceBusTrigger("orderstopic", "FoodSub", Connection = "serviceBus")] ServiceBusReceivedMessage mySbMsg, IBinder binder)
        {
            Console.WriteLine($"Processed order: {mySbMsg.ApplicationProperties["orderCode"]}");

            var sb = new StringBuilder();
            sb.AppendLine($"Product Name: {Encoding.UTF8.GetString(mySbMsg.Body)}");
            sb.AppendLine($"Price: {mySbMsg.ApplicationProperties["price"]}");
            sb.AppendLine($"Username: {mySbMsg.ApplicationProperties["userName"]}");

            var outboundBlob = new BlobAttribute($"invoices/{mySbMsg.ApplicationProperties["orderCode"]}", FileAccess.Write);
            using var writer = binder.Bind<TextWriter>(outboundBlob);

            writer.WriteLine(sb.ToString());
            _logger.LogInformation(sb.ToString());
        }
    }
}
