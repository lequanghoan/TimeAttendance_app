using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using TimeAttendance.Business;
using TimeAttendance.Model;
using TimeAttendance.ServiceBus;

namespace TimeAttendance.FunctionApp
{
    public static class FaceRecognition
    {
        [FunctionName("FaceRecognition")]
        public async static void Run([ServiceBusTrigger("face-detection", AccessRights.Manage, Connection = "ServiceBusConnection")]string mySbMsg, TraceWriter log)
        {
            string str = "Time Attendance: Queue Receive Face Detection";

            try
            {
                DetectFaceModel detectFaceModel = JsonConvert.DeserializeObject<DetectFaceModel>(mySbMsg);

                FaceHelperBusiness faceHelperBusiness = new FaceHelperBusiness();

                //log.Info($"{"Bat dau ghep Face: "} processed message: {DateTime.Now.ToString("HH:mm:ss:fff")}", null);
                ////Ghép face từ ảnh lớn
                //Stream imageStream = await faceHelperBusiness.MergingImage(detectFaceModel.ImageUrl, detectFaceModel.ImageWidth, detectFaceModel.ImageHeight, detectFaceModel.ListFaces);
                //log.Info($"{"Ket thuc ghep Face: "} processed message: {DateTime.Now.ToString("HH:mm:ss:fff")}", null);

                if (!string.IsNullOrEmpty(detectFaceModel.ImageUrl))
                {
                    //Detect face
                    //imageStream.Position = 0;
                    //log.Info($"{"Bat dau DetectFace Stream: "} processed message: {DateTime.Now.ToString("HH:mm:ss:fff")}", null);
                    //Face[] listFace = await faceHelperBusiness.DetectFaceAsync(imageStream);
                    //log.Info($"{"Ket thuc DetectFace Stream: "} processed message: {DateTime.Now.ToString("HH:mm:ss:fff")}", null);

                    //log.Info($"{"Bat dau DetectFace URL: "} processed message: {DateTime.Now.ToString("HH:mm:ss:fff")}", null);
                    Face[] listFace = await faceHelperBusiness.DetectFaceAsync(ConfigurationManager.AppSettings["UrlHostImage"] + ConfigurationManager.AppSettings["StorageContainer"] + "/" + detectFaceModel.ImageUrl);
                    //log.Info($"{"Ket tuc DetectFace URL: "} processed message: {DateTime.Now.ToString("HH:mm:ss:fff")}", null);

                    if (listFace != null && listFace.Length > 0)
                    {
                        DetectFaceResultModel resultModel = new DetectFaceResultModel()
                        {
                            LogImageLink = detectFaceModel.ImageUrl,
                            CaptureTime = detectFaceModel.CaptureTime
                        };

                        resultModel.ListIdentifyResult = await faceHelperBusiness.IdentifyPerson(listFace.Select(r => r.FaceId).ToArray());

                        //Đẩy service bus chấm công
                        ServiceBusSetting setting = new ServiceBusSetting()
                        {
                            ConnectionString = ConfigurationManager.AppSettings["ServiceBusConnection"],
                            QueueName = ConfigurationManager.AppSettings["AttendanceQueueName"],
                        };
                        ServiceBusSender<DetectFaceResultModel> serviceBusSender = new ServiceBusSender<DetectFaceResultModel>(setting);
                        await serviceBusSender.SendMessagesAsync(resultModel);
                    }
                }

                log.Info($"{str} processed message: {mySbMsg}", null);
            }
            catch (Exception exception)
            {
                log.Error($"Failed in {str}: {exception.Message}", exception, null);
            }
        }
    }
}
