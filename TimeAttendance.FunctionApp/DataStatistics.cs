using System;
using System.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using TimeAttendance.Business;
using TimeAttendance.Model.Entities;

namespace TimeAttendance.FunctionApp
{
    public static class DataStatistics
    {
        static FaceHelperFuntionBusiness _buss;// = new FaceHelperFuntionBusiness();
        [FunctionName("DataStatistics")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            var RedisConnection = ConfigurationManager.AppSettings["RedisConnection"];
            var connStr = ConfigurationManager.AppSettings["TimeAttendanceEntities"];
            ConnectionModel connectionModel = new ConnectionModel();
            connectionModel.RedisConnection = RedisConnection;
            connectionModel.connStr = connStr;
            _buss = new FaceHelperFuntionBusiness(connectionModel);

            string str = "Data Statistics: Calculate Attendance Time";
            try
            {
                _buss.AddOrUpdateCacheColectionFuntion();
                log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            }
            catch (Exception exception)
            {
                log.Error($"Failed in {str}: {exception.Message}", exception, null);
            }
        }
    }
}
