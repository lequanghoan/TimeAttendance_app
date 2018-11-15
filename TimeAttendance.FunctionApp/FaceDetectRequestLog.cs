using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using System;

namespace TimeAttendance.FunctionApp
{
    public static class FaceDetectRequestLog
    {
        [FunctionName("FaceDetectRequestLog")]
        public static void Run([ServiceBusTrigger("activity-log", AccessRights.Manage, Connection = "ServiceBusConnection")]BrokeredMessage mySbMsg, TraceWriter log)
        {
            string str = "Time Attendance: Queue Face Detection Request";

            try
            {

                log.Info($"{str} processed message: {mySbMsg}", null);
            }
            catch (Exception exception)
            {
                log.Error($"Failed in {str}: {exception.Message}", exception, null);
            }
        }
    }
}
