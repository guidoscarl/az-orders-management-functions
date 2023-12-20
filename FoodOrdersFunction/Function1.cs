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

namespace FoodOrdersFunction
{
    public class Function1
    {
        [FunctionName("FoodInvoices")]
        public void Run([ServiceBusTrigger("orderstopic", "FoodSub", Connection = "connectionString")]Message mySbMsg, IBinder binder)
        {
            Console.WriteLine($"Processed order: {mySbMsg.UserProperties["orderCode"]}");

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
