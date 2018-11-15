using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Model
{
    public class InfoNotificationResult
    {
        public String AttendanceLogId { get; set; }
        public String Code { get; set; }
        public String Name { get; set; }
        public String DepartmentName { get; set; }
        public String JobTitle { get; set; }
        public String ImageLink { get; set; }
        public String DateLog { get; set; }
        public String TimeLog { get; set; }
        public float Confidence { get; set; }
    }
}
