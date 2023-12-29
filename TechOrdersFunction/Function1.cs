using System;
using System.IO;
using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TechOrdersFunction
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> log)
        {
            _logger = log;
        }

        [FunctionName("TechInvoices")]
        public void Run([ServiceBusTrigger("orderstopic", "TechSub", Connection = "serviceBus")] ServiceBusReceivedMessage mySbMsg, IBinder binder)
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
