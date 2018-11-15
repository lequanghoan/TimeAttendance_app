using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace TimeAttendance.Client.Model
{
    public class CurrentFrameModel
    {
        public byte[] DataCurrent { get; set; }
        public DateTime CaptureTime { get; set; }
    }
}
