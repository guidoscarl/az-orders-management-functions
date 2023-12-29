using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdersFunction
{
    public class TechInvoices
    {
        private readonly ILogger<TechInvoices> _logger;

        public TechInvoices(ILogger<TechInvoices> log)
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
