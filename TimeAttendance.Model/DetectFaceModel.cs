using System;
using System.Collections.Generic;
using System.IO;

namespace TimeAttendance.Model
{
    public class DetectFaceModel
    {
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public string ClientIPAddress { get; set; }
        public string CameraIPAdres { get; set; }
        public Stream ImageStream { get; set; }
        public string ImageUrl { get; set; }
        public string LogImageLink { get; set; }
        public IEnumerable<FaceBox> ListFaces { get; set; }
        public DateTime CaptureTime { get; set; }
    }
}
