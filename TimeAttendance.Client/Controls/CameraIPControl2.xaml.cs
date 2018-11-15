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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using TimeAttendance.Client.Model;
using TimeAttendance.Client.Util;
using VLC;
using Windows.Graphics.Imaging;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TimeAttendance.Client.Controls
{
    public sealed partial class CameraIPControl2 : UserControl
    {
        public event EventHandler<ImageAnalyzer> ImageCaptured;
        public event EventHandler<AutoCaptureState> AutoCaptureStateChanged;
        public event EventHandler CameraRestarted;
        public event EventHandler CameraAspectRatioChanged;

        public static readonly DependencyProperty ShowDialogOnApiErrorsProperty =
            DependencyProperty.Register(
            "ShowDialogOnApiErrors",
            typeof(bool),
            typeof(CameraControl),
            new PropertyMetadata(true)
            );

        public bool ShowDialogOnApiErrors
        {
            get { return (bool)GetValue(ShowDialogOnApiErrorsProperty); }
            set { SetValue(ShowDialogOnApiErrorsProperty, (bool)value); }
        }

        public bool FilterOutSmallFaces
        {
            get;
            set;
        }

        private bool enableAutoCaptureMode;
        public bool EnableAutoCaptureMode
        {
            get
            {
                return enableAutoCaptureMode;
            }
            set
            {
                this.enableAutoCaptureMode = value;
                this.commandBar.Visibility = this.enableAutoCaptureMode ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public double CameraAspectRatio { get; set; }
        public int CameraResolutionWidth { get; private set; }
        public int CameraResolutionHeight { get; private set; }

        public int NumFacesOnLastFrame { get; set; }

        private bool isProcessingLoopInProgress = true;
        private AutoCaptureState autoCaptureState;
        private IEnumerable<DetectedFace> detectedFacesFromPreviousFrame;
        private DateTime timeSinceWaitingForStill;
        private DateTime lastTimeWhenAFaceWasDetected;

        private IRealTimeDataProvider realTimeDataProvider;
        private readonly uint sourceImageHeightLimit = InfoSettingFix.FixTakeHeight;
        private string path = Path.GetTempFileName();
        public string CameraIPAdres { get; set; }
        public BitmapBounds SearchArea { get; set; }
        private int TimeDelay = 1000 / InfoSettingFix.InfoSetting.TotalFrame;//4fps
        private int ByteLength = 0;
        private ImageAnalyzer imageWithFace { get; set; }
        private byte[] data { get; set; }
        private IList<DetectedFace> faces { get; set; }
        private CurrentFrameModel CurrentFrame { get; set; }

        public CameraIPControl2()
        {
            this.InitializeComponent();
        }

        #region Camera stream processing

        public void MediaSource(IMediaSource mediaSource)
        {
            mediaElement.MediaSource = mediaSource;
        }

        public async Task ProcessingStreamAsync()
        {
            try
            {
                while (isProcessingLoopInProgress)
                {
                    await ProcessCurrentVideoFrame();
                    await Task.Delay(TimeDelay);
                }
            }
            catch (Exception ex)
            {
                await CoreUtil.GenericApiCallExceptionHandler(ex, "Lỗi khởi động camera.");
            }
        }

        private async Task ProcessCurrentVideoFrame()
        {
            try
            {
                if (mediaElement.MediaPlayer == null)
                {
                    return;
                }

                mediaElement.MediaPlayer.takeSnapshot(0, path, InfoSettingFix.FixTakeWidth, InfoSettingFix.FixTakeHeight);

                data = File.ReadAllBytes(path);
                int byteLength = data.Length;
                //Trường hợp data trống và byte 2 Frame liên tiếp bằng nhau
                if (data == null || byteLength == 0 || byteLength == this.ByteLength)
                {
                    return;
                }

                this.ByteLength = byteLength;
                CurrentFrame = new CurrentFrameModel()
                {
                    CaptureTime = DateTime.Now,
                    DataCurrent = data
                };
                RealtimeFixModel.ListFrame2.Enqueue(CurrentFrame);
                data = null;
            }
            catch (Exception ex)
            {
                return;
            }
            CoreUtil.FreeMemory();
        }

        private void ShowFaceTrackingVisualization(Windows.Foundation.Size framePixelSize, IEnumerable<DetectedFace> detectedFaces)
        {
            this.FaceTrackingVisualizationCanvas.Children.Clear();

            var actualWidthMedia = mediaElement.ActualWidth;
            var actualHeightMedia = mediaElement.ActualHeight;

            double actualWidth = this.FaceTrackingVisualizationCanvas.ActualWidth;
            double actualHeight = this.FaceTrackingVisualizationCanvas.ActualHeight;

            if (detectedFaces != null && actualWidth != 0 && actualHeight != 0)
            {
                double widthScale = framePixelSize.Width / actualWidth;
                double heightScale = framePixelSize.Height / actualHeight;

                foreach (DetectedFace face in detectedFaces)
                {
                    RealTimeFaceIdentificationBorder faceBorder = new RealTimeFaceIdentificationBorder();
                    this.FaceTrackingVisualizationCanvas.Children.Add(faceBorder);

                    faceBorder.ShowFaceRectangle((uint)(face.FaceBox.X / widthScale), (uint)(face.FaceBox.Y / heightScale), (uint)(face.FaceBox.Width / widthScale), (uint)(face.FaceBox.Height / heightScale));
                }
            }
        }

        private async void UpdateAutoCaptureState(IEnumerable<DetectedFace> detectedFaces)
        {
            const int IntervalBeforeCheckingForStill = 500;
            const int IntervalWithoutFacesBeforeRevertingToWaitingForFaces = 3;

            if (!detectedFaces.Any())
            {
                if (this.autoCaptureState == AutoCaptureState.WaitingForStillFaces &&
                    (DateTime.Now - this.lastTimeWhenAFaceWasDetected).TotalSeconds > IntervalWithoutFacesBeforeRevertingToWaitingForFaces)
                {
                    this.autoCaptureState = AutoCaptureState.WaitingForFaces;
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        this.OnAutoCaptureStateChanged(this.autoCaptureState);
                    });
                }

                return;
            }

            this.lastTimeWhenAFaceWasDetected = DateTime.Now;

            switch (this.autoCaptureState)
            {
                case AutoCaptureState.WaitingForFaces:
                    // We were waiting for faces and got some... go to the "waiting for still" state
                    this.detectedFacesFromPreviousFrame = detectedFaces;
                    this.timeSinceWaitingForStill = DateTime.Now;
                    this.autoCaptureState = AutoCaptureState.WaitingForStillFaces;

                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        this.OnAutoCaptureStateChanged(this.autoCaptureState);
                    });

                    break;

                case AutoCaptureState.WaitingForStillFaces:
                    // See if we have been waiting for still faces long enough
                    if ((DateTime.Now - this.timeSinceWaitingForStill).TotalMilliseconds >= IntervalBeforeCheckingForStill)
                    {
                        // See if the faces are still enough
                        if (this.AreFacesStill(this.detectedFacesFromPreviousFrame, detectedFaces))
                        {
                            this.autoCaptureState = AutoCaptureState.ShowingCountdownForCapture;
                            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                this.OnAutoCaptureStateChanged(this.autoCaptureState);
                            });
                        }
                        else
                        {
                            // Faces moved too much, update the baseline and keep waiting
                            this.timeSinceWaitingForStill = DateTime.Now;
                            this.detectedFacesFromPreviousFrame = detectedFaces;
                        }
                    }
                    break;

                case AutoCaptureState.ShowingCountdownForCapture:
                    break;

                case AutoCaptureState.ShowingCapturedPhoto:
                    break;

                default:
                    break;
            }
        }

        public async Task<ImageAnalyzer> TakeAutoCapturePhoto()
        {
            var image = await CaptureFrameAsync(new CurrentFrameModel());
            this.autoCaptureState = AutoCaptureState.ShowingCapturedPhoto;
            this.OnAutoCaptureStateChanged(this.autoCaptureState);
            return image;
        }

        public void RestartAutoCaptureCycle()
        {
            this.autoCaptureState = AutoCaptureState.WaitingForFaces;
            this.OnAutoCaptureStateChanged(this.autoCaptureState);
        }

        private bool AreFacesStill(IEnumerable<DetectedFace> detectedFacesFromPreviousFrame, IEnumerable<DetectedFace> detectedFacesFromCurrentFrame)
        {
            int horizontalMovementThreshold = (int)(InfoSettingFix.FixImageWidth * 0.02);
            int verticalMovementThreshold = (int)(InfoSettingFix.FixImageHeight * 0.02);

            if (detectedFacesFromPreviousFrame != null)
            {
                int numStillFaces = 0;
                int totalFacesInPreviousFrame = detectedFacesFromPreviousFrame.Count();

                foreach (DetectedFace faceInPreviousFrame in detectedFacesFromPreviousFrame)
                {
                    if (numStillFaces > 0 && numStillFaces >= totalFacesInPreviousFrame / 2)
                    {
                        // If half or more of the faces in the previous frame are considered still we can stop. It is still enough.
                        break;
                    }

                    // If there is a face in the current frame that is located close enough to this one in the previous frame, we 
                    // assume it is the same face and count it as a still face. 
                    if (detectedFacesFromCurrentFrame.Any(f => Math.Abs((int)faceInPreviousFrame.FaceBox.X - (int)f.FaceBox.X) <= horizontalMovementThreshold &&
                                                               Math.Abs((int)faceInPreviousFrame.FaceBox.Y - (int)f.FaceBox.Y) <= verticalMovementThreshold))
                    {
                        numStillFaces++;
                    }
                }

                if (numStillFaces > 0 && numStillFaces >= totalFacesInPreviousFrame / 2)
                {
                    // If half or more of the faces in the previous frame are considered still we consider the group as still
                    return true;
                }
            }

            return false;
        }

        public async Task StopStreamAsync()
        {
            try
            {
                isProcessingLoopInProgress = false;
                mediaElement.MediaSource = null;
                this.FaceTrackingVisualizationCanvas.Children.Clear();
            }
            catch (Exception)
            {
                //await Util.GenericApiCallExceptionHandler(ex, "Error stopping the camera.");
            }
        }

        public async Task<ImageAnalyzer> CaptureFrameAsync(CurrentFrameModel currentFrame)
        {
            try
            {
                using (Stream stream = currentFrame.DataCurrent.AsBuffer().AsStream())
                {
                    stream.Position = 0;
                    var decoder = await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());
                    var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                    var detector = await FaceDetector.CreateAsync();
                    using (SoftwareBitmap convertedBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Gray8))
                    {
                        faces = await detector.DetectFacesAsync(convertedBitmap, SearchArea);
                        convertedBitmap.Dispose();
                    }

                    this.NumFacesOnLastFrame = faces.Count();

                    var previewFrameSize = new Windows.Foundation.Size(softwareBitmap.PixelWidth, softwareBitmap.PixelHeight);
                    this.ShowFaceTrackingVisualization(previewFrameSize, faces);

                    softwareBitmap.Dispose();
                    stream.Dispose();
                }

                //Không có face thì không phân tích
                if (this.NumFacesOnLastFrame == 0)
                {
                    faces = null;
                    CoreUtil.FreeMemory();
                    return null;
                }

                //Hai khung hình có số lượng khung mật giống nhau quá nửa thì không phân tích nữa
                if (this.AreFacesStill(this.detectedFacesFromPreviousFrame, faces))
                {
                    faces = null;
                    CoreUtil.FreeMemory();
                    return null;
                }

                this.detectedFacesFromPreviousFrame = faces;

                imageWithFace = new ImageAnalyzer(currentFrame.DataCurrent);

                imageWithFace.CameraIPAdres = CameraIPAdres;
                imageWithFace.imageWidth = InfoSettingFix.FixImageWidth;
                imageWithFace.imageHeight = InfoSettingFix.FixImageHeight;
                imageWithFace.CaptureTime = currentFrame.CaptureTime;
                imageWithFace.ListDetectedFaceJson = JsonConvert.SerializeObject(faces.Select(r => r.FaceBox).ToList());

                faces = null;
                CoreUtil.FreeMemory();

                return imageWithFace;

            }
            catch (Exception ex)
            {
                CoreUtil.FreeMemory();
                return null;
            }
        }

        private void OnImageCaptured(ImageAnalyzer imageWithFace)
        {
            if (this.ImageCaptured != null)
            {
                this.ImageCaptured(this, imageWithFace);
            }
        }

        private void OnAutoCaptureStateChanged(AutoCaptureState state)
        {
            if (this.AutoCaptureStateChanged != null)
            {
                this.AutoCaptureStateChanged(this, state);
            }
        }

        #endregion

        public void HideCameraControls()
        {
            this.commandBar.Visibility = Visibility.Collapsed;
        }

        public void SetRealTimeDataProvider(IRealTimeDataProvider provider)
        {
            this.realTimeDataProvider = provider;
        }

        private async void CameraControlButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.cameraControlSymbol.Symbol == Symbol.Camera)
            {
                var img = await CaptureFrameAsync(new CurrentFrameModel());
                if (img != null)
                {
                    this.cameraControlSymbol.Symbol = Symbol.Refresh;
                    this.OnImageCaptured(img);
                }
            }
            else
            {
                this.cameraControlSymbol.Symbol = Symbol.Camera;

                await ProcessingStreamAsync();

                this.CameraRestarted?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Computes a BitmapTransform to downscale the source image if it's too large. 
        /// </summary>
        /// <remarks>
        /// Performance of the FaceDetector degrades significantly with large images, and in most cases it's best to downscale
        /// the source bitmaps if they're too large before passing them into FaceDetector. Remember through, your application's performance needs will vary.
        /// </remarks>
        /// <param name="sourceDecoder">Source image decoder</param>
        /// <returns>A BitmapTransform object holding scaling values if source image is too large</returns>
        private BitmapTransform ComputeScalingTransformForSourceImage(BitmapDecoder sourceDecoder)
        {
            BitmapTransform transform = new BitmapTransform();

            if (sourceDecoder.PixelHeight > this.sourceImageHeightLimit)
            {
                float scalingFactor = (float)this.sourceImageHeightLimit / (float)sourceDecoder.PixelHeight;

                transform.ScaledWidth = (uint)Math.Floor(sourceDecoder.PixelWidth * scalingFactor);
                transform.ScaledHeight = (uint)Math.Floor(sourceDecoder.PixelHeight * scalingFactor);
            }

            return transform;
        }

        /// <summary>
        /// Set size control
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetSizeControl(double width)
        {
            mediaElement.Width = width;
            FaceTrackingVisualizationCanvas.Width = width;
        }
    }
}
