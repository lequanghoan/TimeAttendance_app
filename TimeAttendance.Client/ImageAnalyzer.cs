// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Cognitive Services: http://www.microsoft.com/cognitive
// 
// Microsoft Cognitive Services Github:
// https://github.com/Microsoft/Cognitive
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Vision;
using Newtonsoft.Json;
using ppatierno.AzureSBLite;
using ppatierno.AzureSBLite.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeAttendance.Client.AzureStorage;
using TimeAttendance.Client.Model;
using TimeAttendance.Client.ServiceBus;
using TimeAttendance.Client.Util;

namespace TimeAttendance.Client
{
    public class ImageAnalyzer
    {
        private static FaceAttributeType[] DefaultFaceAttributeTypes = new FaceAttributeType[] { FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.HeadPose, FaceAttributeType.Emotion };
        private static VisualFeature[] DefaultVisualFeatures = new VisualFeature[] { VisualFeature.Tags, VisualFeature.Faces, VisualFeature.Categories, VisualFeature.Description, VisualFeature.Color };

        public event EventHandler FaceDetectionCompleted;
        public event EventHandler FaceRecognitionCompleted;
        public event EventHandler ComputerVisionAnalysisCompleted;
        public event EventHandler OcrAnalysisCompleted;

        public static string PeopleGroupsUserDataFilter = null;

        public Func<Task<Stream>> GetImageStreamCallback { get; set; }
        public string LocalImagePath { get; set; }
        public string ImageUrl { get; set; }

        public IEnumerable<Face> DetectedFaces { get; set; }

        public IEnumerable<IdentifiedPerson> IdentifiedPersons { get; set; }

        public IEnumerable<SimilarFaceMatch> SimilarFaceMatches { get; set; }

        public Microsoft.ProjectOxford.Vision.Contract.AnalysisResult AnalysisResult { get; set; }
        public Microsoft.ProjectOxford.Vision.Contract.OcrResults OcrResults { get; set; }

        // Default to no errors, since this could trigger a stream of popup errors since we might call this
        // for several images at once while auto-detecting the Bing Image Search results.
        public bool ShowDialogOnFaceApiErrors { get; set; } = false;

        public bool FilterOutSmallFaces { get; set; } = false;

        public int DecodedImageHeight { get; private set; }
        public int DecodedImageWidth { get; private set; }
        public byte[] Data { get; set; }
        public int imageWidth { get; set; }
        public int imageHeight { get; set; }
        public string CameraIPAdres { get; set; }
        public DateTime CaptureTime { get; set; }
        public string ListDetectedFaceJson { get; set; }

        public ImageAnalyzer(string url)
        {
            this.ImageUrl = url;
        }

        public ImageAnalyzer(Func<Task<Stream>> getStreamCallback, string path = null)
        {
            this.GetImageStreamCallback = getStreamCallback;
            this.LocalImagePath = path;
        }

        public ImageAnalyzer(byte[] data)
        {
            this.Data = data;
            this.GetImageStreamCallback = () => Task.FromResult<Stream>(new MemoryStream(this.Data));
        }

        public void UpdateDecodedImageSize(int height, int width)
        {
            this.DecodedImageHeight = height;
            this.DecodedImageWidth = width;
        }

        public async Task DetectFacesAsync(bool detectFaceAttributes = false, bool detectFaceLandmarks = false)
        {
            try
            {
                if (this.GetImageStreamCallback != null)
                {
                    Stream imageStream = await this.GetImageStreamCallback();
                    string imageUrl = await new AzureStorageUploadFiles().UploadImageStreamAsync(imageStream, "LogAttendance", "jpg");

                    if (imageUrl == null || string.IsNullOrEmpty(imageUrl))
                    {
                        return;
                    }

                    DetectFaceModel detectFaceModel = new DetectFaceModel();
                    detectFaceModel.ImageWidth = imageWidth;
                    detectFaceModel.ImageHeight = imageHeight;
                    detectFaceModel.ImageUrl = imageUrl;
                    detectFaceModel.CameraIPAdres = CameraIPAdres;
                    detectFaceModel.CaptureTime = CaptureTime;
                    detectFaceModel.ListFaces = JsonConvert.DeserializeObject<IEnumerable<FaceBox>>(ListDetectedFaceJson);

                    //Gửi Messages lên service bus
                    ServiceBusFuntion<DetectFaceModel> serviceBusSender = new ServiceBusFuntion<DetectFaceModel>();
                    await serviceBusSender.SendMessagesAsync(detectFaceModel);
                }
                return;
            }
            catch (Exception e)
            {
                this.DetectedFaces = Enumerable.Empty<Face>();
            }
        }

        private void OnFaceDetectionCompleted()
        {
            if (this.FaceDetectionCompleted != null)
            {
                this.FaceDetectionCompleted(this, EventArgs.Empty);
            }
        }

        private void OnFaceRecognitionCompleted()
        {
            if (this.FaceRecognitionCompleted != null)
            {
                this.FaceRecognitionCompleted(this, EventArgs.Empty);
            }
        }
    }


    public class IdentifiedPerson
    {
        public double Confidence
        {
            get; set;
        }

        public Person Person
        {
            get; set;
        }

        public Guid FaceId
        {
            get; set;
        }

        public InfoEmployeeModel InfoEmployee { get; set; }
    }

    public class SimilarFaceMatch
    {
        public Face Face
        {
            get; set;
        }

        public SimilarPersistedFace SimilarPersistedFace
        {
            get; set;
        }
    }

    public class InfoEmployeeModel
    {
        public string DepartmentName { get; set; }
        public string JobTitleName { get; set; }
        public string FaceId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string GenderName { get; set; }
        public DateTime? InComeDate { get; set; }
        public DateTime? OutComeDate { get; set; }
        public string IdentifyCardNumber { get; set; }
        public string LinkImage { get; set; }
        public string EmployeeId { get; set; }
        public string Address { get; set; }
    }
}
