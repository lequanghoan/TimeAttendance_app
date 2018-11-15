using System;
using System.Collections.Generic;
using System.IO;

namespace TimeAttendance.Client.Model
{
    public class DetectFaceModel
    {
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public string CameraIPAdres { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<FaceBox> ListFaces { get; set; }
        public DateTime CaptureTime { get; set; }
    }
}
