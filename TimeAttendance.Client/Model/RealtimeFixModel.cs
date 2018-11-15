using System;
using System.Collections.Generic;

namespace TimeAttendance.Client.Model
{
    public static class RealtimeFixModel
    {
        public static Dictionary<string, DateTime> ListEmployeeRealtime;
        public static int TimeFix = 30;
        public static Queue<ImageAnalyzer> ListImageDetect1 = new Queue<ImageAnalyzer>();
        public static Queue<ImageAnalyzer> ListImageDetect2 = new Queue<ImageAnalyzer>();
        public static Queue<ImageAnalyzer> ListImageDetect3 = new Queue<ImageAnalyzer>();
        public static Queue<ImageAnalyzer> ListImageDetect4 = new Queue<ImageAnalyzer>();
        public static Queue<CurrentFrameModel> ListFrame1 = new Queue<CurrentFrameModel>();
        public static Queue<CurrentFrameModel> ListFrame2 = new Queue<CurrentFrameModel>();
        public static Queue<CurrentFrameModel> ListFrame3 = new Queue<CurrentFrameModel>();
        public static Queue<CurrentFrameModel> ListFrame4 = new Queue<CurrentFrameModel>();
    }
}
