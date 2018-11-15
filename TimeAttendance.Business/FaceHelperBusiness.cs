using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeAttendance.Model;
using TimeAttendance.Model.Repositories;
using TimeAttendance.Utils;
using System.Web.Hosting;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using TimeAttendance.Storage;

namespace TimeAttendance.Business
{
    public class FaceHelperBusiness
    {
        private TimeAttendanceEntities db = new TimeAttendanceEntities();
        private DetectFaceResultModel detectFaceResultModel;
        private ComboboxBusiness comboboxBusiness = new ComboboxBusiness();
        private TransactionLog modelTransactionLog;
        private TimeAttendanceLog modelTimeAttendanceLog;
        private AttendanceLog modelAttendanceLog;
        private string TimeAttendanceLog = string.Empty;
        private Nullable<DateTime> dateNow = null;
        private const int FaceImageBoxPaddingPercentage = 50;

        public FaceHelperBusiness()
        {
            try
            {
                dateNow = DateTime.Now;
                if (dateNow != TimeAttendanceStatic.DateNow || string.IsNullOrEmpty(TimeAttendanceStatic.TimeIn) || string.IsNullOrEmpty(TimeAttendanceStatic.TimeOut)
                    || TimeAttendanceStatic.ConfidenceFix == 0 || string.IsNullOrEmpty(TimeAttendanceStatic.NotProcessTime))
                {
                    TimeAttendanceStatic.DateNow = dateNow;
                    comboboxBusiness.GetConfigResult();

                    TimeAttendanceStatic.StartTime = DateTime.ParseExact(dateNow.Value.ToString("MM/dd/yyyy") + string.Format(" {0}:00", TimeAttendanceStatic.TimeIn), "MM/dd/yyyy HH:mm:ss", null);
                    TimeAttendanceStatic.EndTime = DateTime.ParseExact(dateNow.Value.ToString("MM/dd/yyyy") + string.Format(" {0}:00", TimeAttendanceStatic.TimeOut), "MM/dd/yyyy HH:mm:ss", null);
                    TimeAttendanceStatic.ProcessTime = DateTime.ParseExact(dateNow.Value.ToString("MM/dd/yyyy") + string.Format(" {0}:00", TimeAttendanceStatic.NotProcessTime), "MM/dd/yyyy HH:mm:ss", null);
                }

                if (TimeAttendanceStatic.ListInfoEmployee == null || TimeAttendanceStatic.ListInfoEmployee.Count() == 0)
                {
                    TimeAttendanceStatic.ListInfoEmployee = comboboxBusiness.GetAllEmployee();
                }
            }
            catch (Exception ex) { }
        }

        public async Task<Stream> MergingImage(string imageUrl, int imageWidth, int imageHeight, IEnumerable<FaceBox> listFaces)
        {
            AzureStorageUploadFiles azureStorageUploadFiles = AzureStorageUploadFiles.GetInstance();
            if (azureStorageUploadFiles.CheckExitPhotoAsync(imageUrl) && listFaces != null && listFaces.Count() > 0)
            {
                Bitmap imgCrop = new Bitmap(azureStorageUploadFiles.ReadFileToStream(imageUrl));

                List<ImageCropModel> listImageCrop = new List<ImageCropModel>();
                foreach (FaceBox face in listFaces)
                {
                    int width, height, xStartPosition, yStartPosition;
                    EnlargeFaceBoxSize(face, imageWidth, imageHeight, out width, out height, out xStartPosition,
                        out yStartPosition);

                    Image imageFace = imgCrop.Clone(new Rectangle(xStartPosition, yStartPosition, width, height), imgCrop.PixelFormat);

                    listImageCrop.Add(new ImageCropModel()
                    {
                        ImageCrop = imageFace,
                        Width = width,
                        Height = height
                    });
                }

                Bitmap imgMerging = new Bitmap(listImageCrop.Sum(r => r.Width) + (listImageCrop.Count() - 1) * 5, listImageCrop.Max(r => r.Height));
                Graphics grpMerging = Graphics.FromImage(imgMerging);
                int startPoint = 0;
                foreach (var itemImage in listImageCrop)
                {
                    grpMerging.DrawImage(itemImage.ImageCrop, new Point(startPoint, 0));
                    //Giữa 2 Face cách nhau 5px
                    startPoint += itemImage.Width + 5;
                }
                grpMerging.Save();
                Stream imageStream = new MemoryStream();
                imgMerging.Save(imageStream, ImageFormat.Jpeg);
                imageStream.Position = 0;

                await azureStorageUploadFiles.UploadImageStreamAsync(imageStream, "ImageFace", "jpg");

                return imageStream;
            }
            return null;
        }

