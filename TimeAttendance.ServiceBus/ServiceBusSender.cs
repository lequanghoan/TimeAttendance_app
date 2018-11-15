using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using TimeAttendance.Utils;

namespace TimeAttendance.ServiceBus
{
    public class ServiceBusSender<T>
    {
        private QueueClient queueClient;
        ServiceBusSetting _serviceBusSetting;
        public ServiceBusSender(ServiceBusSetting serviceBusSetting)
        {
            _serviceBusSetting = serviceBusSetting;
        }

        public async Task SendMessagesAsync(T messageObject)
        {
            try
            {
                queueClient = QueueClient.CreateFromConnectionString(_serviceBusSetting.ConnectionString, _serviceBusSetting.QueueName);

                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObject)));
                BrokeredMessage message = new BrokeredMessage(stream);
                message.Properties["time"] = DateTime.UtcNow;

                // Send the message to the queue
                await queueClient.SendAsync(message);

                queueClient.Close();
            }
            catch (Exception exception)
            {
                throw new ErrorException(ErrorMessage.ERR008, exception.InnerException);
            }
        }
    }
}
