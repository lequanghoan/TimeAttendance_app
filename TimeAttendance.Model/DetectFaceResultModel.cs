using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model
{
    public class DetectFaceResultModel
    {
        public string CameraIPAdress { get; set; }
        public string LogImageLink { get; set; }
        public DateTime CaptureTime { get; set; }
        public IEnumerable<IdentifyResult> ListIdentifyResult { get; set; }
    }
}
