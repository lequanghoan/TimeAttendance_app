using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Client.Model
{
    public static class InfoSettingFix
    {
        public static Dictionary<string, DateTime> DictionaryList;
        public static DateTime DateNow = DateTime.Now;
        public static InfoSettingModel InfoSetting;
        public static uint FixTakeWidth = 1280;
        public static uint FixTakeHeight = 720;
        public static int FixImageWidth = 1280;
        public static int FixImageHeight = 720;
    }
}
