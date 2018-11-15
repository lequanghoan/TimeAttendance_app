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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TimeAttendance.Client.Model;
using TimeAttendance.Client.Util;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TimeAttendance.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RealTimeOneCameraIP : Page
    {
        private Task processingReadQueue, processingWriteQueue, processingReloadPage;
        private bool isProcessingReadQueue, isProcessingWriteQueue, isReload;
        private bool isReadQueue, isWriteQueue;
        private int TimeWrite = 150, TimeRead = 500;

        public ObservableCollection<EmployeeModel> PersonsInCurrentGroup { get; set; } = new ObservableCollection<EmployeeModel>();

        public RealTimeOneCameraIP()
        {
            this.InitializeComponent();

            this.DataContext = this;

            if (RealtimeFixModel.ListEmployeeRealtime == null)
            {
                RealtimeFixModel.ListEmployeeRealtime = new Dictionary<string, DateTime>();
            }
        }

        /// <summary>
        /// Load lại trang sau h
        /// </summary>
        /// <returns></returns>
        private async Task ReloadPage()
        {
            while (isReload)
            {
                if (RealtimeFixModel.ListFrame1.Count >= 1000)
                {
                    RealtimeFixModel.ListFrame1 = new Queue<CurrentFrameModel>();
                }
                await Task.Delay(1000);
            }
        }

        private void StartCamera()
        {
            //Camera control
            if (InfoSettingFix.InfoSetting.ListCamera != null && InfoSettingFix.InfoSetting.ListCamera.Count > 0)
            {
                var itemCamera = InfoSettingFix.InfoSetting.ListCamera.First();
                MainViewModel mainViewModel = new MainViewModel();
                mainViewModel.CreateFromUri(itemCamera.StreaURI);
                cameraControl.MediaSource(mainViewModel.MediaSource);
                cameraControl.FilterOutSmallFaces = true;
                cameraControl.HideCameraControls();
                cameraControl.CameraAspectRatioChanged += CameraControl_CameraAspectRatioChanged;
                cameraControl.CameraIPAdres = itemCamera.CameraIP;
                cameraControl.SearchArea = new BitmapBounds() { Width = itemCamera.BoxWidth, Height = itemCamera.BoxHeight, X = itemCamera.BoxPointX, Y = itemCamera.BoxPointY };
            }
        }

        private void CameraControl_CameraAspectRatioChanged(object sender, EventArgs e)
        {
            this.UpdateCameraHostSize();
        }

        /// <summary>
        /// Ghi ảnh capture vào Queue
        /// </summary>
        private void StartWriteQueue()
        {
            this.isProcessingWriteQueue = true;

            if (this.processingWriteQueue == null || this.processingWriteQueue.Status != TaskStatus.Running)
            {
                this.processingWriteQueue = Task.Run(() => this.ProcessingWriteQueue());
            }

            isReload = true;
            if (this.processingReloadPage == null || this.processingReloadPage.Status != TaskStatus.Running)
            {
                this.processingReloadPage = Task.Run(() => this.ReloadPage());
            }
        }

        private async void ProcessingWriteQueue()
        {
            while (this.isProcessingWriteQueue)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (!this.isWriteQueue && RealtimeFixModel.ListFrame1.Count > 0)
                    {
                        LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " Count ListFrame1: " + RealtimeFixModel.ListFrame1.Count + "\n\r");
                        this.isWriteQueue = true;
                        try
                        {
                            CurrentFrameModel currentFrameModel;
                            lock (RealtimeFixModel.ListFrame1)
                            {
                                currentFrameModel = RealtimeFixModel.ListFrame1.Dequeue();
                            }

                            ImageAnalyzer imageAnalyzer = await this.cameraControl.CaptureFrameAsync(currentFrameModel);
                            if (imageAnalyzer != null)
                                RealtimeFixModel.ListImageDetect1.Enqueue(imageAnalyzer);

                            currentFrameModel = null;
                            imageAnalyzer = null;

                            CoreUtil.FreeMemory();
                            isWriteQueue = false;
                        }
                        catch (Exception ex)
                        {
                            this.isWriteQueue = false;
                            LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " ProcessingWriteQueue: " + ex.Message + "\n\r");
                        }
                    }
                });

                await Task.Delay(TimeWrite);
            }
        }

        /// <summary>
        /// Đọc ảnh capture từ Queue
        /// </summary>
        private void StartReadQueue()
        {
            this.isProcessingReadQueue = true;

            if (this.processingReadQueue == null || this.processingReadQueue.Status != TaskStatus.Running)
            {
                this.processingReadQueue = Task.Run(() => this.ProcessingReadQueue());
            }
        }

        private async void ProcessingReadQueue()
        {
            while (this.isProcessingReadQueue)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        if (!this.isReadQueue && RealtimeFixModel.ListImageDetect1.Count > 0)
                        {
                            LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " Count ListImageDetect1: " + RealtimeFixModel.ListImageDetect1.Count + "\n\r");
                            this.isReadQueue = true;

                            ImageAnalyzer imageAnalyzer;
                            lock (RealtimeFixModel.ListFrame1)
                            {
                                imageAnalyzer = RealtimeFixModel.ListImageDetect1.Dequeue();
                            }
                            await this.ProcessDetectFace(imageAnalyzer);
                            imageAnalyzer = null;
                        }
                        //Nếu rảnh chuyển sang ghi Queue
                        else if (!this.isReadQueue)
                        {
                            if (RealtimeFixModel.ListFrame1.Count > 0)
                            {
                                this.isReadQueue = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame1)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame1.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer1 = await this.cameraControl.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer1 != null)
                                    RealtimeFixModel.ListImageDetect1.Enqueue(imageAnalyzer1);

                                currentFrameModel = null;
                                imageAnalyzer1 = null;

                                this.isReadQueue = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.isReadQueue = false;
                        LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " ProcessingReadQueue: " + ex.Message + "\n\r");
                    }
                    CoreUtil.FreeMemory();
                });

                await Task.Delay(TimeRead);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task ProcessDetectFace(ImageAnalyzer e)
        {
            if (e == null)
            {
                this.isReadQueue = false;
                return;
            }

            try
            {
                //Detect face
                await this.DetectFacesAsync(e);

                //EmployeeModel itemPersons;
                //if (e.IdentifiedPersons != null)
                //{
                //    foreach (var item in e.IdentifiedPersons)
                //    {
                //        //Không tồn tại thì thêm mới
                //        if (item.InfoEmployee != null)
                //        {
                //            itemPersons = new EmployeeModel()
                //            {
                //                DateTime = e.CaptureTime.ToString("HH:mm:ss"),
                //                Name = item.InfoEmployee.Name + " (" + item.InfoEmployee.Code + ") ",
                //                JobTitle = item.InfoEmployee.JobTitleName + " / " + item.InfoEmployee.DepartmentName,
                //                Type = "1",
                //                Color = "#00265c"
                //            };
                //            this.PersonsInCurrentGroup.Insert(0, itemPersons);

                //            if (this.PersonsInCurrentGroup.Count() > 200)
                //                this.PersonsInCurrentGroup.RemoveAt(200);
                //        }
                //    }
                //}
                //itemPersons = null;
            }
            catch (Exception ex)
            {
                this.isReadQueue = false;
                LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " ProcessDetectFace: " + ex.Message + "\n\r");
            }

            this.isReadQueue = false;
            CoreUtil.FreeMemory();
        }

        private async Task DetectFacesAsync(ImageAnalyzer e)
        {
            await e.DetectFacesAsync(detectFaceAttributes: true);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //Start camera control
            this.StartCamera();

            Task.Run(() => this.cameraControl.ProcessingStreamAsync());

            //Bắt đầu ghi queue hình ảnh
            this.StartWriteQueue();

            //Bắt đầu đọc queue hình ảnh
            this.StartReadQueue();

            base.OnNavigatedTo(e);
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            this.isProcessingReadQueue = false;
            this.isProcessingWriteQueue = false;
            //Camera control
            this.cameraControl.CameraAspectRatioChanged -= CameraControl_CameraAspectRatioChanged;

            //Camera control
            await this.cameraControl.StopStreamAsync();
            base.OnNavigatingFrom(e);
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCameraHostSize();
        }

        private void UpdateCameraHostSize()
        {
            //Camera control
            this.cameraHostGrid.Width = this.cameraHostGrid.ActualHeight * (this.cameraControl.CameraAspectRatio != 0 ? this.cameraControl.CameraAspectRatio : 1.777777777777);
            cameraControl.SetSizeControl(this.cameraHostGrid.Width);
        }
    }
}