using ppatierno.AzureSBLite;
using ppatierno.AzureSBLite.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Client.ServiceBus
{
    public static class ServiceBusSetting
    {
        public static string QueueURL { get; set; }

        public static string AccessKeyName { get; set; }

        public static string AccessKeyValue { get; set; }

        //public static bool CheckServiceBus()
        //{
        //    try
        //    {
        //        ServiceBusConnectionStringBuilder builder = new ServiceBusConnectionStringBuilder(ServiceBusSetting.ConnectionString);
        //        builder.TransportType = TransportType.Amqp;

        //        MessagingFactory factory = MessagingFactory.CreateFromConnectionString(ServiceBusSetting.ConnectionString);

        //        QueueClient client = factory.CreateQueueClient(ServiceBusSetting.QueueName);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
    }
}