        public async Task<Face[]> DetectFaceAsync(Stream imageStream)
        {
            try
            {
                Face[] listFace = Task.Run(async () =>
                     {
                         return await FaceServiceHelper.DetectAsync(imageStream, true, false, null);
                     }).Result;
                return listFace;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Face[]> DetectFaceAsync(string linkImage)
        {
            try
            {
                Face[] listFace = Task.Run(async () =>
                {
                    return await FaceServiceHelper.DetectAsync(linkImage, true, false, null);
                }).Result;
                return listFace;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Xác định người trong nhóm
        /// </summary>
        /// <param name="detectedFaceIds">List face id detect được</param>
        /// <returns></returns>
        public async Task<IEnumerable<IdentifyResult>> IdentifyPerson(Guid[] detectedFaceIds)
        {
            try
            {
                return await FaceServiceHelper.IdentifyAsync(FaceServiceHelper.GroupTimeAttendance, detectedFaceIds);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<DetectFaceResultModel> DetectFace(DetectFaceModel detectFaceModel)
        {
            //detectFaceResultModel = new DetectFaceResultModel();
            //try
            //{
            //    DateTime start = DateTime.Now;
            //    List<IdentifiedPerson> resultPersons = new List<IdentifiedPerson>();
            //    modelTransactionLog = new TransactionLog()
            //    {
            //        TransactionLogId = Guid.NewGuid().ToString(),
            //        ClientIPAddress = detectFaceModel.ClientIPAddress,
            //        CameraIPAdress = detectFaceModel.CameraIPAdres,
            //        Date = start,
            //        CallDateTime = start,
            //        ImageLink = detectFaceModel.LogImageLink,
            //    };

            //    if (detectFaceModel.ListFaces != null && detectFaceModel.ListFaces.Count() > 0)
            //    {

            //        List<string> faceImagesUrl = new List<string>();
            //        Bitmap imgCrop = new Bitmap(detectFaceModel.ImageUrl);

            //        List<ImageCropModel> listImageCrop = new List<ImageCropModel>();
            //        foreach (FaceBox face in detectFaceModel.ListFaces)
            //        {
            //            int width, height, xStartPosition, yStartPosition;
            //            EnlargeFaceBoxSize(face, detectFaceModel.ImageWidth, detectFaceModel.ImageHeight, out width, out height, out xStartPosition,
            //                out yStartPosition);

            //            Image imageFace = imgCrop.Clone(new Rectangle(xStartPosition, yStartPosition, width, height), imgCrop.PixelFormat);

            //            listImageCrop.Add(new ImageCropModel()
            //            {
            //                ImageCrop = imageFace,
            //                Width = width,
            //                Height = height
            //            });
            //        }

            //        Bitmap imgMerging = new Bitmap(listImageCrop.Sum(r => r.Width) + (listImageCrop.Count() - 1) * 5, listImageCrop.Max(r => r.Height));
            //        Graphics grpMerging = Graphics.FromImage(imgMerging);
            //        int startPoint = 0;
            //        foreach (var itemImage in listImageCrop)
            //        {
            //            grpMerging.DrawImage(itemImage.ImageCrop, new Point(startPoint, 0));
            //            //Giữa 2 Face cách nhau 5px
            //            startPoint += itemImage.Width + 5;
            //        }
            //        grpMerging.Save();
            //        imgMerging.Save(detectFaceModel.ImageStream, ImageFormat.Bmp);
            //        detectFaceModel.ImageStream.Position = 0;
            //        ////Lưu ảnh để kiểm tra khi Debug
            //        //FaceVipService.CoreUtil.SaveImageStream(imgMerging, Constants.FolderImageFace, "MergingFace_");
            //        //AzureStorageUploadFiles.GetInstance().UploadPhoto(imgMerging, hfc[i].FileName, Constants.FolderEmployee);
            //        detectFaceResultModel.DetectedFaces = Task.Run(async () =>
            //        {
            //            return await FaceServiceHelper.DetectAsync(detectFaceModel.ImageStream, true, false, null);
            //        }).Result;
            //    }

            //    modelTransactionLog.ResponseDateTime = DateTime.Now;
            //    var ss = modelTransactionLog.ResponseDateTime - modelTransactionLog.CallDateTime;
            //    modelTransactionLog.ResponseTime = ss.Value.TotalMilliseconds;
            //    if (detectFaceResultModel.DetectedFaces != null && detectFaceResultModel.DetectedFaces.Count() > 0)
            //    {
            //        Guid[] detectedFaceIds = detectFaceResultModel.DetectedFaces?.Select(f => f.FaceId).ToArray();
            //        if (detectedFaceIds != null && detectedFaceIds.Any())
            //        {

            //            IdentifyResult[] groupResults;
            //            IdentifiedPerson alreadyIdentifiedPerson;
            //            Candidate candidate;
            //            try
            //            {
            //                groupResults = await FaceServiceHelper.IdentifyAsync(FaceServiceHelper.GroupTimeAttendance, detectedFaceIds);
            //                foreach (var match in groupResults)
            //                {
            //                    if (!match.Candidates.Any())
            //                    {
            //                        resultPersons.Add(new IdentifiedPerson
            //                        {
            //                            Confidence = 0,
            //                            FaceId = match.FaceId,
            //                            InfoEmployee = null,
            //                            Note = JsonConvert.SerializeObject(detectFaceResultModel.DetectedFaces.Where(r => r.FaceId.Equals(match.FaceId)).FirstOrDefault())
            //                        });
            //                        continue;
            //                    }

            //                    candidate = match.Candidates[0];
            //                    alreadyIdentifiedPerson = resultPersons.FirstOrDefault(p => p.Person.PersonId == candidate.PersonId);
            //                    //Trường hợp tồn tại cập nhật
            //                    if (alreadyIdentifiedPerson != null)
            //                    {
            //                        // We already tagged this person in another group. Replace the existing one if this new one if the confidence is higher.
            //                        if (alreadyIdentifiedPerson.Confidence < candidate.Confidence)
            //                        {
            //                            alreadyIdentifiedPerson.Confidence = candidate.Confidence;
            //                        }
            //                    }
            //                    //Không tồn tại thêm mới
            //                    else
            //                    {
            //                        alreadyIdentifiedPerson = new IdentifiedPerson
            //                        {
            //                            Confidence = match.Candidates[0].Confidence,
            //                            FaceId = match.FaceId,
            //                            InfoEmployee = TimeAttendanceStatic.ListInfoEmployee.Where(r => r.FaceId.Equals(candidate.PersonId.ToString())).FirstOrDefault(),
            //                            Note = JsonConvert.SerializeObject(detectFaceResultModel.DetectedFaces.Where(r => r.FaceId.Equals(match.FaceId)).FirstOrDefault())
            //                        };
            //                        if (alreadyIdentifiedPerson.InfoEmployee != null)
            //                            alreadyIdentifiedPerson.Person = new Person() { Name = alreadyIdentifiedPerson.InfoEmployee.Name, PersonId = candidate.PersonId };
            //                        else
            //                            alreadyIdentifiedPerson.Person = new Person() { Name = "Unknown", PersonId = candidate.PersonId };

            //                        resultPersons.Add(alreadyIdentifiedPerson);
            //                    }
            //                }
            //            }
            //            catch (Exception e)
            //            {
            //            }

            //            //log chấm công
            //            if (resultPersons != null && resultPersons.Count() > 0)
            //            {
            //                await this.LogTimeAttendance(db, resultPersons.Where(r => r.Confidence >= TimeAttendanceStatic.ConfidenceFix / 100).ToList(), detectFaceModel.CaptureTime, detectFaceModel.LogImageLink);
            //            }
            //            detectFaceResultModel.IdentifiedPersons = resultPersons.Where(r => r.Confidence > 0).ToList();
            //        }
            //    }
            //    else
            //    {
            //        resultPersons.Add(new IdentifiedPerson() { Note = "NotFace" });
            //    }
            //    //log vao Attendance
            //    await this.LogAttendance(db, resultPersons, start, detectFaceModel.LogImageLink);

            //    modelTransactionLog.StatusCode = "200";
            //    db.TransactionLog.Add(modelTransactionLog);
            //    db.SaveChanges();
            //}
            //catch (Exception ex)
            //{
            //    modelTransactionLog.StatusCode = ex.Source;
            //    throw ex;
            //}



            //return detectFaceResultModel;
            return null;
        }

        /// <summary>
        /// Log chấm công
        /// </summary>
        /// <param name="listIdentifiedPerson">List nhân viên nhậ được</param>
        /// <param name="date">Thời gian ghi nhận</param>
        /// <param name="imageLink">Hình ảnh nhận được</param>
        private async Task LogTimeAttendance(TimeAttendanceEntities dbEnti, List<IdentifiedPerson> listIdentifiedPerson, DateTime date, string imageLink)
        {
            List<TimeAttendanceLog> listTimeAttendanceLog = new List<TimeAttendanceLog>();
            var logDateTo = DateTimeUtils.ConvertDateFrom(date);
            var listLogNow = dbEnti.TimeAttendanceLog.Where(r => r.Date == logDateTo).ToList();
            TimeSpan timeSpan;
            TimeSpan timeSpan2;
            int sophut;
            //Lấy nhân viên có độ chính xác >= mức độ chính xác quy định
            foreach (var item in listIdentifiedPerson)
            {
                if (item.InfoEmployee == null)
                {
                    continue;
                }

                modelTimeAttendanceLog = listLogNow.Where(r => r.EmployeeId.Equals(item.InfoEmployee.EmployeeId)).FirstOrDefault();
                if (modelTimeAttendanceLog != null)
                {
                    timeSpan = (date - modelTimeAttendanceLog.TimeIn.Value);
                    modelTimeAttendanceLog.TimeOut = date;
                    modelTimeAttendanceLog.ImageOut = imageLink;
                    modelTimeAttendanceLog.Total = (int)(timeSpan.TotalHours * 60);
                    timeSpan2 = (date - TimeAttendanceStatic.EndTime.Value);
                    modelTimeAttendanceLog.EarlyMinutes = date < TimeAttendanceStatic.EndTime.Value ? (Math.Abs((timeSpan2.Hours * 60)) + Math.Abs(timeSpan2.Minutes)) : 0;
                    modelTimeAttendanceLog.ImageFaceOut = item.ImageFace;
                }
                else
                {
                    timeSpan = (date - TimeAttendanceStatic.StartTime.Value);
                    sophut = (Math.Abs((timeSpan.Hours * 60)) + Math.Abs(timeSpan.Minutes));
                    modelTimeAttendanceLog = new TimeAttendanceLog()
                    {
                        TimeAttendanceLogId = Guid.NewGuid().ToString(),
                        EmployeeId = item.InfoEmployee.EmployeeId,
                        Date = logDateTo,
                        TimeIn = date,
                        ImageIn = imageLink,
                        LateMinutes = date < TimeAttendanceStatic.StartTime.Value ? 0 : Math.Abs(sophut),
                        ImageFaceIn = item.ImageFace
                    };
                    listTimeAttendanceLog.Add(modelTimeAttendanceLog);
                }
            }
            if (listTimeAttendanceLog.Count > 0)
            {
                dbEnti.TimeAttendanceLog.AddRange(listTimeAttendanceLog);
            }
            dbEnti.SaveChanges();
        }

        private async Task LogAttendance(TimeAttendanceEntities dbEnti, List<IdentifiedPerson> listIdentifiedPerson, DateTime date, string imageLink)
        {
            try
            {
                List<AttendanceLog> listTimeAttendanceLog = new List<AttendanceLog>();
                var logDateTo = date;
                TimeSpan timeSpan;
                foreach (var item in listIdentifiedPerson)
                {
                    timeSpan = (date - TimeAttendanceStatic.StartTime.Value);
                    modelAttendanceLog = new AttendanceLog()
                    {
                        AttendanceLogId = Guid.NewGuid().ToString(),
                        EmployeeId = item.InfoEmployee != null ? item.InfoEmployee.EmployeeId : "Unknown",
                        Date = logDateTo,
                        ImageLink = imageLink,
                        ClientIPAddress = modelTransactionLog.ClientIPAddress,
                        CameraIPAdress = modelTransactionLog.CameraIPAdress,
                        Confidence = item.Confidence,
                        FaceCount = listIdentifiedPerson.Count(),
                        ImageFace = item.ImageFace,
                        Note = item.Note
                    };
                    listTimeAttendanceLog.Add(modelAttendanceLog);
                }
                if (listTimeAttendanceLog.Count > 0)
                    dbEnti.AttendanceLog.AddRange(listTimeAttendanceLog);
                dbEnti.SaveChanges();
            }
            catch (Exception)
            { }
        }

        private static void EnlargeFaceBoxSize(FaceBox face, int imageWidth, int imageHeight, out int width, out int height,
           out int xStartPosition,
           out int yStartPosition)
        {
            width = (int)face.Width;
            height = (int)face.Height;
            int paddingWidth = (int)(face.Width * FaceImageBoxPaddingPercentage / 100);
            int paddingHeight = (int)(face.Height * FaceImageBoxPaddingPercentage / 100);
            xStartPosition = (int)face.X;
            yStartPosition = (int)face.Y;
            if (xStartPosition >= paddingWidth)
            {
                xStartPosition = xStartPosition - paddingWidth;
                width = width + paddingWidth;
            }
            else
            {
                width = width + xStartPosition;
                xStartPosition = 0;
            }
            if (yStartPosition >= paddingHeight)
            {
                yStartPosition = yStartPosition - paddingHeight;
                height = height + paddingHeight;
            }
            else
            {
                height = height + paddingHeight;
                yStartPosition = 0;
            }
            if (imageWidth >= xStartPosition + width + paddingWidth)
            {
                width = width + paddingWidth;
            }
            else
            {
                width = imageWidth - xStartPosition;
            }
            if (imageHeight >= yStartPosition + height + paddingHeight)
            {
                height = height + paddingHeight;
            }
            else
            {
                height = imageHeight - yStartPosition;
            }
        }


        public void LogTimeAttendanceFuntion(List<IdentifiedPerson> listIdentifiedPerson, DateTime date, string imageLink)
        {
            List<TimeAttendanceLog> listTimeAttendanceLog = new List<TimeAttendanceLog>();
            var logDateTo = DateTimeUtils.ConvertDateFrom(date);
            var listLogNow = db.TimeAttendanceLog.Where(r => r.Date == logDateTo).ToList();
            TimeSpan timeSpan;
            TimeSpan timeSpan2;
            int sophut;
            //Lấy nhân viên có độ chính xác >= mức độ chính xác quy định
            foreach (var item in listIdentifiedPerson)
            {
                if (item.InfoEmployee == null)
                {
                    continue;
                }

                modelTimeAttendanceLog = listLogNow.Where(r => r.EmployeeId.Equals(item.InfoEmployee.EmployeeId)).FirstOrDefault();
                if (modelTimeAttendanceLog != null)
                {
                    timeSpan = (date - modelTimeAttendanceLog.TimeIn.Value);
                    modelTimeAttendanceLog.TimeOut = date;
                    modelTimeAttendanceLog.ImageOut = imageLink;
                    modelTimeAttendanceLog.Total = (int)(timeSpan.TotalHours * 60);
                    timeSpan2 = (date - TimeAttendanceStatic.EndTime.Value);
                    modelTimeAttendanceLog.EarlyMinutes = date < TimeAttendanceStatic.EndTime.Value ? (Math.Abs((timeSpan2.Hours * 60)) + Math.Abs(timeSpan2.Minutes)) : 0;
                    modelTimeAttendanceLog.ImageFaceOut = item.ImageFace;
                }
                else
                {
                    timeSpan = (date - TimeAttendanceStatic.StartTime.Value);
                    sophut = (Math.Abs((timeSpan.Hours * 60)) + Math.Abs(timeSpan.Minutes));
                    modelTimeAttendanceLog = new TimeAttendanceLog()
                    {
                        TimeAttendanceLogId = Guid.NewGuid().ToString(),
                        EmployeeId = item.InfoEmployee.EmployeeId,
                        Date = logDateTo,
                        TimeIn = date,
                        ImageIn = imageLink,
                        LateMinutes = date < TimeAttendanceStatic.StartTime.Value ? 0 : Math.Abs(sophut),
                        ImageFaceIn = item.ImageFace
                    };
                    listTimeAttendanceLog.Add(modelTimeAttendanceLog);
                }
            }
            if (listTimeAttendanceLog.Count > 0)
            {
                db.TimeAttendanceLog.AddRange(listTimeAttendanceLog);
            }
            db.SaveChanges();
        }

        public void LogAttendanceFuntion(List<IdentifiedPerson> listIdentifiedPerson, DateTime date, string imageLink)
        {
            try
            {
                List<AttendanceLog> listTimeAttendanceLog = new List<AttendanceLog>();
                var logDateTo = date;
                TimeSpan timeSpan;
                foreach (var item in listIdentifiedPerson)
                {
                    timeSpan = (date - TimeAttendanceStatic.StartTime.Value);
                    modelAttendanceLog = new AttendanceLog()
                    {
                        AttendanceLogId = Guid.NewGuid().ToString(),
                        EmployeeId = item.InfoEmployee != null ? item.InfoEmployee.EmployeeId : "Unknown",
                        Date = logDateTo,
                        ImageLink = imageLink,
                        ClientIPAddress = modelTransactionLog.ClientIPAddress,
                        CameraIPAdress = modelTransactionLog.CameraIPAdress,
                        Confidence = item.Confidence,
                        FaceCount = listIdentifiedPerson.Count(),
                        ImageFace = item.ImageFace,
                        Note = item.Note
                    };
                    listTimeAttendanceLog.Add(modelAttendanceLog);
                }
                if (listTimeAttendanceLog.Count > 0)
                    db.AttendanceLog.AddRange(listTimeAttendanceLog);
                db.SaveChanges();
            }
            catch (Exception)
            { }
        }


    }

    public class ImageCropModel
    {
        public Image ImageCrop { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
