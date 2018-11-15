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
    public sealed partial class RealTimeFourCameraIP : Page
    {
        private Task processingReadQueue1, processingReadQueue2, processingReadQueue3, processingReadQueue4, processingWriteQueue1, processingWriteQueue2, processingWriteQueue3, processingWriteQueue4, processingReloadPage;
        private bool isProcessingReadQueue, isProcessingWriteQueue;
        private bool isReadQueue1, isReadQueue2, isReadQueue3, isReadQueue4, isWriteQueue1, isWriteQueue2, isWriteQueue3, isWriteQueue4, isReload;
        private int TimeWrite = 66, TimeRead = 100;
        private DateTime DateStartPage = DateTime.Now;

        public ObservableCollection<EmployeeModel> PersonsInCurrentGroup { get; set; } = new ObservableCollection<EmployeeModel>();

        public RealTimeFourCameraIP()
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
                if (RealtimeFixModel.ListFrame2.Count >= 1000)
                {
                    RealtimeFixModel.ListFrame2 = new Queue<CurrentFrameModel>();
                }
                if (RealtimeFixModel.ListFrame3.Count >= 1000)
                {
                    RealtimeFixModel.ListFrame3 = new Queue<CurrentFrameModel>();
                }
                if (RealtimeFixModel.ListFrame4.Count >= 1000)
                {
                    RealtimeFixModel.ListFrame4 = new Queue<CurrentFrameModel>();
                }
                await Task.Delay(2000);
            }
        }

        private void StartCamera()
        {
            //Camera control
            if (InfoSettingFix.InfoSetting.ListCamera != null && InfoSettingFix.InfoSetting.ListCamera.Count > 0)
            {
                int index = 1;
                foreach (var itemCamera in InfoSettingFix.InfoSetting.ListCamera)
                {
                    if (index == 1)
                    {
                        MainViewModel mainViewModel = new MainViewModel();
                        mainViewModel.CreateFromUri(itemCamera.StreaURI);
                        cameraControl1.MediaSource(mainViewModel.MediaSource);
                        cameraControl1.FilterOutSmallFaces = true;
                        cameraControl1.HideCameraControls();
                        cameraControl1.CameraAspectRatioChanged += CameraControl_CameraAspectRatioChanged;
                        cameraControl1.CameraIPAdres = itemCamera.CameraIP;
                        cameraControl1.SearchArea = new BitmapBounds() { Width = itemCamera.BoxWidth, Height = itemCamera.BoxHeight, X = itemCamera.BoxPointX, Y = itemCamera.BoxPointY };
                    }
                    else if (index == 2)
                    {
                        MainViewModel mainViewModel = new MainViewModel();
                        mainViewModel.CreateFromUri(itemCamera.StreaURI);
                        cameraControl2.MediaSource(mainViewModel.MediaSource);
                        cameraControl2.FilterOutSmallFaces = true;
                        cameraControl2.HideCameraControls();
                        cameraControl2.CameraAspectRatioChanged += CameraControl_CameraAspectRatioChanged;
                        cameraControl2.CameraIPAdres = itemCamera.CameraIP;
                        cameraControl2.SearchArea = new BitmapBounds() { Width = itemCamera.BoxWidth, Height = itemCamera.BoxHeight, X = itemCamera.BoxPointX, Y = itemCamera.BoxPointY };
                    }
                    else if (index == 3)
                    {
                        MainViewModel mainViewModel = new MainViewModel();
                        mainViewModel.CreateFromUri(itemCamera.StreaURI);
                        cameraControl3.MediaSource(mainViewModel.MediaSource);
                        cameraControl3.FilterOutSmallFaces = true;
                        cameraControl3.HideCameraControls();
                        cameraControl3.CameraAspectRatioChanged += CameraControl_CameraAspectRatioChanged;
                        cameraControl3.CameraIPAdres = itemCamera.CameraIP;
                        cameraControl3.SearchArea = new BitmapBounds() { Width = itemCamera.BoxWidth, Height = itemCamera.BoxHeight, X = itemCamera.BoxPointX, Y = itemCamera.BoxPointY };
                    }
                    else if (index == 4)
                    {
                        MainViewModel mainViewModel = new MainViewModel();
                        mainViewModel.CreateFromUri(itemCamera.StreaURI);
                        cameraControl4.MediaSource(mainViewModel.MediaSource);
                        cameraControl4.FilterOutSmallFaces = true;
                        cameraControl4.HideCameraControls();
                        cameraControl4.CameraAspectRatioChanged += CameraControl_CameraAspectRatioChanged;
                        cameraControl4.CameraIPAdres = itemCamera.CameraIP;
                        cameraControl4.SearchArea = new BitmapBounds() { Width = itemCamera.BoxWidth, Height = itemCamera.BoxHeight, X = itemCamera.BoxPointX, Y = itemCamera.BoxPointY };
                    }
                    index++;
                }
            }
        }

        private void CameraControl_CameraAspectRatioChanged(object sender, EventArgs e)
        {
            this.UpdateCameraHostSize();
        }

        private void StartWriteQueue()
        {
            this.isProcessingWriteQueue = true;

            if (this.processingWriteQueue1 == null || this.processingWriteQueue1.Status != TaskStatus.Running)
            {
                this.processingWriteQueue1 = Task.Run(() => this.ProcessingWriteQueue1());
            }

            if (this.processingWriteQueue2 == null || this.processingWriteQueue2.Status != TaskStatus.Running)
            {
                this.processingWriteQueue2 = Task.Run(() => this.ProcessingWriteQueue2());
            }

            if (this.processingWriteQueue3 == null || this.processingWriteQueue3.Status != TaskStatus.Running)
            {
                this.processingWriteQueue3 = Task.Run(() => this.ProcessingWriteQueue3());
            }

            if (this.processingWriteQueue4 == null || this.processingWriteQueue4.Status != TaskStatus.Running)
            {
                this.processingWriteQueue4 = Task.Run(() => this.ProcessingWriteQueue4());
            }

            isReload = true;
            if (this.processingReloadPage == null || this.processingReloadPage.Status != TaskStatus.Running)
            {
                this.processingReloadPage = Task.Run(() => this.ReloadPage());
            }
        }

        private async Task ProcessingWriteQueue1()
        {
            while (isProcessingWriteQueue)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (!isWriteQueue1 && RealtimeFixModel.ListFrame1.Count > 0)
                    {
                        LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " Count ListFrame1: " + RealtimeFixModel.ListFrame1.Count + "\n\r");
                        isWriteQueue1 = true;
                        try
                        {
                            CurrentFrameModel currentFrameModel;
                            lock (RealtimeFixModel.ListFrame1)
                            {
                                currentFrameModel = RealtimeFixModel.ListFrame1.Dequeue();
                            }

                            ImageAnalyzer imageAnalyzer1 = await this.cameraControl1.CaptureFrameAsync(currentFrameModel);
                            if (imageAnalyzer1 != null)
                                RealtimeFixModel.ListImageDetect1.Enqueue(imageAnalyzer1);

                            currentFrameModel = null;
                            imageAnalyzer1 = null;
                            CoreUtil.FreeMemory();

                            isWriteQueue1 = false;
                        }
                        catch (Exception ex)
                        {
                            isWriteQueue1 = false;
                            LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " ProcessingWriteQueue1: " + ex.Message + "\n\r");
                        }
                    }
                });

                await Task.Delay(TimeWrite);
            }
        }

        private async Task ProcessingWriteQueue2()
        {
            while (isProcessingWriteQueue)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (!isWriteQueue2 && RealtimeFixModel.ListFrame2.Count > 0)
                    {
                        LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " Count ListFrame2: " + RealtimeFixModel.ListFrame2.Count + "\n\r");
                        isWriteQueue2 = true;
                        try
                        {
                            CurrentFrameModel currentFrameModel;
                            lock (RealtimeFixModel.ListFrame2)
                            {
                                currentFrameModel = RealtimeFixModel.ListFrame2.Dequeue();
                            }

                            ImageAnalyzer imageAnalyzer2 = await this.cameraControl2.CaptureFrameAsync(currentFrameModel);
                            if (imageAnalyzer2 != null)
                                RealtimeFixModel.ListImageDetect2.Enqueue(imageAnalyzer2);

                            currentFrameModel = null;
                            imageAnalyzer2 = null;
                            CoreUtil.FreeMemory();

                            isWriteQueue2 = false;
                        }
                        catch (Exception ex)
                        {
                            isWriteQueue2 = false;
                            LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " ProcessingWriteQueue2: " + ex.Message + "\n\r");
                        }
                    }
                });

                await Task.Delay(TimeWrite);
            }
        }

        private async Task ProcessingWriteQueue3()
        {
            while (isProcessingWriteQueue)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (!isWriteQueue3 && RealtimeFixModel.ListFrame3.Count > 0)
                    {
                        LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " Count ListFrame3: " + RealtimeFixModel.ListFrame3.Count + "\n\r");
                        isWriteQueue3 = true;
                        try
                        {
                            CurrentFrameModel currentFrameModel;
                            lock (RealtimeFixModel.ListFrame3)
                            {
                                currentFrameModel = RealtimeFixModel.ListFrame3.Dequeue();
                            }

                            ImageAnalyzer imageAnalyzer3 = await this.cameraControl3.CaptureFrameAsync(currentFrameModel);
                            if (imageAnalyzer3 != null)
                                RealtimeFixModel.ListImageDetect3.Enqueue(imageAnalyzer3);

                            currentFrameModel = null;
                            imageAnalyzer3 = null;
                            CoreUtil.FreeMemory();

                            isWriteQueue3 = false;
                        }
                        catch (Exception ex)
                        {
                            isWriteQueue3 = false;
                            LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " ProcessingWriteQueue3: " + ex.Message + "\n\r");
                        }
                    }
                });

                await Task.Delay(TimeWrite);
            }
        }

        private async Task ProcessingWriteQueue4()
        {
            while (isProcessingWriteQueue)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (!isWriteQueue4 && RealtimeFixModel.ListFrame4.Count > 0)
                    {
                        LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " Count ListFrame4: " + RealtimeFixModel.ListFrame4.Count + "\n\r");
                        isWriteQueue4 = true;
                        try
                        {
                            CurrentFrameModel currentFrameModel;
                            lock (RealtimeFixModel.ListFrame4)
                            {
                                currentFrameModel = RealtimeFixModel.ListFrame4.Dequeue();
                            }

                            ImageAnalyzer imageAnalyzer4 = await this.cameraControl4.CaptureFrameAsync(currentFrameModel);
                            if (imageAnalyzer4 != null)
                                RealtimeFixModel.ListImageDetect4.Enqueue(imageAnalyzer4);

                            currentFrameModel = null;
                            imageAnalyzer4 = null;
                            CoreUtil.FreeMemory();

                            isWriteQueue4 = false;
                        }
                        catch (Exception ex)
                        {
                            isWriteQueue4 = false;
                            LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " ProcessingWriteQueue4: " + ex.Message + "\n\r");
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

            if (this.processingReadQueue1 == null || this.processingReadQueue1.Status != TaskStatus.Running)
            {
                this.processingReadQueue1 = Task.Run(() => this.ProcessingReadQueue1());
            }

            if (this.processingReadQueue2 == null || this.processingReadQueue2.Status != TaskStatus.Running)
            {
                this.processingReadQueue2 = Task.Run(() => this.ProcessingReadQueue2());
            }

            if (this.processingReadQueue3 == null || this.processingReadQueue3.Status != TaskStatus.Running)
            {
                this.processingReadQueue3 = Task.Run(() => this.ProcessingReadQueue3());
            }

            if (this.processingReadQueue4 == null || this.processingReadQueue4.Status != TaskStatus.Running)
            {
                this.processingReadQueue4 = Task.Run(() => this.ProcessingReadQueue4());
            }


        }

        private async void ProcessingReadQueue1()
        {
            while (this.isProcessingReadQueue)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        if (!this.isReadQueue1 && RealtimeFixModel.ListImageDetect1.Count > 0)
                        {
                            LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " Count ListImageDetect1: " + RealtimeFixModel.ListImageDetect1.Count + "\n\r");
                            this.isReadQueue1 = true;

                            ImageAnalyzer imageAnalyzer;
                            lock (RealtimeFixModel.ListImageDetect1)
                            {
                                imageAnalyzer = RealtimeFixModel.ListImageDetect1.Dequeue();
                            }

                            await this.DetectFaceAsync1(imageAnalyzer);

                            imageAnalyzer = null;
                        }
                        else if (!this.isReadQueue1)
                        {
                            if (RealtimeFixModel.ListFrame1.Count > 0)
                            {
                                this.isReadQueue1 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame1)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame1.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer1 = await this.cameraControl1.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer1 != null)
                                    RealtimeFixModel.ListImageDetect1.Enqueue(imageAnalyzer1);

                                currentFrameModel = null;
                                imageAnalyzer1 = null;

                                this.isReadQueue1 = false;
                            }
                            else if (RealtimeFixModel.ListFrame2.Count > 0)
                            {
                                this.isReadQueue1 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame2)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame2.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer2 = await this.cameraControl2.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer2 != null)
                                    RealtimeFixModel.ListImageDetect2.Enqueue(imageAnalyzer2);

                                currentFrameModel = null;
                                imageAnalyzer2 = null;

                                this.isReadQueue1 = false;
                            }
                            else if (RealtimeFixModel.ListFrame3.Count > 0)
                            {
                                this.isReadQueue1 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame3)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame3.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer3 = await this.cameraControl3.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer3 != null)
                                    RealtimeFixModel.ListImageDetect3.Enqueue(imageAnalyzer3);

                                currentFrameModel = null;
                                imageAnalyzer3 = null;

                                this.isReadQueue1 = false;
                            }
                            else if (RealtimeFixModel.ListFrame4.Count > 0)
                            {
                                this.isReadQueue1 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame4)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame4.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer4 = await this.cameraControl4.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer4 != null)
                                    RealtimeFixModel.ListImageDetect4.Enqueue(imageAnalyzer4);

                                currentFrameModel = null;
                                imageAnalyzer4 = null;

                                this.isReadQueue1 = false;
                            }
                            CoreUtil.FreeMemory();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.isReadQueue1 = false;
                        LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " ProcessingReadQueue1: " + ex.Message + ex.Message + "\n\r");
                    }
                });

                await Task.Delay(TimeRead);
            }
        }

        private async void ProcessingReadQueue2()
        {
            while (this.isProcessingReadQueue)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        if (!this.isReadQueue2 && RealtimeFixModel.ListImageDetect2.Count > 0)
                        {
                            LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " Count ListImageDetect2: " + RealtimeFixModel.ListImageDetect2.Count + "\n\r");
                            this.isReadQueue2 = true;

                            ImageAnalyzer imageAnalyzer;
                            lock (RealtimeFixModel.ListImageDetect2)
                            {
                                imageAnalyzer = RealtimeFixModel.ListImageDetect2.Dequeue();
                            }

                            await this.DetectFaceAsync2(imageAnalyzer);

                            imageAnalyzer = null;
                        }
                        else if (!this.isReadQueue2)
                        {
                            if (RealtimeFixModel.ListFrame2.Count > 0)
                            {
                                this.isReadQueue2 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame2)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame2.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer2 = await this.cameraControl2.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer2 != null)
                                    RealtimeFixModel.ListImageDetect2.Enqueue(imageAnalyzer2);

                                currentFrameModel = null;
                                imageAnalyzer2 = null;

                                this.isReadQueue2 = false;
                            }
                            else if (RealtimeFixModel.ListFrame3.Count > 0)
                            {
                                this.isReadQueue2 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame3)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame3.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer3 = await this.cameraControl3.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer3 != null)
                                    RealtimeFixModel.ListImageDetect3.Enqueue(imageAnalyzer3);

                                currentFrameModel = null;
                                imageAnalyzer3 = null;

                                this.isReadQueue2 = false;
                            }
                            else if (RealtimeFixModel.ListFrame4.Count > 0)
                            {
                                this.isReadQueue2 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame4)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame4.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer4 = await this.cameraControl4.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer4 != null)
                                    RealtimeFixModel.ListImageDetect4.Enqueue(imageAnalyzer4);

                                currentFrameModel = null;
                                imageAnalyzer4 = null;

                                this.isReadQueue2 = false;
                            }
                            else if (RealtimeFixModel.ListFrame1.Count > 0)
                            {
                                this.isReadQueue2 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame1)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame1.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer1 = await this.cameraControl1.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer1 != null)
                                    RealtimeFixModel.ListImageDetect1.Enqueue(imageAnalyzer1);

                                currentFrameModel = null;
                                imageAnalyzer1 = null;

                                this.isReadQueue2 = false;
                            }
                            CoreUtil.FreeMemory();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.isReadQueue2 = false;
                        LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " ProcessingReadQueue2: " + ex.Message + ex.Message + "\n\r");
                    }
                });

                await Task.Delay(TimeRead);
            }
        }

        private async void ProcessingReadQueue3()
        {
            while (this.isProcessingReadQueue)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        if (!this.isReadQueue3 && RealtimeFixModel.ListImageDetect3.Count > 0)
                        {
                            LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " Count ListImageDetect3: " + RealtimeFixModel.ListImageDetect3.Count + "\n\r");
                            this.isReadQueue3 = true;

                            ImageAnalyzer imageAnalyzer;
                            lock (RealtimeFixModel.ListImageDetect3)
                            {
                                imageAnalyzer = RealtimeFixModel.ListImageDetect3.Dequeue();
                            }

                            await this.DetectFaceAsync3(imageAnalyzer);

                            imageAnalyzer = null;
                        }
                        //Nếu rảnh chuyển sang ghi Queue
                        else if (!this.isReadQueue3)
                        {
                            if (RealtimeFixModel.ListFrame3.Count > 0)
                            {
                                this.isReadQueue3 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame3)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame3.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer3 = await this.cameraControl3.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer3 != null)
                                    RealtimeFixModel.ListImageDetect3.Enqueue(imageAnalyzer3);

                                currentFrameModel = null;
                                imageAnalyzer3 = null;

                                this.isReadQueue3 = false;
                            }
                            else if (RealtimeFixModel.ListFrame4.Count > 0)
                            {
                                this.isReadQueue3 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame4)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame4.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer4 = await this.cameraControl4.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer4 != null)
                                    RealtimeFixModel.ListImageDetect4.Enqueue(imageAnalyzer4);

                                currentFrameModel = null;
                                imageAnalyzer4 = null;

                                this.isReadQueue3 = false;
                            }
                            else if (RealtimeFixModel.ListFrame1.Count > 0)
                            {
                                this.isReadQueue3 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame1)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame1.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer1 = await this.cameraControl1.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer1 != null)
                                    RealtimeFixModel.ListImageDetect1.Enqueue(imageAnalyzer1);

                                currentFrameModel = null;
                                imageAnalyzer1 = null;

                                this.isReadQueue3 = false;
                            }
                            else if (RealtimeFixModel.ListFrame2.Count > 0)
                            {
                                this.isReadQueue3 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame2)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame2.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer2 = await this.cameraControl2.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer2 != null)
                                    RealtimeFixModel.ListImageDetect2.Enqueue(imageAnalyzer2);

                                currentFrameModel = null;
                                imageAnalyzer2 = null;

                                this.isReadQueue3 = false;
                            }
                            CoreUtil.FreeMemory();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.isReadQueue3 = false;
                        LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " ProcessingReadQueue3: " + ex.Message + ex.Message + "\n\r");
                    }
                });

                await Task.Delay(TimeRead);
            }
        }

        private async void ProcessingReadQueue4()
        {
            while (this.isProcessingReadQueue)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        if (!this.isReadQueue4 && RealtimeFixModel.ListImageDetect4.Count > 0)
                        {
                            LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " Count ListImageDetect4: " + RealtimeFixModel.ListImageDetect4.Count + "\n\r");
                            this.isReadQueue4 = true;

                            ImageAnalyzer imageAnalyzer;
                            lock (RealtimeFixModel.ListImageDetect4)
                            {
                                imageAnalyzer = RealtimeFixModel.ListImageDetect4.Dequeue();
                            }

                            await this.DetectFaceAsync4(imageAnalyzer);

                            imageAnalyzer = null;
                        }
                        //Nếu rảnh chuyển sang ghi Queue
                        else if (!this.isReadQueue4)
                        {
                            if (RealtimeFixModel.ListFrame4.Count > 0)
                            {
                                this.isReadQueue4 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame4)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame4.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer4 = await this.cameraControl4.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer4 != null)
                                    RealtimeFixModel.ListImageDetect4.Enqueue(imageAnalyzer4);

                                currentFrameModel = null;
                                imageAnalyzer4 = null;

                                this.isReadQueue4 = false;
                            }
                            else if (RealtimeFixModel.ListFrame1.Count > 0)
                            {
                                this.isReadQueue4 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame1)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame1.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer1 = await this.cameraControl1.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer1 != null)
                                    RealtimeFixModel.ListImageDetect1.Enqueue(imageAnalyzer1);

                                currentFrameModel = null;
                                imageAnalyzer1 = null;

                                this.isReadQueue4 = false;
                            }
                            else if (RealtimeFixModel.ListFrame2.Count > 0)
                            {
                                this.isReadQueue4 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame2)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame2.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer2 = await this.cameraControl2.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer2 != null)
                                    RealtimeFixModel.ListImageDetect2.Enqueue(imageAnalyzer2);

                                currentFrameModel = null;
                                imageAnalyzer2 = null;

                                this.isReadQueue4 = false;
                            }
                            else if (RealtimeFixModel.ListFrame3.Count > 0)
                            {
                                this.isReadQueue4 = true;

                                CurrentFrameModel currentFrameModel;
                                lock (RealtimeFixModel.ListFrame3)
                                {
                                    currentFrameModel = RealtimeFixModel.ListFrame3.Dequeue();
                                }

                                ImageAnalyzer imageAnalyzer3 = await this.cameraControl3.CaptureFrameAsync(currentFrameModel);
                                if (imageAnalyzer3 != null)
                                    RealtimeFixModel.ListImageDetect3.Enqueue(imageAnalyzer3);

                                currentFrameModel = null;
                                imageAnalyzer3 = null;

                                this.isReadQueue4 = false;
                            }

                            CoreUtil.FreeMemory();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.isReadQueue4 = false;
                        LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " ProcessingReadQueue4: " + ex.Message + ex.Message + "\n\r");
                    }
                });

                await Task.Delay(TimeRead);
            }
        }

        private async Task DetectFaceAsync1(ImageAnalyzer e)
        {
            if (e == null)
            {
                return;
            }

            try
            {             //Detect face
                await e.DetectFacesAsync(detectFaceAttributes: true);

                //Hiển thị thông tin detect lên giao diện
                //this.ViewPersonsCurrent(e);
            }
            catch (Exception ex)
            {
                LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " DetectFaceAsync1: " + ex.Message + ex.Message + "\n\r");
            }

            isReadQueue1 = false;
            CoreUtil.FreeMemory();
        }
        private async Task DetectFaceAsync2(ImageAnalyzer e)
        {
            if (e == null)
            {
                return;
            }

            try
            {
                //Detect face
                await e.DetectFacesAsync(detectFaceAttributes: true);

                //Hiển thị thông tin detect lên giao diện
                //this.ViewPersonsCurrent(e);
            }
            catch (Exception ex)
            {
                LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " DetectFaceAsync2: " + ex.Message + ex.Message + "\n\r");
            }

            isReadQueue2 = false;
            CoreUtil.FreeMemory();
        }
        private async Task DetectFaceAsync3(ImageAnalyzer e)
        {
            if (e == null)
            {
                return;
            }

            try
            {
                //Detect face
                await e.DetectFacesAsync(detectFaceAttributes: true);

                //Hiển thị thông tin detect lên giao diện
                //this.ViewPersonsCurrent(e);
            }
            catch (Exception ex)
            {
                LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " DetectFaceAsync3: " + ex.Message + ex.Message + "\n\r");
            }

            isReadQueue3 = false;
            CoreUtil.FreeMemory();
        }
        private async Task DetectFaceAsync4(ImageAnalyzer e)
        {
            if (e == null)
            {
                return;
            }

            try
            {
                //Detect face
                await e.DetectFacesAsync(detectFaceAttributes: true);

                //Hiển thị thông tin detect lên giao diện
                //this.ViewPersonsCurrent(e);
            }
            catch (Exception ex)
            {
                LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " DetectFaceAsync4: " + ex.Message + ex.Message + "\n\r");
            }

            isReadQueue4 = false;
            CoreUtil.FreeMemory();
        }

        /// <summary>
        /// Hiển thị thông tin detect lên giao diện
        /// </summary>
        /// <param name="e"></param>
        private void ViewPersonsCurrent(ImageAnalyzer e)
        {
            try
            {
                if (e.IdentifiedPersons != null)
                {
                    foreach (var item in e.IdentifiedPersons)
                    {
                        //Không tồn tại thì thêm mới
                        if (item.InfoEmployee != null)
                        {
                            EmployeeModel itemPersons = new EmployeeModel()
                            {
                                DateTime = e.CaptureTime.ToString("HH:mm:ss"),
                                Name = item.InfoEmployee.Name + " (" + item.InfoEmployee.Code + ") ",
                                JobTitle = item.InfoEmployee.JobTitleName + " / " + item.InfoEmployee.DepartmentName,
                                Type = "1",
                                Color = "#00265c"
                            };
                            this.PersonsInCurrentGroup.Insert(0, itemPersons);

                            if (this.PersonsInCurrentGroup.Count() > 200)
                                this.PersonsInCurrentGroup.RemoveAt(200);

                            LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " PersonsInCurrentGroup: " + PersonsInCurrentGroup.Count + "\n\r");
                            itemPersons = null;
                        }
                    }
                    e = null;
                }
            }
            catch (Exception ex)
            {
                LogWriter.GetInstance().Write(DateTime.Now.ToString("HH:mm:ss") + " ViewPersonsCurrent: " + ex.Message + "\n\r");
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //Start camera control
            this.StartCamera();

            Task.Run(() => this.cameraControl1.ProcessingStreamAsync());
            Task.Run(() => this.cameraControl2.ProcessingStreamAsync());
            Task.Run(() => this.cameraControl3.ProcessingStreamAsync());
            Task.Run(() => this.cameraControl4.ProcessingStreamAsync());

            //Camera control
            this.StartWriteQueue();

            //
            this.StartReadQueue();

            base.OnNavigatedTo(e);
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            this.isProcessingReadQueue = false;
            this.isProcessingWriteQueue = false;
            //Camera control
            this.cameraControl1.CameraAspectRatioChanged -= CameraControl_CameraAspectRatioChanged;

            //Camera control
            await this.cameraControl1.StopStreamAsync();
            await this.cameraControl2.StopStreamAsync();
            await this.cameraControl3.StopStreamAsync();
            await this.cameraControl4.StopStreamAsync();
            base.OnNavigatingFrom(e);
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCameraHostSize();
        }

        private void UpdateCameraHostSize()
        {
            //Camera control
            this.cameraHostGrid1.Width = this.cameraHostGrid1.ActualHeight * (this.cameraControl1.CameraAspectRatio != 0 ? this.cameraControl1.CameraAspectRatio : 1.777777777777);
            cameraControl1.SetSizeControl(this.cameraHostGrid1.Width);
            this.cameraHostGrid2.Width = this.cameraHostGrid2.ActualHeight * (this.cameraControl2.CameraAspectRatio != 0 ? this.cameraControl2.CameraAspectRatio : 1.777777777777);
            cameraControl2.SetSizeControl(this.cameraHostGrid2.Width);
            this.cameraHostGrid3.Width = this.cameraHostGrid3.ActualHeight * (this.cameraControl3.CameraAspectRatio != 0 ? this.cameraControl3.CameraAspectRatio : 1.777777777777);
            cameraControl3.SetSizeControl(this.cameraHostGrid3.Width);
            this.cameraHostGrid4.Width = this.cameraHostGrid4.ActualHeight * (this.cameraControl4.CameraAspectRatio != 0 ? this.cameraControl4.CameraAspectRatio : 1.777777777777);
            cameraControl4.SetSizeControl(this.cameraHostGrid4.Width);
        }
    }
}