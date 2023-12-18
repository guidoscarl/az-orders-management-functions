using System;
using System.IO;
using System.Text;
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

        [FunctionName("Function1")]
        public void Run([ServiceBusTrigger("orderstopic", "TechSub", Connection = "connectionString")]Message mySbMsg, IBinder binder)
        {

            var sb = new StringBuilder();
            sb.AppendLine($"Product Name: {Encoding.UTF8.GetString(mySbMsg.Body)}");
            sb.AppendLine($"Price: {mySbMsg.UserProperties["price"]}");
            sb.AppendLine($"Username: {mySbMsg.UserProperties["userName"]}");

            var outboundBlob = new BlobAttribute($"invoices/{mySbMsg.UserProperties["orderCode"]}", FileAccess.Write);
            using var writer = binder.Bind<TextWriter>(outboundBlob);

            writer.WriteLine(sb.ToString());
        }
    }
}
