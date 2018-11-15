using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using TimeAttendance.Model;
using System.Threading.Tasks;
using TimeAttendance.Model.Repositories;
using TimeAttendance.Utils;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using TimeAttendance.Business;
using System.Configuration;
using System.Linq;
using TimeAttendance.ServiceBus;
using TimeAttendance.Model.Entities;

namespace TimeAttendance.FunctionApp
{
    public static class AttendanceTime
    {
        static FaceHelperFuntionBusiness _buss;
        [FunctionName("AttendanceTime")]
        public async static void Run([ServiceBusTrigger("attendance-time", AccessRights.Manage, Connection = "ServiceBusConnection")]string mySbMsg, TraceWriter log)
        {
            var StartTime = ConfigurationManager.AppSettings["StartTime"];
            var EndTime = ConfigurationManager.AppSettings["EndTime"];
            var connStr = ConfigurationManager.AppSettings["TimeAttendanceEntities"];
            var RedisConnection = ConfigurationManager.AppSettings["RedisConnection"];
            var UrlHostImage = ConfigurationManager.AppSettings["UrlHostImage"] + ConfigurationManager.AppSettings["StorageContainer"] + "/";
            ConnectionModel connectionModel = new ConnectionModel();
            connectionModel.RedisConnection = RedisConnection;
            connectionModel.connStr = connStr;

            var dateNow = DateTime.Now;
            TimeZone localZone = TimeZone.CurrentTimeZone;
            TimeSpan currentOffset = localZone.GetUtcOffset(dateNow);
            var utc = currentOffset.Hours;
            if (utc!=7)
            {
                //server của mỹ thì +7 giờ
                dateNow = dateNow.AddHours(7);
            }
            TimeAttendanceStatic.StartTime = DateTime.Parse(dateNow.ToShortDateString() + " " + StartTime);
            TimeAttendanceStatic.EndTime = DateTime.Parse(dateNow.ToShortDateString() + " " + EndTime);


            _buss = new FaceHelperFuntionBusiness(connectionModel);
            string str = "Time Attendance: Calculate Attendance Time";
            try
            {
                DetectFaceResultModel detectFaceModel = JsonConvert.DeserializeObject<DetectFaceResultModel>(mySbMsg);
                if (utc != 7)
                {
                    //server của mỹ thì +7 giờ
                    detectFaceModel.CaptureTime = detectFaceModel.CaptureTime.AddHours(7);
                }
                //lưu vào bảng chấm công và log chấm công
                _buss.LogTimeAttendanceFuntion(detectFaceModel.ListIdentifyResult, detectFaceModel.CaptureTime, detectFaceModel.LogImageLink);
                var rs = _buss.LogAttendanceFuntion(detectFaceModel.ListIdentifyResult, detectFaceModel.CaptureTime, detectFaceModel.LogImageLink, detectFaceModel.CameraIPAdress);
                
                //tính toán cache
                _buss.AddOrUpdateCacheDensityFuntion(detectFaceModel.ListIdentifyResult, detectFaceModel.CaptureTime);
                log.Info($"{str} processed message: {mySbMsg}", null);
                //Đẩy service bus chấm công
                ServiceBusSetting setting = new ServiceBusSetting()
                {
                    ConnectionString = ConfigurationManager.AppSettings["ServiceBusConnection"],
                    QueueName = ConfigurationManager.AppSettings["NotificationQueueName"],
                };
                ServiceBusSender<InfoNotificationResult> serviceBusSender = new ServiceBusSender<InfoNotificationResult>(setting);
                foreach (var item in rs)
                {
                    item.ImageLink = UrlHostImage + item.ImageLink;
                    try
                    {
                        await serviceBusSender.SendMessagesAsync(item);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error($"Failed in {str}: {exception.Message}", exception, null);
            }
        }


    }

}
