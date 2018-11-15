using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace TimeAttendance.FunctionApp
{
    public static class FaceDetectNotification
    {
        [FunctionName("FaceDetectNotification")]
        public static async Task Run([ServiceBusTrigger("notification", AccessRights.Manage, Connection = "ServiceBusConnection")]string myQueueItem, TraceWriter log)
        {
            string str = "Face Detect Notification: Calculate Face Detect Notification";
            try
            {
                log.Info($"C# Queue trigger function processed: {myQueueItem}");

                // In this example the queue item is a new user to be processed in the form of a JSON string with 
                // a "name" value.
                //
                // The JSON format for a native GCM notification is ...
                // { "data": { "message": "notification message" }}  
                NotificationHubClient hub = NotificationHubClient
                                    .CreateClientFromConnectionString(ConfigurationManager.AppSettings["NotificationConnection"], ConfigurationManager.AppSettings["NotificationHub"]);

                string gcmNotificationPayload = "{\"data\": { \"message\": " + myQueueItem + " }} ";
                log.Info($"{gcmNotificationPayload}");
                await hub.SendGcmNativeNotificationAsync(gcmNotificationPayload);
            }
            catch (Exception exception)
            {
                log.Error($"Failed in {str}: {exception.Message}", exception, null);
            }
        }
    }
}
